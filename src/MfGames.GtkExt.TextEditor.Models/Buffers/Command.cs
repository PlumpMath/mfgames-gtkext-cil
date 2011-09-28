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

using C5;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
    /// <summary>
    /// Implements a command, which is a collection of operations both
    /// to perform the command and undo it.
    /// </summary>
    public class Command
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public Command()
        {
            Operations = new ArrayList<ILineBufferOperation>();
            UndoOperations = new ArrayList<ILineBufferOperation>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        public Command(BufferPosition startPosition)
            : this()
        {
            EndPosition = StartPosition = startPosition;
        }

        #endregion

        #region Operations

        /// <summary>
        /// Gets or sets the position at the end of the operations.
        /// </summary>
        /// <value>The end position.</value>
        public BufferPosition EndPosition { get; set; }

        /// <summary>
        /// Gets the operations for the command.
        /// </summary>
        public ArrayList<ILineBufferOperation> Operations { get; private set; }

        /// <summary>
        /// Gets or sets the position for the start of the operations.
        /// </summary>
        /// <value>The operation position.</value>
        public BufferPosition StartPosition { get; set; }

        /// <summary>
        /// Gets the undo operations for this command.
        /// </summary>
        public ArrayList<ILineBufferOperation> UndoOperations { get; private set; }

        #endregion
    }
}