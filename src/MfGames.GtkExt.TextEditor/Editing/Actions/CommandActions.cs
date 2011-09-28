#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

#region Namespaces

using Gdk;

using MfGames.GtkExt.TextEditor.Models.Buffers;

#endregion

namespace MfGames.GtkExt.TextEditor.Editing.Actions
{
    /// <summary>
    /// Contains actions used for undoing and redoing commands.
    /// </summary>
    [ActionFixture]
    public class CommandActions
    {
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
            foreach (
                ILineBufferOperation operation in undoCommand.UndoOperations)
            {
                controller.Do(operation);
            }

            // Push the command on the redo stack.
            controller.Commands.RedoCommands.Push(undoCommand);

            // Move the caret and scroll to it.
            controller.DisplayContext.ScrollToCaret(undoCommand.StartPosition);
        }
    }
}