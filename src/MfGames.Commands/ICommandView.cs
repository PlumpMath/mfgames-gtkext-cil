// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license
namespace MfGames.Commands
{
	/// <summary>
	/// Describes a provider of command display information for a registered command.
	/// This takes the ICommand.Data property as the arguments and
	/// renders the result appropriately for the user and their current locale.
	/// </summary>
	public interface ICommandView
	{
		/// <summary>
		/// Retrives a short, descriptive name of the command for the user. This should
		/// be translated appropriately before returning.
		/// </summary>
		/// <param name="data">The ICommand.Data object of the registered command.</param>
		/// <returns>A string suitable for display to the user.</returns>
		string GetDisplayName(object data);
	}
}