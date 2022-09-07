using System.Collections.Generic;

namespace CoreUtilities.Interfaces
{
	/// <summary>
	/// Service for interacting with keys and values in a registry.
	/// </summary>
	public interface IRegistryService
	{
		/// <summary>
		/// Sets a setting on a given registry path.
		/// </summary>
		/// <param name="setting">The setting path.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="pathAfterKeyLocation">Custom path after the key location (usually for sub folders).</param>
		void SetSetting(string setting, string value, string pathAfterKeyLocation = "");

		/// <summary>
		/// Trys to get a setting from a given registry path.
		/// </summary>
		/// <typeparam name="T">The type to try convert to after retrieval.</typeparam>
		/// <param name="setting">The setting path.</param>
		/// <param name="defaultValue">A default value to set to the path and return if retrieval fails.</param>
		/// <param name="value">An out parameter which is the converted retrieved value if successful</param>
		/// <param name="pathAfterKeyLocation">Custom path after the key location (usually for sub folders).</param>
		/// <returns>A <see cref="bool"/> indicating whether retrieval was succesful. If false, the default value 
		/// is returned and set to the path. </returns>
		bool TryGetSetting<T>(string setting, T defaultValue, out T value, string pathAfterKeyLocation = "");

		/// <summary>
		/// Gets all settings on a given registry path.
		/// </summary>
		/// <param name="pathAfterKeyLocation">Custom path after the key location (usually for sub folders).</param>
		/// <returns>A <see cref="Dictionary{TKey, TValue}"/> of key <see cref="string"/> and value 
		/// <see cref="object"/> which is all settings from the given registry path.</returns>
		Dictionary<string, object> GetAllSettingsInPath(string pathAfterKeyLocation);

		/// <summary>
		/// Deletes a setting on a given registry path.
		/// </summary>
		/// <param name="setting">The path of the setting to delete.</param>
		/// <param name="pathAfterKeyLocation">Custom path after the key location (usually for sub folders).</param>
		void DeleteSetting(string setting, string pathAfterKeyLocation = "");

		/// <summary>
		/// Deletes an entire registry tree.
		/// </summary>
		/// <param name="pathAfterKeyLocation">Custom path after the key location (usually for sub folders).</param>
		void DeleteSubTree(string pathAfterKeyLocation);
	}
}
