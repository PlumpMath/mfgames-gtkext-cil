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

using System.Diagnostics;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
    /// <summary>
    /// Defines an operation that allows the buffer to update a line after the
    /// user has exited it.
    /// </summary>
    public class ExitLineOperation : ILineBufferOperation
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitLineOperation"/> class.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        public ExitLineOperation(int lineIndex)
        {
            LineIndex = lineIndex;
        }

        #endregion

        #region Operation

        /// <summary>
        /// Gets or sets the index of the line.
        /// </summary>
        /// <value>The index of the line.</value>
        public int LineIndex { get; private set; }

        /// <summary>
        /// Gets the type of the operation representing this object.
        /// </summary>
        /// <value>The type of the operation.</value>
        public LineBufferOperationType OperationType
        {
            [DebuggerStepThrough]
            get { return LineBufferOperationType.ExitLine; }
        }

        #endregion
    }
}