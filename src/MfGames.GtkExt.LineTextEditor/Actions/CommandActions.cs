using Gdk;

using MfGames.GtkExt.LineTextEditor.Attributes;
using MfGames.GtkExt.LineTextEditor.Commands;
using MfGames.GtkExt.LineTextEditor.Interfaces;

namespace MfGames.GtkExt.LineTextEditor.Actions
{
    /// <summary>
    /// Contains actions used for undoing and redoing commands.
    /// </summary>
    [ActionFixture]
    public class CommandActions
    {
        /// <summary>
        /// Undoes the last command.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        [Action]
        [KeyBinding(Key.Z, ModifierType.ControlMask)]
        public static void Undo(IActionContext actionContext)
        {
            // Check to see if there an undo command.
            if (actionContext.Commands.UndoCommands.Count <= 0)
            {
                return;
            }

            // Get the undo command from the stack.
            Command undoCommand = actionContext.Commands.UndoCommands.Pop();

            // Loop through the undo commands and perform them.
            foreach (var operation in undoCommand.UndoOperations)
            {
                actionContext.Do(operation);
            }

            // Push the command on the redo stack.
            actionContext.Commands.RedoCommands.Push(undoCommand);
        }

        /// <summary>
        /// Redoes the specified action context.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        [Action]
        [KeyBinding(Key.Y, ModifierType.ControlMask)]
        public static void Redo(IActionContext actionContext)
        {
            // Check to see if there an redo command.
            if (actionContext.Commands.RedoCommands.Count <= 0)
            {
                return;
            }

            // Get the redo command from the stack.
            Command redoCommand = actionContext.Commands.RedoCommands.Pop();

            // Loop through the operations and perform them.
            foreach (var operation in redoCommand.Operations)
            {
                actionContext.Do(operation);
            }

            // Push the command on the redo stack.
            actionContext.Commands.UndoCommands.Push(redoCommand);
        }
    }
}