// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands
{
	/// <summary>
	/// Represents a command that can be executed.
	/// </summary>
	/// <typeparam name="TContext">
	/// The type of state object needed to be updated with the execution of commands.
	/// </typeparam>
	public interface ICommand<in TContext>
	{
		#region Methods

		/// <summary>
		/// Performs the command for the first time.
		/// </summary>
		/// <param name="state">The initial state of the system.</param>
		/// <returns>The state of the system after executing.</returns>
		void Do(TContext state);

		#endregion
	}
}
