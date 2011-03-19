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
using System.Collections.Generic;

using MfGames.GtkExt.TextEditor.Models.Buffers;

#endregion

namespace MfGames.GtkExt.TextEditor.Models
{
	/// <summary>
	/// Implements a line buffer that keeps all the lines in memory.
	/// </summary>
	public class MemoryLineBuffer : LineBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryLineBuffer"/> class.
		/// </summary>
		public MemoryLineBuffer()
		{
			lines = new List<string>();
			lines.Add(string.Empty);
		}

		/// <summary>
		/// Copies the given buffer into a memory buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public MemoryLineBuffer(LineBuffer buffer)
		{
			int lineCount = buffer.LineCount;

			lines = new List<string>(lineCount);

			for (int line = 0; line < lineCount; line++)
			{
				lines.Add(
					buffer.GetLineText(line, new CharacterRange(0), LineContexts.None));
			}
		}

		#endregion

		#region Buffer Viewing

		private readonly List<string> lines;

		private bool readOnly;

		/// <summary>
		/// Gets the line count.
		/// </summary>
		/// <value>The line count.</value>
		public override int LineCount
		{
			get { return lines.Count; }
		}

		/// <summary>
		/// If set to <see langword="true"/>, the buffer is read-only and the editing commands
		/// should throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public override bool ReadOnly
		{
			get { return readOnly; }
		}

		public override int GetLineLength(
			int lineIndex,
			LineContexts lineContexts)
		{
			return lines[lineIndex].Length;
		}

		public override string GetLineNumber(int lineIndex)
		{
			// Line numbers are given as 1-based instead of 0-based.
			return (lineIndex + 1).ToString("N0");
		}

		/// <summary>
		/// Gets the text of a given line in the buffer.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer. If the index is beyond the end of the buffer, the last line is used.</param>
		/// <param name="characters">The character range to pull the text.</param>
		/// <returns></returns>
		public override string GetLineText(
			int lineIndex,
			CharacterRange characters,
			LineContexts lineContexts)
		{
			string text = lines[lineIndex];

			return characters.Substring(text);
		}

		/// <summary>
		/// Sets the read only flag on the buffer.
		/// </summary>
		/// <param name="newReadOnly">if set to <c>true</c> [read only].</param>
		public void SetReadOnly(bool newReadOnly)
		{
			readOnly = newReadOnly;
		}

		#endregion

		#region Buffer Operations

		/// <summary>
		/// Performs the given operation, raising any events for changing.
		/// </summary>
		/// <param name="operation">The operation.</param>
		public override void Do(ILineBufferOperation operation)
		{
			// Figure out what to do based on the operation.
			switch (operation.OperationType)
			{
				case LineBufferOperationType.SetText:
					// Pull out the text operation.
					var setTextOperation = (SetTextOperation) operation;

					// Set the text of the line.
					lines[setTextOperation.LineIndex] = setTextOperation.Text;

					// Fire a line changed operation.
					RaiseLineChanged(new LineChangedArgs(setTextOperation.LineIndex));
					break;

				case LineBufferOperationType.InsertLines:
					// Pull out the insert operation.
					var insertLinesOperation = (InsertLinesOperation) operation;

					// Insert the new lines into the buffer.
					for (int index = 0; index < insertLinesOperation.Count; index++)
					{
						lines.Insert(insertLinesOperation.LineIndex, string.Empty);
					}

					// Fire an insert line change.
					RaiseLinesInserted(
						new LineRangeEventArgs(
							insertLinesOperation.LineIndex, insertLinesOperation.Count));
					break;

				case LineBufferOperationType.DeleteLines:
					// Pull out the delete operation.
					var deleteLinesOperation = (DeleteLinesOperation) operation;

					// Delete the lines from the buffer.
					lines.RemoveRange(
						deleteLinesOperation.LineIndex, deleteLinesOperation.Count);

					// Fire an delete line change.
					RaiseLinesDeleted(
						new LineRangeEventArgs(
							deleteLinesOperation.LineIndex, deleteLinesOperation.Count));
					break;
			}
		}

		#endregion
	}
}