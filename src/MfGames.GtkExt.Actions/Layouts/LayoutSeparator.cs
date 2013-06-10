// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Represents a separator for the screen.
	/// </summary>
	public class LayoutSeparator: ILayoutListItem
	{
		#region Methods

		/// <summary>
		/// Populates the specified shell with sub-menus.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="shell">The shell.</param>
		public void Populate(
			ActionManager manager,
			MenuShell shell)
		{
			var separator = new MenuItem();
			shell.Add(separator);
		}

		#endregion
	}
}
