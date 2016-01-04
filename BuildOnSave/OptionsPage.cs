using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Linq;

namespace BuildOnSave
{
	public class OptionsPage : DialogPage
	{
		private const string CategoryName = "Build On Save";

		#region Public Settings
		[Category(CategoryName)]
		[DisplayName("Build Entire Solution")]
		[Description("Determines whether the entire solution should be built or just the project containing the modified document")]
		public bool BuildEntireSolution
		{
			get
			{
				return Settings.BuildEntireSolution;
			}
			set
			{
				Settings.BuildEntireSolution = value;
			}
		}

		[Category(CategoryName)]
		[DisplayName("Enabled")]
		[Description("Determines whether a build should triggered upon document save")]
		public bool Enabled
		{
			get
			{
				return Settings.AutoBuildEnabled;
			}
			set
			{
				Settings.AutoBuildEnabled = value;
			}
		}

		[Category(CategoryName)]
		[DisplayName("Extensions")]
		[Description("Document extensions which trigger a build upon saving a document")]
		public string[] Extensions
		{
			get
			{
				return Settings.Extensions.ToArray();
			}
			set
			{
				Settings.Extensions = value;
			}
		}
		#endregion

	}
}
