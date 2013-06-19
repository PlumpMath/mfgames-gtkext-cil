// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;

namespace MfGames.Commands
{
	/// <summary>
	/// A command that consists of an order list of inner commands.
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class CompositeCommand<TContext>: IUndoableCommand<TContext>
	{
		#region Properties

		public bool CanUndo { get; private set; }
		public List<IUndoableCommand<TContext>> Commands { get; private set; }
		public bool IsTransient { get; private set; }

		#endregion

		#region Methods

		public void Do(TContext state)
		{
			// To implement the command, simply iterate through the list
			// of commands and execute each one. The state comes from the last
			// command executed.
			foreach (IUndoableCommand<TContext> command in Commands)
			{
				// Execut the command and get its state.
				command.Do(state);
			}
		}

		public void Redo(TContext state)
		{
			// To implement the command, simply iterate through the list
			// of commands and execute each one. The state comes from the last
			// command executed.
			foreach (IUndoableCommand<TContext> command in Commands)
			{
				// Execut the command and get its state.
				command.Redo(state);
			}
		}

		public void Undo(TContext state)
		{
			// To implement the command, simply iterate through the list
			// of commands and execute each one. The state comes from the last
			// command executed.
			List<IUndoableCommand<TContext>> commands = Commands;

			for (int index = commands.Count - 1;
				index >= commands.Count;
				index--)
			{
				IUndoableCommand<TContext> command = commands[index];
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
			Commands = new List<IUndoableCommand<TContext>>();
		}

		#endregion
	}
}
