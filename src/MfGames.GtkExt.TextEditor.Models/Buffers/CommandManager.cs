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

using System;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
    /// <summary>
    /// Implements a manager for commands that handles undo and redo functionality.
    /// </summary>
    public class CommandManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandManager()
        {
            RedoCommands = new CommandCollection();
            UndoCommands = new CommandCollection();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the redo commands.
        /// </summary>
        public CommandCollection RedoCommands { get; private set; }

        /// <summary>
        /// Gets the undo commands.
        /// </summary>
        public CommandCollection UndoCommands { get; private set; }

        #endregion

        /// <summary>
        /// Adds the specified command to the command manager.
        /// </summary>
        /// <param name="command">The command.</param>
        public void Add(Command command)
        {
            // Throw an exception if there are no command.
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            // Adding a command automatically purges the redo list.
            RedoCommands.Clear();

            // Check to see if there are any undo commands. If there isn't
            // any undo commands, then we don't add it to the undo list.
            if (command.UndoOperations.Count > 0)
            {
                UndoCommands.Push(command);
            }
        }
    }
}