// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MfGames.Commands
{
	/// <summary>
	/// Encapsulates the functionality for executing commands within a system. The
	/// controller handles undo and redo functionality, performing the actions, and
	/// keeping track of the last state of the executing command.
	/// </summary>
	/// <typeparam name="TState">
	/// The type of state object needed to be updated with the execution of commands.
	/// </typeparam>
	public class UndoRedoCommandController<TState>: ICommandController<TState>
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
		public TState State { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Executes a command in the system and manages the resulting state.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		public void Do(ICommand<TState> command)
		{
			// Determine if this command is undoable or not.
			if (!command.CanUndo)
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
			Do(command, false, false);
		}

		/// <summary>
		/// Re-performs a command that was recently undone.
		/// </summary>
		public void Redo()
		{
			// Make sure we're in a known and valid state.
			Contract.Assert(CanRedo);

			// Pull off the first command from the redo buffer and perform it.
			ICommand<TState> command = redoCommands[0];
			redoCommands.RemoveAt(0);
			Do(command, false, true);
		}

		/// <summary>
		/// Undoes a command that was recently done, either through the Do() or Redo().
		/// </summary>
		public void Undo()
		{
			// Make sure we're in a known and valid state.
			Contract.Assert(CanUndo);

			// Pull off the first undo command, get its inverse operation, and
			// perform it.
			ICommand<TState> command = undoCommands[0];
			undoCommands.RemoveAt(0);
			Do(command, true, true);
		}

		/// <summary>
		/// Updates the state object. The default implementation does not update the
		/// state if the state object is null.
		/// </summary>
		/// <param name="state">The new state object to use.</param>
		protected virtual void UpdateState(TState state)
		{
			if (state != null)
			{
				State = state;
			}
		}

		/// <summary>
		/// The internal "do" method is what performs the actual work on the project.
		/// It also handles pushing the appropriate command on the correct undo/redo
		/// list.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="useDo">if set to <c>true</c> [use inverse].</param>
		/// <param name="ignoreDeferredCommands">if set to <c>true</c> [ignore deferred commands].</param>
		private void Do(
			ICommand<TState> command,
			bool useDo,
			bool ignoreDeferredCommands)
		{
			// Perform the action based on undo or redo.
			TState newState = useDo
				? command.Do()
				: command.Undo();

			UpdateState(newState);

			// Add the action to the appropriate buffer. This assumes that the undo
			// and redo operations have been properly managed before this method is
			// called. This does not manage the buffers since the undo/redo allows
			// the user to go back and forth between the two lists.
			if (command.CanUndo
				&& !command.IsTransient)
			{
				if (useDo)
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
				var commands = new List<ICommand<TState>>(deferredCommands);

				deferredCommands.Clear();

				// Go through the commands and process each one.
				foreach (ICommand<TState> deferredCommand in commands)
				{
					Do(deferredCommand);
				}
			}
		}

		#endregion

		#region Constructors

		public UndoRedoCommandController()
		{
			undoCommands = new List<ICommand<TState>>();
			redoCommands = new List<ICommand<TState>>();
			deferredCommands = new List<ICommand<TState>>();
		}

		#endregion

		#region Fields

		private readonly List<ICommand<TState>> deferredCommands;
		private readonly List<ICommand<TState>> redoCommands;
		private readonly List<ICommand<TState>> undoCommands;

		#endregion
	}
}
