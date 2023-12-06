using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CoreUtilities.Interfaces.Dialogues
{
	/// <summary>
	/// Interface for classes managing dialogues to inherit.
	/// </summary>
	public interface IDialogueService
	{
		/// <summary>
		/// Registers a view type to associate with a viewmodel type.
		/// </summary>
		/// <param name="viewType">The type of view to register.</param>
		/// <param name="vmType">The type of viewmodel to register to.</param>
		void RegisterViewForViewModel(Type viewType, Type vmType);

		/// <summary>
		/// Given a datacontext (viewmodel), opens a registered dialogue (if it exists) in a standard window.
		/// </summary>
		/// <param name="dataContext">An <see cref="object"/> which is the datacontext of the view to be opened
		/// (if registered).</param>
		/// <param name="dialogueSize">An overriding dialogue size if required, otherwise it is automatic (which
		/// can have undesired results).</param>
		/// <returns>An (awaitable) <see cref="Task"/>.</returns>
		void ShowCustomDialogue(object dataContext, Size? dialogueSize = null);

        /// <summary>
        /// Given a datacontext (viewmodel), opens a registered dialogue (if it exists) in a borderless window.
        /// </summary>
        /// <param name="dataContext">An <see cref="object"/> which is the datacontext of the view to be opened
        /// (if registered).</param>
        /// <param name="dialogueSize">An overriding dialogue size if required, otherwise it is automatic (which
        /// can have undesired results).</param>
        /// <returns>An (awaitable) <see cref="Task"/>.</returns>
        void ShowBorderlessCustomDialogue(object dataContext, Size? dialogueSize = null);

		/// <summary>
		/// Opens an open file dialogue.
		/// </summary>
		/// <returns>The path to open.</returns>
		string ShowOpenFileDialogue();

		/// <summary>
		/// Opens a colour picker dialogue.
		/// </summary>
		/// <param name="inputColour">The starting colour of the dialogue.</param>
		/// <param name="colourChangedCallback">Called when the colour selected in the dialogue changes.</param>
		/// <returns>The picked <see cref="Color"/>.</returns>
		Color ShowColourPickerDialogue(Color inputColour, Action<Color>? colourChangedCallback = null);

		/// <summary>
		/// Shows a message box to display to the user.
		/// </summary>
		/// <param name="title">The message box title.</param>
		/// <param name="message">The message in the message box.</param>
		/// <param name="button">The type of confirmation button(s) the message box should display.</param>
		void ShowMessageBox(string title, string message, MessageBoxButton button);
	}
}
