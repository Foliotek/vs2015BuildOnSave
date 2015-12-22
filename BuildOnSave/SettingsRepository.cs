using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildOnSave
{
	public class Settings
	{
		private const string SettingsRootPath = @"BuildOnSave\Settings\";
		private const string ExtensionsPropName = @"Extensions";
		private readonly string[] DefaultExtensions = { "cs", "config" };

		public Settings(ServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			InitializeSettings();
		}

		private ServiceProvider _serviceProvider { get; set; }
		private IVsWritableSettingsStore _settingsStore = null;
		private IVsWritableSettingsStore SettingsStore
		{
			get
			{
				if (_settingsStore == null)
				{
					IVsSettingsManager settingsManager = _serviceProvider.GetService(typeof(SVsSettingsManager)) as IVsSettingsManager;
					settingsManager.GetWritableSettingsStore((uint)__VsSettingsScope.SettingsScope_UserSettings, out _settingsStore);
				}
				return _settingsStore;
			}
		}

		private void InitializeSettings()
		{
			// Ensure root collection exists
			if (!CollectionExists(SettingsRootPath))
				SettingsStore.CreateCollection(SettingsRootPath);

			// Ensure default extensions
			EnsureDefaultExtensions();
		}
		private void EnsureDefaultExtensions()
		{
			// If property doesn't exist, set default extensions
			if (!PropertyExists(SettingsRootPath, ExtensionsPropName))
				SetStringArray(ExtensionsPropName, DefaultExtensions);

			// If property does exist, ensure all defaults are present
			else
			{
				var currExtensions = GetStringArray(ExtensionsPropName).ToList();
				var currDefExts = currExtensions.Where(e => DefaultExtensions.Contains(e)).ToList();

				if (currExtensions.Count != DefaultExtensions.Length)
				{
					currExtensions = currExtensions.Where(e => !DefaultExtensions.Contains(e)).ToList();
					currExtensions.AddRange(DefaultExtensions);
				}
			}
		}

		#region Public Methods
		public bool CollectionExists(string path)
		{
			int exists = 0;
			SettingsStore.CollectionExists(path, out exists);

			return ExistenceIntToBool(exists);
		}

		public bool PropertyExists(string path, string key)
		{
			int exists = 0;
			SettingsStore.PropertyExists(path, key, out exists);

			return ExistenceIntToBool(exists);
		}

		public void SetString(string key, string value)
		{
			SettingsStore.SetString(SettingsRootPath, key, value);
		}

		public void SetStringArray(string key, string[] value)
		{
			// Flatten string into form 'extension1,extension2,extension3'
			var sb = new StringBuilder();
			foreach (var item in value)
			{
				sb.Append($"{item.Trim()},");
			}
			var flatStr = sb.ToString();
			flatStr = flatStr.Remove(flatStr.LastIndexOf(','));

			// Set value
			SetString(key, flatStr);
		}

		public string GetString(string key)
		{
			string value;
			SettingsStore.GetStringOrDefault(SettingsRootPath, key, "", out value);
			return value;
		}

		public string[] GetStringArray(string key)
		{
			string val = GetString(key);
			return val.Split(',').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
		}

		private bool ExistenceIntToBool(int exists)
		{
			if (exists == 1)
				return true;
			else
				return false;
		}
		#endregion
	}
}
