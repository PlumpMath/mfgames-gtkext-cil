// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace GtkExtDemo.Actions
{
	/// <summary>
	/// Defines a user action that switches notebook pages.
	/// </summary>
	public class SwitchPageAction: Action
	{
		#region Methods

		/// <summary>
		/// Switched the notebook page when activated.
		/// </summary>
		protected override void OnActivated()
		{
			notebook.Page = pageNumber;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchPageAction"/>
		/// class and configures it to switch to the given page.
		/// </summary>
		/// <param name="notebook">The notebook.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="label">The label.</param>
		public SwitchPageAction(
			Notebook notebook,
			int pageNumber,
			string label)
			: base("SwitchPage" + pageNumber, label)
		{
			this.notebook = notebook;
			this.pageNumber = pageNumber;
		}

		#endregion

		#region Fields

		private readonly Notebook notebook;
		private readonly int pageNumber;

		#endregion
	}
}
