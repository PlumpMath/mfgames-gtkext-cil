// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands
{
	/// <summary>
	/// A command that can be undone and redone, with various controls to dictate
	/// when this is available.
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public interface IUndoableCommand<in TContext>: ICommand<TContext>
	{
		#region Properties

		/// <summary>
		/// If true, then the command can be undo and it is managed as such by
		/// the controller. If the command cannot be undone, then
		/// it prevents any preceeding action from being undone.
		/// </summary>
		bool CanUndo { get; }

		/// <summary>
		/// If true, then the command is one that doesn't change the state of the system
		/// and does not need to be recorded in an undo or redo buffer.
		/// 
		/// One example of this would be keyboard navigation commands which are optionally
		/// recorded depending on user preferences.
		/// </summary>
		bool IsTransient { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Requests that the command be re-executed after an Undo() operation. This
		/// assumes that the state of the system is identical (or as close as possible)
		/// to the state at the point of the initial Do().
		/// </summary>
		/// <returns>The state of the system after executing.</returns>
		void Redo(TContext state);

		/// <summary>
		/// Undoes the command to return the system to the state before the command
		/// was executed. The Undo() command is only called when the state is identical
		/// to after the Do() or Redo() command was executed. In addition, it will
		/// only be called if <see cref="CanUndo"/> is true.
		/// </summary>
		/// <returns>The state of the system after executing.</returns>
		void Undo(TContext state);

		#endregion
	}
}
