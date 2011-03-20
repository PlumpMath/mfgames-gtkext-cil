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
	public class MemoryLineBuffer : MultiplexedOperationLineBuffer
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

		/// <summary>
		/// Gets the length of the line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <param name="lineContexts">The line contexts.</param>
		/// <returns>The length of the line.</returns>
		public override int GetLineLength(
			int lineIndex,
			LineContexts lineContexts)
		{
			return lines[lineIndex].Length;
		}

		/// <summary>
		/// Gets the formatted line number for a given line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>A formatted line number.</returns>
		public override string GetLineNumber(int lineIndex)
		{
			// Line numbers are given as 1-based instead of 0-based.
			return (lineIndex + 1).ToString("N0");
		}

		/// <summary>
		/// Gets the line without any manipulations.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <returns></returns>
		public string GetLineText(int lineIndex)
		{
			return lines[lineIndex];
		}

		/// <summary>
		/// Gets the text of a given line in the buffer.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer. If the index is beyond the end of the buffer, the last line is used.</param>
		/// <param name="characters">The character range to pull the text.</param>
		/// <param name="lineContexts">The line contexts.</param>
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
		/// Performs the given operation on the line buffer. This will raise any
		/// events that were appropriate for the operation.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected override LineBufferOperationResults Do(
			InsertTextOperation operation)
		{
			// Get the text from the buffer, insert the text, and put it back.
			int lineIndex = operation.BufferPosition.LineIndex;
			string line = lines[lineIndex];
			int characterIndex = Math.Min(
				operation.BufferPosition.CharacterIndex, line.Length);

			string newLine = line.Insert(characterIndex, operation.Text);

			lines[lineIndex] = newLine;

			// Fire a line changed operation.
			RaiseLineChanged(new LineChangedArgs(lineIndex));

			// Return the appropriate results.
			return
				new LineBufferOperationResults(
					new BufferPosition(lineIndex, characterIndex + operation.Text.Length));
		}

		/// <summary>
		/// Deletes text from the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected override LineBufferOperationResults Do(
			DeleteTextOperation operation)
		{
			// Get the text from the buffer, insert the text, and put it back.
			int lineIndex = operation.LineIndex;
			string line = lines[lineIndex];
			int endCharacterIndex = Math.Min(
				operation.CharacterRange.EndIndex, line.Length);

			string newLine = line.Remove(
				operation.CharacterRange.StartIndex,
				endCharacterIndex - operation.CharacterRange.StartIndex);

			lines[lineIndex] = newLine;

			// Fire a line changed operation.
			RaiseLineChanged(new LineChangedArgs(lineIndex));

			// Return the appropriate results.
			return
				new LineBufferOperationResults(
					new BufferPosition(lineIndex, operation.CharacterRange.StartIndex));
		}

		/// <summary>
		/// Performs the set text operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected override LineBufferOperationResults Do(SetTextOperation operation)
		{
			// Set the text of the line.
			lines[operation.LineIndex] = operation.Text;

			// Fire a line changed operation.
			RaiseLineChanged(new LineChangedArgs(operation.LineIndex));

			// Return the appropriate results.
			return
				new LineBufferOperationResults(
					new BufferPosition(operation.LineIndex, lines[operation.LineIndex].Length));
		}

		/// <summary>
		/// Performs the delete lines operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected override LineBufferOperationResults Do(
			DeleteLinesOperation operation)
		{
			// Delete the lines from the buffer.
			lines.RemoveRange(operation.LineIndex, operation.Count);

			// Fire an delete line change.
			RaiseLinesDeleted(
				new LineRangeEventArgs(operation.LineIndex, operation.Count));

			// Return the appropriate results.
			return
				new LineBufferOperationResults(new BufferPosition(operation.LineIndex, 0));
		}

		/// <summary>
		/// Performs the insert lines operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected override LineBufferOperationResults Do(
			InsertLinesOperation operation)
		{
			// Insert the new lines into the buffer.
			for (int index = 0; index < operation.Count; index++)
			{
				lines.Insert(operation.LineIndex, string.Empty);
			}

			// Fire an insert line change.
			RaiseLinesInserted(
				new LineRangeEventArgs(operation.LineIndex, operation.Count));

			// Return the appropriate results.
			return
				new LineBufferOperationResults(
					new BufferPosition(operation.LineIndex + operation.Count, 0));
		}

		#endregion
	}
}