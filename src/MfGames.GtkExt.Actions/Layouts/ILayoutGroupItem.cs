// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Identifies an item that can be inside a layout group.
	/// </summary>
	public interface ILayoutGroupItem
	{
		#region Methods

		/// <summary>
		/// Populates the specified shell with sub-menus.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="menuBar">The menu bar.</param>
		void Populate(
			ActionManager manager,
			MenuBar menuBar);

		#endregion
	}
}
