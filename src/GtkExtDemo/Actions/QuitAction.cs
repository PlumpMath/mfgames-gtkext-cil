// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace GtkExtDemo.Actions
{
	/// <summary>
	/// Represents the quit action.
	/// </summary>
	public class QuitAction: Action
	{
		#region Methods

		/// <summary>
		/// Called when the action is called.
		/// </summary>
		protected override void OnActivated()
		{
			Application.Quit();
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="QuitAction"/> class
		/// and uses the stock settings for it.
		/// </summary>
		public QuitAction()
			: base("Quit", null, null, Stock.Quit)
		{
		}

		#endregion
	}
}
