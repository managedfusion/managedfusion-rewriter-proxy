using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace ManagedFusion.Rewriter
{
	[RunInstaller(true)]
	public partial class WebBridgeInstaller : System.Configuration.Install.Installer
	{
		public WebBridgeInstaller()
		{
			InitializeComponent();
		}
	}
}
