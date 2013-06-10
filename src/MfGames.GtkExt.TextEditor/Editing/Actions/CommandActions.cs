// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gdk;
using MfGames.GtkExt.TextEditor.Models.Buffers;

namespace MfGames.GtkExt.TextEditor.Editing.Actions
{
	/// <summary>
	/// Contains actions used for undoing and redoing commands.
	/// </summary>
	[ActionFixture]
	public class CommandActions
	{
		#region Methods

		/// <summary>
		/// Redoes the specified action context.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.Y, ModifierType.ControlMask)]
		public static void Redo(EditorViewController controller)
		{
			// Check to see if there an redo command.
			if (controller.Commands.RedoCommands.Count <= 0)
			{
				return;
			}

			// Get the redo command from the stack.
			Command redoCommand = controller.Commands.RedoCommands.Pop();

			// Loop through the operations and perform them.
			foreach (ILineBufferOperation operation in redoCommand.Operations)
			{
				controller.Do(operation);
			}

			// Push the command on the redo stack.
			controller.Commands.UndoCommands.Push(redoCommand);

			// Move the caret and scroll to it.
			controller.DisplayContext.ScrollToCaret(redoCommand.EndPosition);
		}

		/// <summary>
		/// Undoes the last command.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.Z, ModifierType.ControlMask)]
		public static void Undo(EditorViewController controller)
		{
			// Check to see if there an undo command.
			if (controller.Commands.UndoCommands.Count <= 0)
			{
				return;
			}

			// Get the undo command from the stack.
			Command undoCommand = controller.Commands.UndoCommands.Pop();

			// Loop through the undo commands and perform them.
			foreach (ILineBufferOperation operation in undoCommand.UndoOperations)
			{
				controller.Do(operation);
			}

			// Push the command on the redo stack.
			controller.Commands.RedoCommands.Push(undoCommand);

			// Move the caret and scroll to it.
			controller.DisplayContext.ScrollToCaret(undoCommand.StartPosition);
		}

		#endregion
	}
}
