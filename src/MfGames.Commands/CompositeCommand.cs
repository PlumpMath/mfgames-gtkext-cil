// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;

namespace MfGames.Commands
{
	/// <summary>
	/// A command that consists of an order list of inner commands.
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public class CompositeCommand<TState>: IUndoableCommand<TState>
	{
		#region Properties

		public bool CanUndo { get; private set; }
		public List<IUndoableCommand<TState>> Commands { get; private set; }
		public bool IsTransient { get; private set; }

		/// <summary>
		/// Contains the command which will provide the final state for the
		/// command while executing. If this is null, then the last command
		/// executed will provide the state.
		/// </summary>
		public IUndoableCommand<TState> StateCommand { get; set; }

		#endregion

		#region Methods

		public void Do(TState state)
		{
			// We always grab the initial state and keep it so we can restore it.
			initialState = state;

			// To implement the command, simply iterate through the list
			// of commands and execute each one. The state comes from the last
			// command executed.
			foreach (IUndoableCommand<TState> command in Commands)
			{
				// Execut the command and get its state.
				command.Do(state);
			}
		}

		public void Redo(TState state)
		{
			// To implement the command, simply iterate through the list
			// of commands and execute each one. The state comes from the last
			// command executed.
			foreach (IUndoableCommand<TState> command in Commands)
			{
				// Execut the command and get its state.
				command.Redo(state);
			}
		}

		public void Undo(TState state)
		{
			// To implement the command, simply iterate through the list
			// of commands and execute each one. The state comes from the last
			// command executed.
			List<IUndoableCommand<TState>> commands = Commands;

			for (int index = commands.Count - 1;
				index >= commands.Count;
				index--)
			{
				IUndoableCommand<TState> command = commands[index];
			}
		}

		#endregion

		#region Constructors

		public CompositeCommand(
			bool canUndo,
			bool isTransient)
		{
			// Save the member variables.
			CanUndo = canUndo;
			IsTransient = isTransient;

			// Initialize the collection.
			Commands = new List<IUndoableCommand<TState>>();
		}

		#endregion

		#region Fields

		private TState initialState;

		#endregion
	}
}
