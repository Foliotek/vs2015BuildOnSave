using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.OLE.Interop;

namespace BuildOnSave
{
	public class SettingsRepository
	{
		private const string SettingsRootPath = @"BuildOnSave\Settings\";
		private const string ExtensionsPropName = @"Extensions";
		private readonly string[] DefaultExtensions = { "cs", "config" };

		// Constructor
		public SettingsRepository()
		{
			var _dte = Package.GetGlobalService(typeof (DTE)) as DTE2;
			ServiceProvider = GetServiceProvider(_dte);
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
			if (!CollectionExists(SettingsRootPath))
				SettingsStore.CreateCollection(SettingsRootPath);

			// Ensure default extensions
			EnsureDefaultExtensions();

			// Initialize public properties
			Extensions = GetStringCollection(ExtensionsPropName);
		}
		private void EnsureDefaultExtensions()
		{
			// If property doesn't exist, set default extensions
			if (!PropertyExists(SettingsRootPath, ExtensionsPropName))
				SetStringCollection(ExtensionsPropName, DefaultExtensions);

			// If property does exist, ensure all defaults are present
			else
			{
				// Get current extensions
				var currExtensions = GetStringCollection(ExtensionsPropName).ToList();
				var nonDefault = currExtensions.Where(e => !DefaultExtensions.Contains(e)).ToList();

				// If length of NON-DEFAULT extensions PLUS the length of DEFAULT extensions
				// is the SAME AS the length of the ORIGINAL list, we know that all defaults
				// are accounted for
				if (nonDefault.Count + DefaultExtensions.Length != currExtensions.Count)
				{
					currExtensions.AddRange(DefaultExtensions);
					SetStringCollection(ExtensionsPropName, currExtensions.Distinct().ToArray());
				}
			}
		}

		#region Public
		// Properties
		public IEnumerable<string> Extensions { get; set; }

		// Methods
		public void SaveChanges()
		{
			SetStringCollection(ExtensionsPropName, Extensions);
		}
		#endregion

		#region Private Methods
		private bool CollectionExists(string path)
		{
			int exists;
			SettingsStore.CollectionExists(path, out exists);

			return ExistenceIntToBool(exists);
		}

		private bool PropertyExists(string path, string key)
		{
			int exists;
			SettingsStore.PropertyExists(path, key, out exists);

			return ExistenceIntToBool(exists);
		}

		private void SetString(string key, string value)
		{
			SettingsStore.SetString(SettingsRootPath, key, value);
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
			flatStr = flatStr.Remove(flatStr.LastIndexOf(','));

			// Set value
			SetString(key, flatStr);
		}

		private string GetString(string key)
		{
			string value;
			SettingsStore.GetStringOrDefault(SettingsRootPath, key, "", out value);
			return value;
		}

		private IEnumerable<string> GetStringCollection(string key)
		{
			string val = GetString(key);
			if (val.Contains(','))
				return val.Split(',').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
			else
				return new [] { val };
		}

		private static bool ExistenceIntToBool(int exists)
		{
			return exists == 1;
		}

		private static ServiceProvider GetServiceProvider(DTE2 dte)
		{
			return new ServiceProvider((IServiceProvider)dte);
		}
		#endregion
	}
}
