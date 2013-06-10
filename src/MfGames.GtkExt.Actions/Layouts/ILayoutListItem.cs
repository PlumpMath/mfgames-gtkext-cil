// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Represents the common functionality for all layout items.
	/// </summary>
	public interface ILayoutListItem
	{
		#region Methods

		/// <summary>
		/// Populates the specified shell with sub-menus.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="shell">The shell.</param>
		void Populate(
			ActionManager manager,
			MenuShell shell);

		#endregion
	}
}
