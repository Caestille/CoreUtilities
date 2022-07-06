using CoreUtilities.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoreUtilities.Services
{
	public class RegistryService : IRegistryService
	{
		private static string keyLocation;
		private static Dictionary<string, string> subPaths = new ();

		public RegistryService()
		{
			if (!File.Exists("AppGuid.txt"))
			{
				File.WriteAllText("AppGuid.txt", Guid.NewGuid().ToString());
			}
			string appGuid = File.ReadAllText("AppGuid.txt");
			keyLocation = $@"SOFTWARE\{appGuid}";
		}

		public void SetKeyLocation(string location)
		{
			keyLocation = location;
		}

		public void AddSubPath(string key, string path)
		{
			subPaths[key] = path;
		}

		public void SetSetting(string setting, string value)
		{
			var keyPath = keyLocation + (subPaths.ContainsKey(setting) ? subPaths[setting] : "");
			// Despite name, this will open the key if it already exists
			RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);
			key.SetValue(setting, value);
			key.Close();
		}

		public bool TryGetSetting<T>(string setting, T defaultValue, out T value)
		{
			var success = false;
			object? outOfRegistryValue = null;
			var keyPath = keyLocation + (subPaths.ContainsKey(setting) ? subPaths[setting] : "");
			// Despite name, this will open the key if it already exists
			RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);

			try
			{
				outOfRegistryValue = key.GetValue(setting);
				success = outOfRegistryValue != null;
			}
			catch 
			{
				value = defaultValue;
				SetSetting(setting, defaultValue.ToString());
				return false;
			}
			finally
			{
				key.Close();
			}

			value = (T)Convert.ChangeType(outOfRegistryValue, typeof(T));
			return success;
		}

		public void DeleteSetting(string setting)
		{
			var keyPath = keyLocation + (subPaths.ContainsKey(setting) ? subPaths[setting] : "");
			// Despite name, this will open the key if it already exists
			var key = Registry.CurrentUser.CreateSubKey(keyPath);

			try
			{
				key.DeleteValue(setting);
			}
			catch { }
			finally
			{
				key.Close();
			}
		}
	}
}
