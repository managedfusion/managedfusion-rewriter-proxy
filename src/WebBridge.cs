using System;
using System.Linq;
using System.ServiceProcess;
using ManagedFusion.Rewriter.Azure;
using ManagedFusion.Rewriter.Engines;

namespace ManagedFusion.Rewriter
{
	partial class WebBridge : ServiceBase
	{
		private ApacheProxyEngine _host;

		public WebBridge()
		{
			_host = new ApacheProxyEngine();

			ServiceName = "ManagedFusionWebBridge";
		}

		protected override void OnStart(string[] args)
		{
			_host.InitAndOpen();
		}

		protected override void OnStop()
		{
			_host.Close();
		}
	}
}
