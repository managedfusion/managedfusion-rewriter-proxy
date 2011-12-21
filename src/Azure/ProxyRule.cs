using System;
using System.Linq;

namespace ManagedFusion.Rewriter.Azure
{
	public class ProxyRule
	{
		public string Path;
		public Uri DownstreamUri;
	}
}
