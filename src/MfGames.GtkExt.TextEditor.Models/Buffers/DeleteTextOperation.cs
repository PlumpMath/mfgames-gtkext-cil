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
	/// <see cref="SetTextOperation"/>, this deletes text at a specific position
	/// and returns the buffer position for resulting position.
	/// </summary>
	public class DeleteTextOperation : ILineBufferOperation
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteTextOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="characterRange">The character range.</param>
		public DeleteTextOperation(
			int lineIndex,
			CharacterRange characterRange)
		{
			if (lineIndex < 0)
			{
				throw new ArgumentOutOfRangeException(
					"lineIndex", "Line index cannot be negative.");
			}

			LineIndex = lineIndex;
			CharacterRange = characterRange;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteTextOperation"/> class.
		/// </summary>
		/// <param name="bufferPosition">The buffer position.</param>
		/// <param name="length">The length.</param>
		public DeleteTextOperation(
			BufferPosition bufferPosition,
			int length)
		{
			LineIndex = bufferPosition.LineIndex;
			CharacterRange = CharacterRange.FromLength(
				bufferPosition.CharacterIndex, length);
		}

		#endregion

		#region Operation

		/// <summary>
		/// Gets or sets the character range to delete.
		/// </summary>
		/// <value>The character range.</value>
		public CharacterRange CharacterRange { get; set; }

		/// <summary>
		/// Gets or sets the index of the line.
		/// </summary>
		/// <value>The index of the line.</value>
		public int LineIndex { get; set; }

		/// <summary>
		/// Gets the type of the operation representing this object.
		/// </summary>
		/// <value>The type of the operation.</value>
		public LineBufferOperationType OperationType
		{
			[DebuggerStepThrough]
			get { return LineBufferOperationType.DeleteText; }
		}

		#endregion
	}
}