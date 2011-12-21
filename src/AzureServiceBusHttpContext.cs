using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Web;
using System.Collections;

namespace ManagedFusion.Rewriter
{
	public class AzureServiceBusHttpContext : HttpContextBase
	{
		private RequestContext _context;
		private AzureServiceBusHttpRequest _request;
		private AzureServiceBusHttpResponse _response;

		private IDictionary _items;

		public AzureServiceBusHttpContext(RequestContext context)
		{
			_context = context;
			_items = new Hashtable();
		}

		public override IDictionary Items
		{
			get { return _items; }
		}

		public override HttpRequestBase Request
		{
			get
			{
				if (_request == null)
					_request = new AzureServiceBusHttpRequest(_context);

				return _request;
			}
		}

		public override HttpResponseBase Response
		{
			get
			{
				if (_response == null)
					_response = new AzureServiceBusHttpResponse(_context);

				return _response;
			}
		}

		public RequestContext RequestContext
		{
			get { return _context; }
		}
	}

	public class AzureServiceBusHttpResponse : HttpResponseBase
	{
		private RequestContext _context;
		private Message _response;
		private HttpResponseMessageProperty _responseProperties;

		public AzureServiceBusHttpResponse(RequestContext context)
		{
			_context = context;
			_responseProperties = new HttpResponseMessageProperty();
		}

		public override NameValueCollection Headers
		{
			get { return _responseProperties.Headers; }
		}

		public override int StatusCode
		{
			get { return (int)_responseProperties.StatusCode; }
			set { _responseProperties.StatusCode = (HttpStatusCode)value; }
		}

		public override string StatusDescription
		{
			get { return _responseProperties.StatusDescription; }
			set { _responseProperties.StatusDescription = value; }
		}

		public override void ClearHeaders()
		{
		}

		public override void ClearContent()
		{
		}

		public override void AppendHeader(string name, string value)
		{
			Headers.Add(name, value);
		}

		public override bool BufferOutput
		{
			get { return false; }
			set { ; }
		}

		public Message Message
		{
			get { return _response; }
			set
			{
				_response = value;

				_response.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
				_response.Properties.Add(HttpResponseMessageProperty.Name, _responseProperties);
			}
		}
	}

	public class AzureServiceBusHttpRequest : HttpRequestBase
	{
		private RequestContext _context;
		private Message _request;
		private HttpRequestMessageProperty _requestProperties;
		private NameValueCollection _serverVariables;

		public AzureServiceBusHttpRequest(RequestContext context)
		{
			_context = context;
			_request = _context.RequestMessage;
			_requestProperties = _context.RequestMessage.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
			_serverVariables = new NameValueCollection();

			Uri via = _request.Properties.Via;
			_serverVariables.Add("SERVER_NAME", via.Host);
			_serverVariables.Add("SERVER_PORT", via.Port.ToString());
			_serverVariables.Add("SERVER_PROTOCOL", via.Scheme);
		}

		public override NameValueCollection Headers
		{
			get { return _requestProperties.Headers; }
		}

		public override string HttpMethod
		{
			get { return _requestProperties.Method; }
		}

		public override string UserAgent
		{
			get { return Headers["User-Agent"]; }
		}

		public override int ContentLength
		{
			get
			{
				var slength = Headers["Content-Length"];
				int length;
				if (Int32.TryParse(slength, out length))
					return length;

				return 0;
			}
		}

		public override string ContentType
		{
			get { return Headers["Content-Type"]; }
			set { throw new NotSupportedException(); }
		}

		public override string[] AcceptTypes
		{
			get { return (Headers["Accept"] ?? "").Split(','); }
		}

		public override Uri UrlReferrer
		{
			get { return new Uri(Headers["Referer"]); }
		}

		public override string UserHostAddress
		{
			get { return "127.0.0.1" /* TODO: find out how to get requesters IP */; }
		}

		public override NameValueCollection ServerVariables
		{
			get { return _serverVariables; }
		}

		public Message Message
		{
			get { return _request; }
		}
	}
}
