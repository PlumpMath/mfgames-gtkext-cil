#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
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
using System.Diagnostics;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Represents an operation to insert text into the buffer. Unlike
	/// <see cref="SetTextOperation"/>, this inserts text into a specific position
	/// and returns the buffer position for the end of the insert.
	/// </summary>
	public class InsertTextOperation : ILineBufferOperation
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="InsertTextOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="characterIndex">Index of the character.</param>
		/// <param name="text">The text.</param>
		public InsertTextOperation(
			int lineIndex,
			int characterIndex,
			string text)
			: this(new BufferPosition(lineIndex, characterIndex), text)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InsertTextOperation"/> class.
		/// </summary>
		/// <param name="bufferPosition">The buffer position.</param>
		/// <param name="text">The text.</param>
		public InsertTextOperation(
			BufferPosition bufferPosition,
			string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			BufferPosition = bufferPosition;
			Text = text;
		}

		#endregion

		#region Operation

		/// <summary>
		/// Gets or sets the buffer position for the insert operation.
		/// </summary>
		/// <value>The buffer position.</value>
		public BufferPosition BufferPosition { get; private set; }

		/// <summary>
		/// Gets the type of the operation representing this object.
		/// </summary>
		/// <value>The type of the operation.</value>
		public LineBufferOperationType OperationType
		{
			[DebuggerStepThrough]
			get { return LineBufferOperationType.InsertText; }
		}

		/// <summary>
		/// Gets the text for this operation.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		#endregion
	}
}