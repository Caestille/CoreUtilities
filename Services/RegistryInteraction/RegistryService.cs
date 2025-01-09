namespace CoreUtilities.Services.RegistryInteraction
{
    using CoreUtilities.Interfaces.RegistryInteraction;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implementation of <see cref="IRegistryService"/> for interacting with keys/values from the windows registry.
    /// </summary>
    public class RegistryService : IRegistryService
	{
		private readonly string keyLocation;

		/// <summary>
		/// Constructor for <see cref="RegistryService"/>.
		/// </summary>
		/// <param name="keyDirectory">Base directory which other calls are relative to.</param>
		/// <param name="addGuid">Flag, if <see cref="true"/>, a guid is appended to the keyDirectory, allowing
		/// for multiple of the same application. This guid is persisted in the application base directory.</param>
		public RegistryService(string keyDirectory, bool addGuid = false)
		{
            this.keyLocation = keyDirectory;
			if (addGuid)
			{
				if (!File.Exists("AppGuid.txt"))
				{
					File.WriteAllText("AppGuid.txt", Guid.NewGuid().ToString());
				}
				string appGuid = File.ReadAllText("AppGuid.txt");
                this.keyLocation = $@"{keyDirectory}\{appGuid}";
			}
		}

		/// <inheritdoc/>
		public void SetSetting(string setting, string value, string pathAfterKeyLocation = string.Empty)
		{
			// Despite name, this will open the key if it already exists
			RegistryKey key = Registry.CurrentUser.CreateSubKey(this.keyLocation + pathAfterKeyLocation);
			key.SetValue(setting, value);
			key.Close();
		}

		/// <inheritdoc/>
		public bool TryGetSetting<T>(string setting, T defaultValue, out T value, string pathAfterKeyLocation = string.Empty)
		{
			var success = false;
			object? outOfRegistryValue = null;
			// Despite name, this will open the key if it already exists
			RegistryKey key = Registry.CurrentUser.CreateSubKey(this.keyLocation + pathAfterKeyLocation);

			T? castValue = default;
			try
			{
				outOfRegistryValue = key.GetValue(setting);
				if (outOfRegistryValue != null && Convert.ChangeType(outOfRegistryValue, typeof(T)) is T val)
				{
					castValue = val;
					success = true;
				}
			}
			catch
			{
				value = defaultValue;
                this.SetSetting(setting, defaultValue?.ToString() ?? string.Empty);
				return false;
			}
			finally
			{
				key.Close();
			}

			value = success ? castValue! : defaultValue;
			return success;
		}

		/// <inheritdoc/>
		public Dictionary<string, object> GetAllSettingsInPath(string pathAfterKeyLocation)
		{
			var valuesBynames = new Dictionary<string, object>();
			using (var rootKey = Registry.CurrentUser.OpenSubKey(this.keyLocation + pathAfterKeyLocation))
			{
				if (rootKey != null)
				{
					string[] valueNames = rootKey.GetValueNames();
					foreach (string currSubKey in valueNames)
					{
						var value = rootKey.GetValue(currSubKey);
						if (value != null)
						{
							valuesBynames.Add(currSubKey, value);
						}
					}
					rootKey.Close();
				}
			}
			return valuesBynames;
		}

		/// <inheritdoc/>
		public void DeleteSetting(string setting, string pathAfterKeyLocation = string.Empty)
		{
			// Despite name, this will open the key if it already exists
			var key = Registry.CurrentUser.CreateSubKey(this.keyLocation + pathAfterKeyLocation);

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
				Registry.CurrentUser.DeleteSubKeyTree(this.keyLocation + pathAfterKeyLocation);
			}
			catch { }
		}
	}
}
