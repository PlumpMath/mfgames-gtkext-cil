// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MfGames.Commands
{
	/// <summary>
	/// Encapsulates the functionality for executing commands within a system. The
	/// controller handles undo and redo functionality, performing the actions, and
	/// keeping track of the last state of the executing command.
	/// </summary>
	/// <typeparam name="TContext">
	/// The type of state object needed to be updated with the execution of commands.
	/// </typeparam>
	public class UndoRedoCommandController<TContext>: ICommandController<TContext>
	{
		#region Properties

		/// <summary>
		/// Contains a flag if the controller has commands that can be redone.
		/// </summary>
		public bool CanRedo
		{
			get { return redoCommands.Count > 0; }
		}

		/// <summary>
		/// Contains a flag if the controller has commands that can be undone.
		/// </summary>
		public bool CanUndo
		{
			get { return undoCommands.Count > 0; }
		}

		/// <summary>
		/// Contains the state of the last command executed, regardless if it was a Do(),
		/// Undo(), or Redo().
		/// </summary>
		public TContext State { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Executes a command in the system and manages the resulting state.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		public void Do(
			ICommand<TContext> command,
			TContext state)
		{
			// Determine if this command is undoable or not.
			var undoableCommand = command as IUndoableCommand<TContext>;

			if (undoableCommand == null
				|| !undoableCommand.CanUndo)
			{
				// Since the command cannot be undone, we need to clear out the undo
				// buffer, which will make sure CanUndo will return false and we can
				// recover that memory.
				undoCommands.Clear();
			}

			// In all cases, we clear out the redo buffer because this is a
			// user-initiated command and redo no longer makes sense.
			redoCommands.Clear();

			// Perform the operation.
			Do(command, state, false, false);
		}

		/// <summary>
		/// Re-performs a command that was recently undone.
		/// </summary>
		public void Redo(TContext state)
		{
			// Make sure we're in a known and valid state.
			Contract.Assert(CanRedo);

			// Pull off the first command from the redo buffer and perform it.
			ICommand<TContext> command = redoCommands[0];
			redoCommands.RemoveAt(0);
			Do(command, state, false, true);
		}

		/// <summary>
		/// Undoes a command that was recently done, either through the Do() or Redo().
		/// </summary>
		public void Undo(TContext state)
		{
			// Make sure we're in a known and valid state.
			Contract.Assert(CanUndo);

			// Pull off the first undo command, get its inverse operation, and
			// perform it.
			ICommand<TContext> command = undoCommands[0];
			undoCommands.RemoveAt(0);
			Do(command, state, true, true);
		}

		/// <summary>
		/// The internal "do" method is what performs the actual work on the project.
		/// It also handles pushing the appropriate command on the correct undo/redo
		/// list.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="state"></param>
		/// <param name="useUndo">if set to <c>true</c> [use inverse].</param>
		/// <param name="ignoreDeferredCommands">if set to <c>true</c> [ignore deferred commands].</param>
		private void Do(
			ICommand<TContext> command,
			TContext state,
			bool useUndo,
			bool ignoreDeferredCommands)
		{
			// Perform the action based on undo or redo.
			var undoableCommand = command as IUndoableCommand<TContext>;

			if (undoableCommand == null
				|| !useUndo)
			{
				command.Do(state);
			}
			else
			{
				undoableCommand.Undo(state);
			}

			// Add the action to the appropriate buffer. This assumes that the undo
			// and redo operations have been properly managed before this method is
			// called. This does not manage the buffers since the undo/redo allows
			// the user to go back and forth between the two lists.
			if (undoableCommand != null
				&& undoableCommand.CanUndo
				&& !undoableCommand.IsTransient)
			{
				if (useUndo)
				{
					redoCommands.Insert(0, command);
				}
				else
				{
					undoCommands.Insert(0, command);
				}
			}

			// If we are ignoring deferred commands, we clear them to ensure they
			// aren't run in the next execution.
			if (ignoreDeferredCommands)
			{
				deferredCommands.Clear();
			}

			// If we have deferred commands, then execute them in order.
			if (deferredCommands.Count > 0)
			{
				// Copy the list and clear it out in case any of these actions
				// add any more deferred commands.
				var commands = new List<ICommand<TContext>>(deferredCommands);

				deferredCommands.Clear();

				// Go through the commands and process each one.
				foreach (ICommand<TContext> deferredCommand in commands)
				{
					Do(deferredCommand, state);
				}
			}
		}

		#endregion

		#region Constructors

		public UndoRedoCommandController()
		{
			undoCommands = new List<ICommand<TContext>>();
			redoCommands = new List<ICommand<TContext>>();
			deferredCommands = new List<ICommand<TContext>>();
		}

		#endregion

		#region Fields

		private readonly List<ICommand<TContext>> deferredCommands;
		private readonly List<ICommand<TContext>> redoCommands;
		private readonly List<ICommand<TContext>> undoCommands;

		#endregion
	}
}
