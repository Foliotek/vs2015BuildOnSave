using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace BuildOnSave
{
	public static class Utilities
	{
		public static DTE2 GetDTE()
		{
			return Package.GetGlobalService(typeof(DTE)) as DTE2;
		}

		public static ServiceProvider GetServiceProvider()
		{
			var dte = GetDTE();
			return new ServiceProvider((IServiceProvider)dte);
		}

		public static IVsStatusbar GetStatusBar()
		{
			return Package.GetGlobalService(typeof(SVsStatusbar)) as IVsStatusbar;
		}
	}
}
