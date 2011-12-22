﻿/** 
 * Copyright (C) 2007-2010 Nicholas Berardi, Managed Fusion, LLC (nick@managedfusion.com)
 * 
 * <author>Nicholas Berardi</author>
 * <author_email>nick@managedfusion.com</author_email>
 * <company>Managed Fusion, LLC</company>
 * <product>Url Rewriter and Reverse Proxy</product>
 * <license>Microsoft Public License (Ms-PL)</license>
 * <agreement>
 * This software, as defined above in <product />, is copyrighted by the <author /> and the <company />, all defined above.
 * 
 * For all binary distributions the <product /> is licensed for use under <license />.
 * For all source distributions please contact the <author /> at <author_email /> for a commercial license.
 * 
 * This copyright notice may not be removed and if this <product /> or any parts of it are used any other
 * packaged software, attribution needs to be given to the author, <author />.  This can be in the form of a textual
 * message at program startup or in documentation (online or textual) provided with the packaged software.
 * </agreement>
 * <product_url>http://www.managedfusion.com/products/url-rewriter/</product_url>
 * <license_url>http://www.managedfusion.com/products/url-rewriter/license.aspx</license_url>
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public class ProxyHandler : IHttpProxyHandler, IHttpHandler
	{
		#region IHttpProxyHandler Members

		/// <summary>
		/// Inits the specified request for the proxy.
		/// </summary>
		/// <param name="requestUrl">The request URL.</param>
		/// <param name="responseUrl">The response URL.</param>
		public void Init(Uri requestUrl, Uri responseUrl)
		{
			RequestUrl = requestUrl;
			ResponseUrl = responseUrl;
		}

		/// <summary>
		/// Gets or sets the requested URL.
		/// </summary>
		/// <value>The requested URL.</value>
		public Uri RequestUrl { get; private set; }

		/// <summary>
		/// Gets or sets the response URL.
		/// </summary>
		/// <value>The response URL.</value>
		public Uri ResponseUrl { get; private set; }

		#endregion

		#region IHttpHandler Members

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		public bool IsReusable
		{
			get { return false; }
		}

		/// <summary>
		/// Sends the request to server.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private WebResponse SendRequestToTarget(HttpContextBase context)
		{
			// get the request
			var request = WebRequest.CreateDefault(RequestUrl);

			if (request == null)
				throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("The requested url, <{0}>, could not be found.", RequestUrl));

			// keep the same HTTP request method
			request.Method = context.Request.HttpMethod;
			var knownVerb = KnownHttpVerb.Parse(request.Method);

			// depending on the type of this request specific values for an HTTP request
			if (request is HttpWebRequest)
			{
				var httpRequest = request as HttpWebRequest;
				httpRequest.AllowAutoRedirect = false;
				httpRequest.ServicePoint.Expect100Continue = false;

				// add all the headers from the other proxied session to this request
				foreach (string name in context.Request.Headers.AllKeys)
				{
					// add the headers that are restricted in their supported manor
					switch (name)
					{
						case "User-Agent":
							httpRequest.UserAgent = context.Request.UserAgent;
							break;

						case "Connection":
							string connection = context.Request.Headers[name];
							if (connection.IndexOf("Keep-Alive", StringComparison.OrdinalIgnoreCase) > 0)
								httpRequest.KeepAlive = true;

							var list = new List<string>();
							foreach (string conn in connection.Split(','))
							{
								string c = conn.Trim();
								if (!c.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase) && !c.Equals("Close", StringComparison.OrdinalIgnoreCase))
									list.Add(c);
							}

							if (list.Count > 0)
								httpRequest.Connection = String.Join(", ", list.ToArray());
							break;

						case "Transfer-Encoding":
							httpRequest.SendChunked = true;
							httpRequest.TransferEncoding = context.Request.Headers[name];
							break;

						case "Expect":
							httpRequest.ServicePoint.Expect100Continue = true;
							break;

						case "If-Modified-Since":
							DateTime ifModifiedSince;
							if (DateTime.TryParse(context.Request.Headers[name], out ifModifiedSince))
								httpRequest.IfModifiedSince = ifModifiedSince;
							break;

						case "Content-Length":
							httpRequest.ContentLength = context.Request.ContentLength;
							break;

						case "Content-Type":
							httpRequest.ContentType = context.Request.ContentType;
							break;

						case "Accept":
							httpRequest.Accept = String.Join(", ", context.Request.AcceptTypes);
							break;

						case "Referer":
							httpRequest.Referer = context.Request.UrlReferrer.OriginalString;
							break;
					}

					// add to header if not restricted
					if (!WebHeaderCollection.IsRestricted(name, false))
					{
						// it is nessisary to get the values for headers that are allowed to specifiy
						// multiple values in an instance (i.e. Cookie)
						string[] values = context.Request.Headers.GetValues(name);
						foreach (string value in values)
							request.Headers.Add(name, value);
					}
				}
			}

			// add the vanity url to the header
			if (Manager.Configuration.Rewriter.AllowVanityHeader)
			{
				request.Headers.Add("X-Reverse-Proxied-By", Manager.RewriterUserAgent);
				request.Headers.Add("X-ManagedFusion-Rewriter-Version", Manager.RewriterVersion.ToString(2));
			}

			/*
			 * Add Proxy Standard Protocol Headers
			 */

			// http://en.wikipedia.org/wiki/X-Forwarded-For
			request.Headers.Add("X-Forwarded-For", context.Request.UserHostAddress);

			// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.45
			string currentServerName = context.Request.ServerVariables["SERVER_NAME"];
			string currentServerPort = context.Request.ServerVariables["SERVER_PORT"];
			string currentServerProtocol = context.Request.ServerVariables["SERVER_PROTOCOL"];

			if (currentServerProtocol.IndexOf("/") >= 0)
				currentServerProtocol = currentServerProtocol.Substring(currentServerProtocol.IndexOf("/") + 1);

			string currentVia = String.Format("{0} {1}:{2} ({3})", currentServerProtocol, currentServerName, currentServerPort, Manager.RewriterNameAndVersion);

			request.Headers.Add("Via", currentVia);

			/*
			 * End - Add Proxy Standard Protocol Headers
			 */

			OnRequestToTarget(context, request);

			// ContentLength is set to -1 if their is no data to send
			if (request.ContentLength >= 0 && !knownVerb.ContentBodyNotAllowed)
			{
				WriteStreamRequestToTarget(context, request);
			}

			// get the response
			WebResponse response;
			try { response = request.GetResponse(); }
			catch (WebException exc)
			{
				Manager.Log(String.Format("Error received from {0}: {1}", request.RequestUri, exc.Message), "Proxy");
				response = exc.Response;
			}

			if (response == null)
			{
				Manager.Log("No response was received, returning a '400 Bad Request' to the client.", "Proxy");
				throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("The requested url, <{0}>, could not be found.", RequestUrl));
			}

			Manager.Log(response.GetType().ToString(), "Proxy");
			if (response is HttpWebResponse)
			{
				var httpResponse = response as HttpWebResponse;
				Manager.Log(String.Format("Received '{0} {1}'", ((int)httpResponse.StatusCode), httpResponse.StatusDescription), "Proxy");
			}

			return response;
		}

		/// <summary>
		/// Sends the response to client.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="response">The response.</param>
		private void SendResponseToClient(HttpContextBase context, WebResponse response)
		{
			context.Response.ClearHeaders();
			context.Response.ClearContent();

			/* You cannot set any cache headers through the API, because it will prevent the use of the cache headers
			 * added through the AppendHeader method.
			 */

			// add all the headers from the other proxied session to this request
			for (int i = 0; i < response.Headers.Count; i++)
			{
				string name = response.Headers.GetKey(i);

				// don't add any of these headers
				// don't check for restricted response headers because HttpContext doesn't seem to care
				if (name == "Server" ||
					name == "X-Powered-By" ||
					name == "Date" ||
					name == "Host")
					continue;

				string[] values = response.Headers.GetValues(i);
				if (values.Length == 0)
					continue;

				if (name == "Location")
				{
					try
					{
						string location = values[0];

						// make sure location is not empty before creating the URL
						if (!String.IsNullOrEmpty(location))
						{
							// reminder: If location is an absolute URL, the Uri instance is created using only location.
							var requestLocationUrl = new Uri(RequestUrl, location);
							var responseLocationUrl = new UriBuilder(requestLocationUrl);

							// if the requested location for the host and port is the same as the requested URL we need to update them to the response
							if (Uri.Compare(requestLocationUrl, RequestUrl, UriComponents.SchemeAndServer, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
							{
								responseLocationUrl.Port = ResponseUrl.Port;
								responseLocationUrl.Host = ResponseUrl.Host;
								responseLocationUrl.Scheme = ResponseUrl.Scheme;

								string path = responseLocationUrl.Path;
								int pathIndex = path.IndexOf(RequestUrl.AbsolutePath, StringComparison.OrdinalIgnoreCase);

								// since this is not redirecting to a different server try to replease the path
								if (pathIndex > -1)
								{
									path = path.Remove(pathIndex, RequestUrl.AbsolutePath.Length);
									path = path.Insert(pathIndex, ResponseUrl.AbsolutePath);

									responseLocationUrl.Path = path;
								}
							}

							context.Response.AppendHeader(name, responseLocationUrl.Uri.OriginalString);
						}
					}
					catch { /* do nothing on purpose */ }

					// nothing else can occure just continue processing from next header
					continue;
				}

				// if this is a chuncked response then we should send it correctly
				if (name == "Transfer-Encoding")
				{
					/* http://www.go-mono.com/docs/index.aspx?link=P%3aSystem.Web.HttpResponse.Buffer
					 * 
					 * This controls whether HttpResponse should buffer the output before it is delivered to a 
					 * client. The default is true.
					 * 
					 * The buffering can be changed during the execution back and forth if needed. Notice that 
					 * changing the buffering state will not flush the current contents held in the output buffer, 
					 * the contents will only be flushed out on the next write operation or by manually calling 
					 * System.Web.HttpResponse.Flush
					 */
					context.Response.BufferOutput = false;
					continue;
				}

				if (name == "Content-Type")
				{
					/* http://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
					 * http://en.wikipedia.org/wiki/Motion_JPEG#M-JPEG_over_HTTP
					 * 
					 * The multipart/x-mixed-replace content-type should be treated as a streaming content and shouldn't be buffered.
					 */
					if (values[0].StartsWith("multipart/x-mixed-replace"))
						context.Response.BufferOutput = false;
				}

				// it is nessisary to get the values for headers that are allowed to specifiy
				// multiple values in an instance (i.e. Set-Cookie)
				foreach (string value in values)
					context.Response.AppendHeader(name, value);
			}

			Manager.Log(String.Format("Response is {0}being buffered", (context.Response.BufferOutput ? "" : "not ")), "Proxy");

			// add the vanity url to the header
			Manager.TryToAddVanityHeader(context);

			// set all HTTP specific protocol stuff
			if (response is HttpWebResponse)
			{
				var httpResponse = response as HttpWebResponse;
				context.Response.StatusCode = (int)httpResponse.StatusCode;
				context.Response.StatusDescription = httpResponse.StatusDescription;

				Manager.Log(String.Format("Responding '{0} {1}'", ((int)httpResponse.StatusCode), httpResponse.StatusDescription), "Proxy");
			}

			OnResponseToClient(context, response);

			WriteStreamResponseToTarget(context, response);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(new HttpContextWrapper(context));
		}

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		public void ProcessRequest(HttpContextBase context)
		{
			Manager.Log("**********************************************************************************");

			// send the request to the target
			Manager.Log("Request: " + RequestUrl, "Proxy");
			var response = SendRequestToTarget(context);

			// send the response to the client
			Manager.Log("Response: " + ResponseUrl, "Proxy");
			SendResponseToClient(context, response);

			Manager.Log("**********************************************************************************");
		}

		#endregion

		public virtual void WriteStreamResponseToTarget(HttpContextBase context, WebResponse response)
		{
			int bufferSize = Manager.Configuration.Proxy.ResponseSize;
			// push the content out to through the stream
			using (var responseStream = response.GetResponseStream())
			using (var bufferStream = new BufferedStream(responseStream, Manager.Configuration.Proxy.BufferSize))
			{
				byte[] buffer = new byte[bufferSize];

				try
				{
					while (true)
					{
						// make sure that the stream can be read from
						if (!bufferStream.CanRead)
							break;

						int bytesReturned = bufferStream.Read(buffer, 0, bufferSize);

						// if not bytes were returned the end of the stream has been reached
						// and the loop should exit
						if (bytesReturned == 0)
							break;

						// write bytes to the response
						context.Response.OutputStream.Write(buffer, 0, bytesReturned);
					}
				}
				catch (Exception exc)
				{
					Manager.Log("Error on response: " + exc.Message, "Proxy");
				}
			}
		}

		public virtual void WriteStreamRequestToTarget(HttpContextBase context, WebRequest request)
		{
			int bufferSize = Manager.Configuration.Proxy.RequestSize;
			using (Stream requestStream = request.GetRequestStream(), bufferStream = new BufferedStream(context.Request.InputStream, Manager.Configuration.Proxy.BufferSize))
			{
				byte[] buffer = new byte[bufferSize];

				try
				{
					while (true)
					{
						// make sure that the stream can be read from
						if (!bufferStream.CanRead)
							break;

						int bytesReturned = bufferStream.Read(buffer, 0, bufferSize);

						// if not bytes were returned the end of the stream has been reached
						// and the loop should exit
						if (bytesReturned == 0)
							break;

						// write bytes to the response
						requestStream.Write(buffer, 0, bytesReturned);
					}
				}
				catch (Exception exc)
				{
					Manager.Log("Error on request: " + exc.Message, "Proxy");
				}
			}
		}

		/// <summary>
		/// Called when [request to target].
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="request">The request.</param>
		protected virtual void OnRequestToTarget(HttpContextBase context, WebRequest request)
		{
		}

		/// <summary>
		/// Called when [response to client].
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="response">The response.</param>
		protected virtual void OnResponseToClient(HttpContextBase context, WebResponse response)
		{
		}
	}
}