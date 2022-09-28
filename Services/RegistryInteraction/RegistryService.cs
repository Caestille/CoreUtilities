using CoreUtilities.Interfaces.RegistryInteraction;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoreUtilities.Services.RegistryInteraction
{
	/// <summary>
	/// Implementation of <see cref="IRegistryService"/> for interacting with keys/values from the windows registry.
	/// </summary>
	public class RegistryService : IRegistryService
	{
		private string keyLocation;

		/// <summary>
		/// Constructor for <see cref="RegistryService"/>.
		/// </summary>
		/// <param name="keyDirectory">Base directory which other calls are relative to.</param>
		/// <param name="addGuid">Flag, if <see cref="true"/>, a guid is appended to the keyDirectory, allowing
		/// for multiple of the same application. This guid is persisted in the application base directory.</param>
		public RegistryService(string keyDirectory, bool addGuid = false)
		{
			keyLocation = keyDirectory;
			if (addGuid)
			{
				if (!File.Exists("AppGuid.txt"))
				{
					File.WriteAllText("AppGuid.txt", Guid.NewGuid().ToString());
				}
				string appGuid = File.ReadAllText("AppGuid.txt");
				keyLocation = $@"{keyDirectory}\{appGuid}";
			}
		}

		/// <inheritdoc/>
		public void SetSetting(string setting, string value, string pathAfterKeyLocation = "")
		{
			// Despite name, this will open the key if it already exists
			RegistryKey key = Registry.CurrentUser.CreateSubKey(keyLocation + pathAfterKeyLocation);
			key.SetValue(setting, value);
			key.Close();
		}

		/// <inheritdoc/>
		public bool TryGetSetting<T>(string setting, T defaultValue, out T value, string pathAfterKeyLocation = "")
		{
			var success = false;
			object? outOfRegistryValue = null;
			// Despite name, this will open the key if it already exists
			RegistryKey key = Registry.CurrentUser.CreateSubKey(keyLocation + pathAfterKeyLocation);

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

			value = success ? (T)Convert.ChangeType(outOfRegistryValue, typeof(T)) : defaultValue;
			return success;
		}

		/// <inheritdoc/>
		public Dictionary<string, object> GetAllSettingsInPath(string pathAfterKeyLocation)
		{
			var valuesBynames = new Dictionary<string, object>();
			using (RegistryKey rootKey = Registry.CurrentUser.OpenSubKey(keyLocation + pathAfterKeyLocation))
			{
				if (rootKey != null)
				{
					string[] valueNames = rootKey.GetValueNames();
					foreach (string currSubKey in valueNames)
					{
						object value = rootKey.GetValue(currSubKey);
						valuesBynames.Add(currSubKey, value);
					}
					rootKey.Close();
				}
			}
			return valuesBynames;
		}

		/// <inheritdoc/>
		public void DeleteSetting(string setting, string pathAfterKeyLocation = "")
		{
			// Despite name, this will open the key if it already exists
			var key = Registry.CurrentUser.CreateSubKey(keyLocation + pathAfterKeyLocation);

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

		/// <inheritdoc/>
		public void DeleteSubTree(string pathAfterKeyLocation)
		{
			try
			{
				Registry.CurrentUser.DeleteSubKeyTree(keyLocation + pathAfterKeyLocation);
			}
			catch { }
		}
	}
}
