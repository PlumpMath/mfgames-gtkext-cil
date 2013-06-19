// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands.TextEditing
{
	/// <summary>
	/// Extends the command controller to provide the basic text editing commands
	/// appropriate for the controller. All other text editing commands are composites
	/// of the core commands.
	/// </summary>
	/// <typeparam name="TStatus"></typeparam>
	public interface ITextEditingCommandController<TStatus>:
		ICommandController<TStatus>
	{
	}
}
