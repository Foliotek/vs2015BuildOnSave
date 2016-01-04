using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildOnSave
{
	public class SettingsRepository
	{
		private const string SettingsRootPath = @"BuildOnSave\Settings\";
		private const string ExtensionsPropName = "Extensions";
		private const string BuildOnSaveEnabledPropName = "AutoBuildEnabled";
		private readonly string[] DefaultExtensions = { "cs", "config" };

		// Constructor
		public SettingsRepository()
		{
			ServiceProvider = Utilities.GetServiceProvider();
			InitializeSettings();
		}

		private ServiceProvider ServiceProvider { get; }
		private IVsWritableSettingsStore _settingsStore;
		private IVsWritableSettingsStore SettingsStore
		{
			get
			{
				// If it's null, initialize it before returning
				if (_settingsStore == null)
				{
					var settingsManager = ServiceProvider.GetService(typeof(SVsSettingsManager)) as IVsSettingsManager;
					settingsManager.GetWritableSettingsStore((uint)__VsSettingsScope.SettingsScope_UserSettings, out _settingsStore);
				}
				return _settingsStore;
			}
		}

		private void InitializeSettings()
		{
			// Ensure root collection exists
			if (!CollectionExists())
				SettingsStore.CreateCollection(SettingsRootPath);

			// Ensure default extensions are set in registry
			EnsureDefaultExtensions();

			// Initialize public properties
			Extensions = GetStringCollection(ExtensionsPropName);
			AutoBuildEnabled = GetBool(BuildOnSaveEnabledPropName, true);
		}
		private void EnsureDefaultExtensions()
		{
			// If property isn't set, set Enabled = true
			if (!PropertyExists(BuildOnSaveEnabledPropName))
				SetBool(BuildOnSaveEnabledPropName, true);

			// If property doesn't exist, set default extensions
			if (!PropertyExists(ExtensionsPropName))
				SetStringCollection(ExtensionsPropName, DefaultExtensions);

			// If property does exist, ensure all defaults are present
			else
			{
				// Get current extensions
				var currExtensions = GetStringCollection(ExtensionsPropName).ToList();
				var nonDefault = currExtensions.Where(e => !DefaultExtensions.Contains(e));

				// If length of NON-DEFAULT extensions PLUS the length of DEFAULT extensions
				// is the SAME AS the length of the ORIGINAL list, we can assume that all defaults
				// are accounted for
				if (nonDefault.Count() + DefaultExtensions.Length != currExtensions.Count)
				{
					currExtensions.AddRange(DefaultExtensions);
					SetStringCollection(ExtensionsPropName, currExtensions.Distinct().ToArray());
				}
			}
		}

		#region Public
		/// <summary>
		///		The collection of file extensions that exist in the repository
		/// </summary>
		public IEnumerable<string> Extensions { get; set; }
		public bool AutoBuildEnabled { get; set; }

		/// <summary>
		///		Saves changes to ALL the public properties of this class
		/// </summary>
		public void SaveAllChanges()
		{
			// Set extensions
			SaveExtensions();

			// Set Enabled
			SaveAutoBuildEnabled();
		}

		/// <summary>
		///		Saves changes to extensions
		/// </summary>
		public void SaveExtensions()
		{
			SetStringCollection(ExtensionsPropName, Extensions);
		}

		/// <summary>
		///		Saves changes to AutoBuildEnabled
		/// </summary>
		public void SaveAutoBuildEnabled()
		{
			SetBool(BuildOnSaveEnabledPropName, AutoBuildEnabled);
		}
		#endregion

		#region Private Methods
		private bool PropertyExists(string key, string path = SettingsRootPath)
		{
			int exists;
			SettingsStore.PropertyExists(path, key, out exists);

			return Utilities.IntToBool(exists);
		}

		private bool CollectionExists(string path = SettingsRootPath)
		{
			int exists;
			SettingsStore.CollectionExists(path, out exists);

			return Utilities.IntToBool(exists);
		}

		private IEnumerable<string> GetStringCollection(string key)
		{
			string val = GetString(key);
			if (val.Contains(','))
				return val.Split(',').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
			else
				return new[] { val };
		}

		private void SetStringCollection(string key, IEnumerable<string> value)
		{
			// Flatten string into form 'extension1,extension2,extension3'
			var sb = new StringBuilder();
			foreach (var item in value)
			{
				sb.Append($"{item.Trim()},");
			}
			var flatStr = sb.ToString();
			if (flatStr.Trim(' ', '\n').EndsWith(","))
				flatStr = flatStr.Remove(flatStr.LastIndexOf(','));

			// Set value
			SetString(key, flatStr);
		}

		private string GetString(string key, string defaultValue = "")
		{
			string value;
			SettingsStore.GetStringOrDefault(SettingsRootPath, key, "", out value);
			return value;
		}

		private void SetString(string key, string value)
		{
			SettingsStore.SetString(SettingsRootPath, key, value);
		}

		private bool GetBool(string key, bool defaultValue = false)
		{
			int value = Utilities.BoolToInt(defaultValue);
			SettingsStore.GetBoolOrDefault(SettingsRootPath, key, value, out value);
			return Utilities.IntToBool(value);
		}

		private void SetBool(string key, bool value)
		{
			SettingsStore.SetBool(SettingsRootPath, key, Utilities.BoolToInt(value));
		}
		#endregion
	}
}
