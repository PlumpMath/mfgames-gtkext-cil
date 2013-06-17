// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using MfGames.HierarchicalPaths;

namespace MfGames.Commands
{
	/// <summary>
	/// Encapsulates the functionality of rendering <see cref="CommandReference"/>
	/// objects into a user-formatted display. This manager allows for commands to
	/// be referenced via a <see cref="HierarchicalPaths"/> within menus and
	/// popups while still expanding into a (potentially translated) description with
	/// toolkit-specific icons and elements.
	/// </summary>
	public class CommandViewManager
	{
		#region Methods

		/// <summary>
		/// Retrives a short, descriptive name of the command for the user. This should
		/// be translated appropriately before returning.
		/// </summary>
		/// <param name="commandReference">A <see cref="CommandReference"/> representing the command and an optional parameter.</param>
		/// <returns>A string suitable for display to the user.</returns>
		public string GetDisplayName(CommandReference commandReference)
		{
			return null;
		}

		/// <summary>
		/// Registers a command view for a given <c>ICommand.Key</c> reference.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="commandView"></param>
		public void Register(
			HierarchicalPath key,
			ICommandView commandView)
		{
		}

		#endregion
	}
}
