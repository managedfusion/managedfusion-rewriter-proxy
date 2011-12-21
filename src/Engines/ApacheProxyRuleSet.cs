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
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using ManagedFusion.Rewriter.Azure;

namespace ManagedFusion.Rewriter.Engines
{
	public class ApacheProxyRuleSet
	{
		private const string DefaultLogFileName = "url-rewrite-log.txt";

		private static readonly RegexOptions FileOptions = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant;

		/// <summary>Rewrite Log Level</summary>
		/// <remarks>
		/// 0 : Nothing is logged.
		/// 1 : Provides simple input and output.
		/// 2 : Provides 1 and conditions and rewrites input and ouput.
		/// 3 : Provides 2 and processing rules from rules file.
		/// 4 : Provides 3 and ignored lines.
		/// 5 : Provides 4 and shows configuration errors
		/// ...
		/// 9 : Provides 8 and is very verbose.
		/// </remarks>
		private static readonly Regex ProxyLogLevelLine = new Regex(@"^RewriteLogLevel[\s]+(?<level>[0-9])", FileOptions);
		private static readonly Regex ProxyLogLine = new Regex(@"^RewriteLog[\s]+""?(?<location>[^""]+)""?", FileOptions);
		private static readonly Regex ProxyRequestsLine = new Regex(@"^ProxyRequests[\s]+(?i:(?<state>on|off))", FileOptions);
		private static readonly Regex ProxyPassLine = new Regex(@"^ProxyPass[\s]+(?<path>[\S]+)[\s]+(?<url>[\S]+)[\s]*", FileOptions);

		private readonly FileInfo _ruleSetFile;
		private object _refreshLock;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApacheProxyRuleSet"/> class.
		/// </summary>
		/// <param name="physicalBase">The physical base.</param>
		/// <param name="ruleSetFile">The rule set file.</param>
		public ApacheProxyRuleSet(string physicalBase, FileInfo ruleSetFile)
		{
			Rules = new List<ProxyRule>();
			PhysicalBase = physicalBase;

			_ruleSetFile = ruleSetFile;
			_refreshLock = new Object();
		}

		public List<ProxyRule> Rules { get; set; }
		public string PhysicalBase { get; set; }
		public bool EngineEnabled { get; set; }
		public int LogLevel { get; set; }
		public string LogLocation { get; set; }

		#region Refresh Actions

		/// <summary>
		/// Normalizes the log location.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		protected string NormalizeLogLocation(string path)
		{
			if (String.IsNullOrEmpty(path))
				throw new ArgumentNullException("path");

			string pathRoot = Path.GetPathRoot(path);

			// if the path root is empty then add a root to it
			if (String.IsNullOrEmpty(pathRoot))
			{
				pathRoot = Path.DirectorySeparatorChar.ToString();
				path = pathRoot + path;
			}

			// if the log path is not rooted to the drive then we need to path the path according to 
			// our web application root directory
			if (pathRoot == Path.DirectorySeparatorChar.ToString())
				path = Path.Combine(Manager.Configuration.Azure.DefaultPhysicalApplicationPath, path);

			// if the path is a directory then add our default filename to it
			if (String.IsNullOrEmpty(Path.GetFileName(path)))
				path = Path.Combine(path, DefaultLogFileName);

			// normalize the path in case anything wacky is in there
			path = Path.GetFullPath(path);

			// verify write access is granted to the path
			new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();

			return path;
		}

		#endregion

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		public void RefreshRules()
		{
			using (StreamReader reader = _ruleSetFile.OpenText())
			{
				RefreshRules(reader);
			}
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public void RefreshRules(TextReader reader)
		{
			// put a lock on the refresh process so that only one refresh can happen at a time
			lock (_refreshLock)
			{
				Manager.LogEnabled = false;
				Manager.LogPath = null;

				string tempLogPath = null;
				int tempLogLevel = 0;
				bool tempEngineEnabled = false;

				string line;
				IList<ProxyRule> rules = new List<ProxyRule>();
				IList<string> unknownLines = new List<string>();

				while (reader.Peek() >= 0)
				{
					line = reader.ReadLine().Trim();

					if (String.IsNullOrEmpty(line))
					{
						// just plain old ignore empty lines no logging or anything
						continue;
					}
					else if (line[0] == '#')
					{
						Manager.LogIf(tempLogLevel >= 4, "Comment: " + line, "Rule Processing");
					}
					else if (ProxyRequestsLine.IsMatch(line))
					{
						#region ProxyRequests

						Match match = ProxyRequestsLine.Match(line);
						string engineState = match.Groups["state"].Value;

						// by default the engine is turned off
						if (String.IsNullOrEmpty(engineState) || String.Equals(engineState, "off", StringComparison.OrdinalIgnoreCase))
						{
							rules.Clear();
							tempEngineEnabled = false;

							// don't bother processing any other rules if the engine is disabled
							break;
						}
						else
						{
							tempEngineEnabled = true;
						}

						Manager.LogIf(tempLogLevel >= 3, "ProxyRequests: " + (tempEngineEnabled ? "Enabled" : "Disabled"), "Rule Processing");

						#endregion
					}
					else if (ProxyLogLine.IsMatch(line))
					{
						#region ProxyLog

						Match match = ProxyLogLine.Match(line);
						tempLogPath = match.Groups["location"].Value;
						tempLogPath = NormalizeLogLocation(tempLogPath);

						Manager.LogIf(tempLogLevel >= 3, "ProxyLog: " + tempLogPath, "Rule Processing");

						#endregion
					}
					else if (ProxyLogLevelLine.IsMatch(line))
					{
						#region ProxyLogLevel

						Match match = ProxyLogLevelLine.Match(line);
						int logLevel = 1;

						if (!Int32.TryParse(match.Groups["level"].Value, out logLevel))
						{
							tempLogLevel = 0;
							Manager.LogIf(tempLogLevel >= 3, "ProxyLogLevel: " + match.Groups["level"].Value + " not understood.", "Rule Processing");
						}
						else
						{
							tempLogLevel = logLevel;
						}

						Manager.LogIf(tempLogLevel >= 3, "ProxyLogLevel: " + logLevel, "Rule Processing");

						#endregion
					}
					else if (ProxyPassLine.IsMatch(line))
					{
						#region ProxyPass

						Match match = ProxyPassLine.Match(line);

						try
						{
							// initialize the proxy
							var rule = new ProxyRule {
								Path = match.Groups["path"].Value,
								DownstreamUri = new Uri(match.Groups["url"].Value)
							};

							// add condition to next rule that shows up
							rules.Add(rule);
						}
						catch (Exception exc)
						{
							if (tempLogLevel >= 3)
								Manager.Log("ProxyPass: " + exc.Message, "Error");
							else
								Manager.Log("ProxyPass: " + exc, "Error");
						}
						finally
						{
							Manager.LogIf(tempLogLevel >= 3, "ProxyPass: " + match.Groups["pattern"].Value + " " + match.Groups["substitution"].Value + " [" + match.Groups["flags"].Value + "]", "Rule Processing");
						}

						#endregion
					}
					else
					{
						unknownLines.Add(line);
					}
				}

				Manager.LogIf(tempLogLevel > 0, "Managed Fusion Rewriter Version: " + Manager.RewriterVersion, "Rule Processing");

				// clear and add new rules
				ClearRules();
				AddRules(rules);

				// try to process any unknown lines
				if (unknownLines.Count > 0)
				{
					RefreshUnknownLines(ref unknownLines);

					foreach (var unknownLine in unknownLines)
						Manager.LogIf(tempLogLevel >= 4, "Not Understood: " + unknownLine, "Unknown");
				}


				// set the ruleset defining properties
				LogLocation = tempLogPath;
				LogLevel = tempLogLevel;
				EngineEnabled = tempEngineEnabled;
				Manager.LogPath = tempLogPath;
				Manager.LogEnabled = tempLogLevel > 0;
			}
		}

		private void AddRules(IList<ProxyRule> rules)
		{
			Rules.AddRange(rules);
		}

		private void ClearRules()
		{
			Rules.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lines"></param>
		/// <returns></returns>
		protected virtual void RefreshUnknownLines(ref IList<string> lines)
		{
		}
	}
}