using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

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
		public IEnumerable<string> Extensions
		{
			get
			{
				return SettingsRepo.Extensions;
			}
			set
			{
				SettingsRepo.Extensions = value;
				SettingsRepo.SaveChanges();
			}
		}
	}
}
