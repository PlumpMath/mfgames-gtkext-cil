// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands
{
	/// <summary>
	/// Describes the interface for a command controller, the system for executing
	/// commands.
	/// </summary>
	/// <typeparam name="TState">
	/// The type of object that would represent the state of the system after executing a command.
	/// </typeparam>
	public interface ICommandController<TState>
	{
		#region Properties

		/// <summary>
		/// Contains a flag if the controller has commands that can be redone.
		/// </summary>
		bool CanRedo { get; }

		/// <summary>
		/// Contains a flag if the controller has commands that can be undone.
		/// </summary>
		bool CanUndo { get; }

		/// <summary>
		/// Contains the state of the last command executed, regardless if it was a Do(),
		/// Undo(), or Redo().
		/// </summary>
		TState State { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Executes a command in the system and manages the resulting state.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		void Do(
			ICommand<TState> command,
			TState state);

		/// <summary>
		/// Re-performs a command that was recently undone.
		/// </summary>
		void Redo(TState state);

		/// <summary>
		/// Undoes a command that was recently done, either through the Do() or Redo().
		/// </summary>
		void Undo(TState state);

		#endregion
	}
}
