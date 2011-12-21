using System;
using System.Configuration;
using System.ServiceProcess;
using ManagedFusion.Rewriter.Azure;
using ManagedFusion.Rewriter.Configuration;
using ManagedFusion.Rewriter.Engines;

namespace ManagedFusion.Rewriter
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main(string[] args)
		{
			bool runOnConsole = false;
			bool showHelp = false;

			try
			{

				if (String.IsNullOrEmpty(Manager.Configuration.Azure.IssuerName))
				{
					Console.WriteLine("Configuration attribute 'issuerName' is not set or empty.");
					return;
				}
				else if (String.IsNullOrEmpty(Manager.Configuration.Azure.IssuerSecret))
				{
					Console.WriteLine("Configuration attribute 'issuerSecret' is not set or empty.");
					return;
				}
			}
			catch (ConfigurationErrorsException ce)
			{
				Console.WriteLine("Configuration exception: {0}", ce.Message);
				return;
			}

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i][0] == '-' || args[i][0] == '/')
				{
					switch (args[i].Substring(1).ToUpperInvariant())
					{
						case "C":
						case "CONSOLE":
							runOnConsole = true;
							break;

						case "?":
						case "H":
						case "HELP":
							showHelp = true;
							break;
					}
				}
			}

			// only run in OS Service mode if registered
			if (!runOnConsole)
			{
				ServiceController sc = new ServiceController("WebBridge");
				try
				{
					var status = sc.Status;
					Console.WriteLine("Status: " + status);
				}
				catch (SystemException)
				{
					runOnConsole = true;
				}
			}

			if (showHelp)
			{
				Console.WriteLine("Managed Fusion Web Bridge");
				Console.WriteLine("  via Windows Azure AppFabric Service Bus");
				Console.WriteLine();
				Console.WriteLine("WebBridge.exe [/console] [/help]");
				Console.WriteLine("  /console  Run service in console window. Short /c");
				Console.WriteLine("  /help     Show help. Short /?");
			}
			else if (runOnConsole)
			{
				var host = new ApacheProxyEngine();
				host.InitAndOpen();

				Console.WriteLine("Managed Fusion Web Bridge is running.");
				Console.WriteLine("Press [Enter] to exit");
				Console.ReadLine();

				host.Close();
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new WebBridge() 
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}