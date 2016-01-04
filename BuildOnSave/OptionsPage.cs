using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Linq;

namespace BuildOnSave
{
	public class OptionsPage : DialogPage
	{
		private const string CategoryName = "Build On Save";

		private SettingsRepository _settingsRepo;
		private SettingsRepository SettingsRepo
		{
			get
			{
				if (_settingsRepo == null)
					_settingsRepo = new SettingsRepository();
				return _settingsRepo;
			}
		}

		#region Public Settings
		[Category(CategoryName)]
		[DisplayName("Enabled")]
		[Description("Automatically build solution on document save.  Visual Studio must be restarted for this change to take effect.")]
		public bool Enabled
		{
			get
			{
				return SettingsRepo.AutoBuildEnabled;
			}
			set
			{
				SettingsRepo.AutoBuildEnabled = value;
				SettingsRepo.SaveAutoBuildEnabled();
			}
		}

		[Category(CategoryName)]
		[DisplayName("Extensions")]
		[Description("Extensions which trigger a build upon saving.  Visual Studio must be restarted for this change to take effect.")]
		public string[] Extensions
		{
			get
			{
				return SettingsRepo.Extensions.ToArray();
			}
			set
			{
				SettingsRepo.Extensions = value;
				SettingsRepo.SaveExtensions();
			}
		}
		#endregion

	}
}
