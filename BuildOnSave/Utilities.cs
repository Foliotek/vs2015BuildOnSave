using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
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
			return GetServiceProvider(dte);
		}
		public static ServiceProvider GetServiceProvider(DTE2 dte)
		{
			return new ServiceProvider((IServiceProvider)dte);
		}
	}
}
