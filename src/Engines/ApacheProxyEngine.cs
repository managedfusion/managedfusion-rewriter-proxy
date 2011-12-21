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
using Microsoft.ServiceBus;

namespace ManagedFusion.Rewriter.Engines
{
	/// <summary>
	/// 
	/// </summary>
	public class ApacheProxyEngine
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApacheEngine"/> class.
		/// </summary>
		public ApacheProxyEngine()
		{
			// set file name
			FileName = Manager.Configuration.Azure.DefaultFileName;

			// set physical application path
			PhysicalApplicationPath = Manager.Configuration.Azure.DefaultPhysicalApplicationPath;

			// if there is no physical application path set then get it from the request
			if (String.IsNullOrEmpty(PhysicalApplicationPath))
				PhysicalApplicationPath = Environment.CurrentDirectory;
		}

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		private string FileName { get; set; }

		/// <summary>
		/// Gets or sets the physical application path.
		/// </summary>
		/// <value>The physical application path.</value>
		private string PhysicalApplicationPath { get; set; }

		private FileWatcher _watcher;
		private ApacheProxyRuleSet _ruleSet;
		private List<AzureProxyHandler> _proxies;

		public void InitAndOpen()
		{
			Init();
			Open();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Open()
		{
			foreach (var proxy in _proxies)
				proxy.Open();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Close()
		{
			foreach (var proxy in _proxies)
				proxy.Close();
		}

		#region Scan Directory For Rules

		/// <summary>
		/// Scans the directories for rules.
		/// </summary>
		/// <param name="refreshDir">The refresh dir.</param>
		private void ScanDirectoriesForRules(DirectoryInfo refreshDir)
		{
			if (refreshDir == null)
				refreshDir = new DirectoryInfo(PhysicalApplicationPath);

			FileInfo file = new FileInfo(Path.Combine(refreshDir.FullName, FileName));
			if (file.Exists)
			{
				_ruleSet = new ApacheProxyRuleSet(PhysicalApplicationPath, file);
				AddRuleSetMonitoring(file.FullName);
			}
		}

		#endregion

		#region RuleSet Expiration

		/// <summary>
		/// Rules the set expired.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="reason">The reason.</param>
		private void RuleSetExpired(string fullPath)
		{
			RefreshRules();
		}

		/// <summary>
		/// Adds the rule set monitoring.
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <param name="fullPath">The full path.</param>
		protected void AddRuleSetMonitoring(string fullPath)
		{
			_watcher = new FileWatcher(fullPath, RuleSetExpired);
		}

		#endregion

		#region IRewriterEngine Members

		/// <summary>
		/// Scans the directories for rules.
		/// </summary>
		public virtual void Init()
		{
			_proxies = new List<AzureProxyHandler>();

			ScanDirectoriesForRules(null);
			RefreshRules();
		}

		/// <summary>
		/// Refreshes the rules.
		/// </summary>
		public virtual void RefreshRules()
		{
			Close();

			_proxies.Clear();
			_ruleSet.RefreshRules();

			foreach (var rule in _ruleSet.Rules)
			{
				Uri responseUrl = ServiceBusEnvironment.CreateServiceUri("http", Manager.Configuration.Azure.ServiceNamespace, "");
				Uri requestUrl = rule.DownstreamUri;

				_proxies.Add(new AzureProxyHandler(requestUrl, responseUrl));
			}
		}

		#endregion
	}
}