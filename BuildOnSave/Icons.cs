using Microsoft.VisualStudio.Shell.Interop;

namespace BuildOnSave
{
	public static class Icons
	{
		/// <summary>
		///	Standard animation icon.
		/// </summary>
		public static object General => Constants.SBAI_General;
		/// <summary>
		///	Animation when printing.
		/// </summary>
		public static object Print => Constants.SBAI_Print;
		/// <summary>
		/// Animation when saving files.
		/// </summary>
		public static object Save => Constants.SBAI_Save;
		/// <summary>
		/// Animation when deploying the solution.
		/// </summary>
		public static object Deploy => Constants.SBAI_Deploy;
		/// <summary>
		/// Animation when synchronizing files over the network.
		/// </summary>
		public static object Sync => Constants.SBAI_Synch;
		/// <summary>
		/// Animation when building the solution.
		/// </summary>
		public static object Build => Constants.SBAI_Build;
		/// <summary>
		/// Animation when searching.
		/// </summary>
		public static object Find => Constants.SBAI_Find;
	}
}
