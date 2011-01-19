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
		IDisplayContext Display { get; }
	}
}