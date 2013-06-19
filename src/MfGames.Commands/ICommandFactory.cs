// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using MfGames.HierarchicalPaths;

namespace MfGames.Commands
{
	/// <summary>
	/// Describes a command factory, a class that encapsulates logic that affects the
	/// state of the system, typically by generating one or more commands in response
	/// to a CommandFactoryReference request. The command factory also is responsible for
	/// the basic formatting of a command by providing the enabled and visible state for
	/// command references along with descriptive text.
	/// </summary>
	public interface ICommandFactory<TStatus>
	{
		#region Properties

		/// <summary>
		/// Retrieves a list of all view keys associated with the factory.
		/// </summary>
		IEnumerable<HierarchicalPath> Keys { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Executes the logic for a given command references (as identified by the
		/// CommandFactoryReference.Key) using the given context for information.
		/// </summary>
		/// <param name="context">The context of the request. This will typically be a widget or graphical element that has the focus when the command reference is activated.</param>
		/// <param name="commandFactoryReference">The command reference to be activated or used.</param>
		void Do(
			object context,
			CommandFactoryReference commandFactoryReference,
			CommandFactoryManager<TStatus> controller);

		/// <summary>
		/// Retrives a short, descriptive name of the command for the user. This should
		/// be translated appropriately before returning. Because the title is used in
		/// GUI elements, a single "&" in front of the preferred character is encouraged.
		/// </summary>
		/// <param name="commandFactoryReference">
		/// A command reference that contains a path and an optional data object.
		/// </param>
		/// <returns>A string suitable for display to the user.</returns>
		string GetTitle(CommandFactoryReference commandFactoryReference);

		#endregion
	}
}
