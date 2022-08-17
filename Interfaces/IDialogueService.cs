using System;
using System.Threading.Tasks;
using System.Windows;

namespace CoreUtilities.Interfaces
{
	public interface IDialogueService
	{
		void RegisterViewForViewModel(Type viewType, Type vmType);

		Task OpenCustomDialogue(object? dataContext = null);

		void OpenFileDialogue();

		void ShowMessageBox(string title, string message, MessageBoxButton button);
	}
}
