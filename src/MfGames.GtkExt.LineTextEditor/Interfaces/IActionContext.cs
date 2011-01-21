using MfGames.GtkExt.LineTextEditor.Actions;

namespace MfGames.GtkExt.LineTextEditor.Interfaces
{
	/// <summary>
	/// Defines the interface to the context for a specific action.
	/// </summary>
	public interface IActionContext
	{
		/// <summary>
		/// Gets the display context for this action.
		/// </summary>
		/// <value>The display.</value>
		IDisplayContext DisplayContext { get; }

        /// <summary>
        /// Gets the action states associated with the action.
        /// </summary>
        ActionStateCollection States { get; }
	}
}