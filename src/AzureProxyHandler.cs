using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using Microsoft.ServiceBus;

namespace ManagedFusion.Rewriter
{
	public class AzureProxyHandler : ProxyHandler
	{
		private BasicHttpRelayBinding _requestBinding;
		private TransportClientEndpointBehavior _credentials;
		private MessageEncoder _webEncoder;
		private IChannelListener<IReplyChannel> _replyChannelListener;

		public AzureProxyHandler(Uri requestUrl, Uri responseUrl)
		{
			_credentials = new TransportClientEndpointBehavior {
				CredentialType = TransportClientCredentialType.SharedSecret
			};
			_credentials.Credentials.SharedSecret.IssuerName = Manager.Configuration.Azure.IssuerName;
			_credentials.Credentials.SharedSecret.IssuerSecret = Manager.Configuration.Azure.IssuerSecret;

			Init(requestUrl, responseUrl);
		
			ServicePointManager.DefaultConnectionLimit = 50;

			_requestBinding = new BasicHttpRelayBinding(EndToEndBasicHttpSecurityMode.None, RelayClientAuthenticationType.None);
			_requestBinding.MaxReceivedMessageSize = int.MaxValue;
			_requestBinding.TransferMode = TransferMode.Streamed;
			_requestBinding.AllowCookies = false;
			_requestBinding.ReceiveTimeout = TimeSpan.MaxValue;
			_requestBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
			_requestBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
			_requestBinding.MaxReceivedMessageSize = int.MaxValue;
			_requestBinding.MaxBufferSize = 4 * 1024 * 1024;
			_requestBinding.MaxBufferPoolSize = 32 * 4 * 1024 * 1024;

			WebMessageEncodingBindingElement encoderBindingElement = new WebMessageEncodingBindingElement();
			encoderBindingElement.ReaderQuotas.MaxArrayLength = int.MaxValue;
			encoderBindingElement.ReaderQuotas.MaxStringContentLength = int.MaxValue;
			encoderBindingElement.ReaderQuotas.MaxDepth = 128;
			encoderBindingElement.ReaderQuotas.MaxBytesPerRead = 65536;
			encoderBindingElement.ContentTypeMapper = new RawContentTypeMapper();
			_webEncoder = encoderBindingElement.CreateMessageEncoderFactory().Encoder;
		}

		public void Open()
		{
			_replyChannelListener = _requestBinding.BuildChannelListener<IReplyChannel>(ResponseUrl, _credentials);
			_replyChannelListener.Open();
			_replyChannelListener.BeginAcceptChannel(ChannelAccepted, _replyChannelListener);
		}

		public void Close()
		{
			if (_replyChannelListener != null)
				_replyChannelListener.Close();
		}

		private void ChannelAccepted(IAsyncResult result)
		{
			try
			{
				IReplyChannel replyChannel = _replyChannelListener.EndAcceptChannel(result);
				if (replyChannel != null)
				{
					try
					{
						replyChannel.Open();
						replyChannel.BeginReceiveRequest(RequestAccepted, replyChannel);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						replyChannel.Abort();
					}

					if (_replyChannelListener.State == CommunicationState.Opened)
					{
						this._replyChannelListener.BeginAcceptChannel(ChannelAccepted, _replyChannelListener);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				this._replyChannelListener.Abort();
				this._replyChannelListener = null;
			}
		}

		private void RequestAccepted(IAsyncResult result)
		{
			IReplyChannel replyChannel = (IReplyChannel)result.AsyncState;

			try
			{
				var requestContext = replyChannel.EndReceiveRequest(result);

				if (requestContext != null)
				{
					var context = new AzureServiceBusHttpContext(requestContext);

					context.Items["ReplyChannel"] = replyChannel;

					ProcessRequest(context);
				}
			}
			catch (Exception exc)
			{
				Manager.Log("Request Accepted: " + exc.Message, "Proxy");
				replyChannel.Abort();
			}
		}

		public override void WriteStreamRequestToTarget(HttpContextBase context, WebRequest request)
		{
			var serviceBusContext = (AzureServiceBusHttpContext)context;
			var requestContext = serviceBusContext.RequestContext;

			using (var requestStream = request.GetRequestStream())
			{
				_webEncoder.WriteMessage(requestContext.RequestMessage, requestStream);
			}
		}

		private void DoneReplying(IAsyncResult result)
		{
			var context = (HttpContextBase)((object[])result.AsyncState)[0];
			var response = (WebResponse)((object[])result.AsyncState)[1];
			var serviceBusContext = (AzureServiceBusHttpContext)context;
			var requestContext = serviceBusContext.RequestContext;
			IReplyChannel replyChannel = (IReplyChannel)context.Items["ReplyChannel"];

			try
			{
				requestContext.EndReply(result);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				if (response != null)
					response.Close();

				replyChannel.BeginReceiveRequest(RequestAccepted, replyChannel);
			}
		}

		public override void WriteStreamResponseToTarget(HttpContextBase context, WebResponse response)
		{
			var serviceBusContext = (AzureServiceBusHttpContext)context;
			var serviceBusResponse = (AzureServiceBusHttpResponse)serviceBusContext.Response;
			var requestContext = serviceBusContext.RequestContext;
			var upstreamReply = (Message)null;
			var responseStream = response.GetResponseStream();

			Message downstreamReply = _webEncoder.ReadMessage(responseStream, 0x200000, response.ContentType);
			upstreamReply = Message.CreateMessage(MessageVersion.None, "RESPONSE", downstreamReply.GetReaderAtBodyContents());

			serviceBusResponse.Message = upstreamReply;
			requestContext.BeginReply(upstreamReply, DoneReplying, new object[] { context, response });
		}

		private class RawContentTypeMapper : WebContentTypeMapper
		{
			public override WebContentFormat GetMessageFormatForContentType(string contentType)
			{
				return WebContentFormat.Raw;
			}
		}
	}
}
