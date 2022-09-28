using System;
using System.Threading.Tasks;
using System.Windows;

namespace CoreUtilities.Interfaces.Dialogues
{
    public interface IDialogueService
    {
        /// <summary>
        /// Registers a view type to associate with a viewmodel type.
        /// </summary>
        /// <param name="viewType">The type of view to register.</param>
        /// <param name="vmType">The type of viewmodel to register to.</param>
        void RegisterViewForViewModel(Type viewType, Type vmType);

        /// <summary>
        /// Given a datacontext (viewmodel), opens a registered dialogue (if it exists)
        /// </summary>
        /// <param name="dataContext">An <see cref="object"/> which is the datacontext of the view to be opened
        /// (if registered).</param>
        /// <param name="dialogueSize">An overriding dialogue size if required, otherwise it is automatic (which
        /// can have undesired results).</param>
        /// <returns>An (awaitable) <see cref="Task"/>.</returns>
        Task OpenCustomDialogue(object? dataContext = null, Size? dialogueSize = null);

        /// <summary>
        /// Opens an open file dialogue.
        /// </summary>
        /// <returns>The path to open.</returns>
        string OpenFileDialogue();

        /// <summary>
        /// Shows a message box to display to the user.
        /// </summary>
        /// <param name="title">The message box title.</param>
        /// <param name="message">The message in the message box.</param>
        /// <param name="button">The type of confirmation button(s) the message box should display.</param>
        void ShowMessageBox(string title, string message, MessageBoxButton button);
    }
}
