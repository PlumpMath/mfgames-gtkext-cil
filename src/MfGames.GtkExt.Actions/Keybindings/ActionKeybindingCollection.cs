// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using MfGames.Collections;

namespace MfGames.GtkExt.Actions.Keybindings
{
	/// <summary>
	/// Implements a keybinding collection that has the individual levels of a
	/// chained keybinding at each level. For example, a keybinding of "Ctrl+Q"
	/// would be at the top level, but "Ctrl+M, M" would have "Ctrl+M" at the first
	/// level and "M" at the second.
	/// </summary>
	public class ActionKeybindingCollection: HierarchicalPathTreeCollection<string>
	{
	}
}
