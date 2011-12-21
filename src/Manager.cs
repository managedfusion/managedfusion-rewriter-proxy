/** 
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
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ManagedFusion.Rewriter.Configuration;

namespace ManagedFusion.Rewriter
{
	/// <summary>
	/// 
	/// </summary>
	public static class Manager
	{
		private static object _logLock = new object();
		private static string _logPath;

		public static readonly RegexOptions RuleOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;

		/// <summary>
		/// The rewriter name.
		/// </summary>
		public const string RewriterName = "ManagedFusion";

		/// <summary>
		/// The rewriter user agent name.
		/// </summary>
		public const string RewriterUserAgent = "ManagedFusion (rewriter; reverse-proxy; +http://managedfusion.com/)";

		/// <summary>
		/// The name of the stoarge location in HttpContext.Items for the proxy handler.
		/// </summary>
		public const string ProxyHandlerStorageName = "ManagedFusion.Rewriter.ProxyHandler";

		/// <summary>
		/// Gets or sets the rewriter version.
		/// </summary>
		/// <value>The rewriter version.</value>
		public static Version RewriterVersion
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the rewriter name and version.
		/// </summary>
		/// <value>The rewriter name and version.</value>
		public static string RewriterNameAndVersion
		{
			get { return RewriterName + "/" + RewriterVersion.ToString(2); }
		}

		private static ManagedFusionRewriterSectionGroup _configuration;

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public static ManagedFusionRewriterSectionGroup Configuration
		{
			get
			{
				if (_configuration == null)
				{
					try { _configuration = ManagedFusionRewriterSectionGroup.Instance; }
					finally
					{
						if (_configuration == null)
							_configuration = new ManagedFusionRewriterSectionGroup();
					}
				}

				return _configuration;
			}
		}

		#region Logging

		/// <summary>
		/// Gets or sets a value indicating if logging is enabled.
		/// </summary>
		public static bool LogEnabled { get; set; }

		/// <summary>
		/// Gets or sets the log path.
		/// </summary>
		public static string LogPath
		{
			get { return _logPath; }
			set
			{
				if (String.IsNullOrEmpty(value))
				{
					_logPath = null;
					return;
				}

				try
				{
					StreamWriter sw = File.Exists(value) ? File.AppendText(value) : File.CreateText(value);
					sw.WriteLine("**********************************************************************************");
					sw.WriteLine("Logging started on " + DateTime.Now.ToString("s"));
					sw.WriteLine("**********************************************************************************");

					sw.Flush();
					sw.Close();
					sw.Dispose();

					_logPath = value;
				}
				catch (Exception exc)
				{
					throw new RewriterException("Problem with log file " + value, exc);
				}
			}
		}

		/// <summary>
		/// Log the message and category if the condition is met.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="messaage"></param>
		/// <param name="category"></param>
		public static void LogIf(bool condition, string messaage, string category)
		{
			if (condition)
				Log(messaage, category);
		}

		/// <summary>
		/// Log the message if the condition is met.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		public static void LogIf(bool condition, string message)
		{
			if (condition)
				Log(message, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public static void Log(string message)
		{
			Log(message, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		public static void Log(string message, string category)
		{
			if (!LogEnabled)
				return;

			if (_logPath == null)
				return;

			StringBuilder sb = new StringBuilder();
			sb.Append(DateTime.Now.ToString("s"));
			sb.Append(" ");

			if (category != null)
			{
				sb.Append("[");
				sb.Append(category);
				sb.Append("] ");
			}

			sb.Append(message);
			sb.Append(Environment.NewLine);

			lock (_logLock)
			{
				File.AppendAllText(_logPath, sb.ToString(), Encoding.UTF8);
			}

			if (Configuration.Rewriter.TraceLog)
				Trace.WriteLine(message, category);
		}

		#endregion

		#region Headers and Server Variables

		/// <summary>
		/// Adds the response header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		internal static void AddResponseHeader(HttpContextBase context, string name, string value)
		{
			context.Response.AppendHeader(name, value);
		}

		/// <summary>
		/// Trys to add the response header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <remarks>This was added to protect against the <c>Server cannot append header after HTTP headers have been sent.</c> exception that occurs occationally 
		/// when a module before the rewriter preforms a <see cref="HttpResponse.Flush"/> forcing all the headers to be written before other modules have had a chance.</remarks>
		/// <returns></returns>
		internal static bool TryAddResponseHeader(HttpContextBase context, string name, string value)
		{
			try
			{
				AddResponseHeader(context, name, value);
				return true;
			}
			catch (Exception exc)
			{
				Manager.Log(exc.Message, "Error");
				return false;
			}
		}

		/// <summary>
		/// Adds the request header.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="name">The header name.</param>
		/// <param name="value">The value.</param>
		internal static void AddRequestHeader(HttpContextBase context, string name, string value)
		{
			// also store header in context
			context.Items[name] = value;

			if (Configuration.Rewriter.AllowRequestHeaderAdding && context.Request.Headers[name] == null)
			{
				try
				{
					if (HttpRuntime.UsingIntegratedPipeline)
					{
						context.Request.Headers.Add(name, value);
					}
					else
					{
						Type targetType = typeof(NameValueCollection);

						// get the property for setting readability
						PropertyInfo isReadOnlyProperty = targetType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

						// set headers as read and write
						isReadOnlyProperty.SetValue(context.Request.Headers, false, null);

						ArrayList list = new ArrayList();
						list.Add(value);

						// get the method to fill in the headers
						MethodInfo fillInHeadersCollectionMethod = targetType.GetMethod("BaseSet", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);
						fillInHeadersCollectionMethod.Invoke(context.Request.Headers, new object[] { name, list });

						// set headers as read only
						isReadOnlyProperty.SetValue(context.Request.Headers, true, null);
					}
				}
				catch (SecurityException) { }
				catch (MethodAccessException) { }
				catch (NullReferenceException) { }
			}
		}

		/// <summary>
		/// Tries to add X rewrite URL header.
		/// </summary>
		/// <param name="context">The context.</param>
		internal static void TryToAddXRewriteUrlHeader(HttpContextBase context)
		{
			// add the X-Rewrite-Url to the context items because it is needed
			// for other services offered by this rewriter, it just won't be 
			// available through the header for this request
			context.Items["X-Rewrite-Url"] = context.Request.RawUrl;

			// add the original url to the header
			if (Configuration.Rewriter.AllowXRewriteUrlHeader)
				AddRequestHeader(context, "X-Rewrite-Url", HttpUtility.UrlPathEncode(context.Request.RawUrl));
		}

		/// <summary>
		/// Tries to add vanity header.
		/// </summary>
		/// <param name="context">The context.</param>
		internal static void TryToAddVanityHeader(HttpContextBase context)
		{
			// add the vanity url to the header
			if (Configuration.Rewriter.AllowVanityHeader)
			{
				TryAddResponseHeader(context, "X-Rewritten-By", RewriterUserAgent);
				TryAddResponseHeader(context, "X-ManagedFusion-Rewriter-Version", RewriterVersion.ToString(2));
			}
		}

		#endregion

		/// <summary>
		/// Initializes the <see cref="Manager"/> class.
		/// </summary>
		static Manager()
		{
			// gets the assembly version of the rewriter
			RewriterVersion = typeof(Manager).Assembly.GetName().Version.Clone() as Version;
		}
	}
}