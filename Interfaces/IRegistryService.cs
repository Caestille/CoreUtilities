using System.Collections.Generic;

namespace CoreUtilities.Interfaces
{
	public interface IRegistryService
	{
		void SetSetting(string setting, string? value, string pathAfterKeyLocation = "");

		bool TryGetSetting<T>(string setting, T defaultValue, out T? value, string pathAfterKeyLocation = "");

		Dictionary<string, object> GetAllSettingsInPath(string pathAfterKeyLocation);

		void DeleteSetting(string setting, string pathAfterKeyLocation = "");

		void DeleteSubTree(string pathAfterKeyLocation);
	}
}
