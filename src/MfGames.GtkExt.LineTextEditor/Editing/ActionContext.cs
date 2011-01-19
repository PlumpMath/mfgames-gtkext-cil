using MfGames.GtkExt.LineTextEditor.Interfaces;

namespace MfGames.GtkExt.LineTextEditor.Editing
{
	public class ActionContext : IActionContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionContext"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public ActionContext(IDisplayContext displayContext)
		{
			this.displayContext = displayContext;
		}

		#endregion

		#region Context

		private readonly IDisplayContext displayContext;

		/// <summary>
		/// Gets the display context.
		/// </summary>
		/// <value>The display context.</value>
		public IDisplayContext Display
		{
			get { return displayContext; }
		}

		#endregion

	}
}