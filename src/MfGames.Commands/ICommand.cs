// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license
namespace MfGames.Commands
{
	/// <summary>
	/// Represents a command that can be executed and optionally undone or redone.
	/// </summary>
	/// <typeparam name="TState">
	/// The type of state object needed to be updated with the execution of commands.
	/// </typeparam>
	public interface ICommand<out TState>
	{
		/// <summary>
		/// If true, then the command can be undo and it is managed as such by
		/// the controller. If the command cannot be undone, then
		/// it prevents any preceeding action from being undone.
		/// </summary>
		bool CanUndo { get; }

		/// <summary>
		/// Performs the command for the first time.
		/// </summary>
		/// <returns>The state of the system after executing.</returns>
		TState Do();

		/// <summary>
		/// Undoes the command to return the system to the state before the command
		/// was executed. The Undo() command is only called when the state is identical
		/// to after the Do() or Redo() command was executed. In addition, it will
		/// only be called if <see cref="CanUndo"/> is true.
		/// </summary>
		/// <returns>The state of the system after executing.</returns>
		TState Undo();

		/// <summary>
		/// Requests that the command be re-executed after an Undo() operation. This
		/// assumes that the state of the system is identical (or as close as possible)
		/// to the state at the point of the initial Do().
		/// </summary>
		/// <returns>The state of the system after executing.</returns>
		TState Redo();
	}
}