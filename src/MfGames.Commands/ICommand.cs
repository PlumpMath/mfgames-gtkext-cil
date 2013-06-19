// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands
{
	/// <summary>
	/// Represents a command that can be executed.
	/// </summary>
	/// <typeparam name="TState">
	/// The type of state object needed to be updated with the execution of commands.
	/// </typeparam>
	public interface ICommand<TState>
	{
		#region Methods

		/// <summary>
		/// Performs the command for the first time.
		/// </summary>
		/// <param name="state">The initial state of the system.</param>
		/// <returns>The state of the system after executing.</returns>
		TState Do(TState state);

		#endregion
	}
}
