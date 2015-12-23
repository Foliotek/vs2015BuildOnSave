using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Linq;

namespace BuildOnSave
{
	public class OptionsPage : DialogPage
	{
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

		[Category("Build On Save")]
		[DisplayName("Extensions")]
		[Description("Extensions which trigger a build upon saving")]
		public string[] Extensions
		{
			get
			{
				return SettingsRepo.Extensions.ToArray();
			}
			set
			{
				SettingsRepo.Extensions = value;
				SettingsRepo.SaveChanges();
			}
		}
	}
}
