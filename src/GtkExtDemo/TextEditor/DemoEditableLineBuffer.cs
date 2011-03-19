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

using C5;

using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;

#endregion

namespace GtkExtDemo.TextEditor
{
	/// <summary>
	/// Implements a demo editable buffer that is intended to be styled and
	/// formatted.
	/// </summary>
	public class DemoEditableLineBuffer : MemoryLineBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoEditableLineBuffer"/> class.
		/// </summary>
		public DemoEditableLineBuffer()
		{
			// Set up the line styles.
			styles = new HashDictionary<int, DemoLineStyleType>();

			// Create the initial lines. There is already one in the buffer before
			// this insert operates.
			InsertLines(0, 4);

			// Set the text on the lines with the prefix so they can be styled
			// as part of the set operation.
			SetText(0, "H: Heading Line");
			SetText(1, "D: Regular Text");
			SetText(2, "H:");
			SetText(3, "D: Regular Text");
			SetText(4, "D: Regular Text");
		}

		#endregion

		#region Operations

		/// <summary>
		/// Checks to see if a line operation caused a style to change.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="results">The results.</param>
		/// <returns></returns>
		private LineBufferOperationResults CheckForStyleChanged(
			int lineIndex,
			LineBufferOperationResults results)
		{
			// Look to see if the line starts with a style change keyword.
			string line = GetLineText(lineIndex);

			if (line.Length < 2 || line.Substring(1, 1) != ":")
			{
				// We don't have a style change, so just return the results.
				return results;
			}

			// Check to see if we have a style change prefix.
			bool changed = false;

			switch (Char.ToUpper(line[0]))
			{
				case 'D':
					styles.Remove(lineIndex);
					changed = true;
					break;

				case 'H':
					styles[lineIndex] = DemoLineStyleType.Heading;
					changed = true;
					break;
			}

			// If we didn't change anything, then just return the unaltered
			// results.
			if (!changed)
			{
				return results;
			}

			// Figure out what the line would look like without the prefix.
			string newLine = line.Substring(2).TrimStart(' ');
			int difference = line.Length - newLine.Length;

			// Set the line text.
			SetText(lineIndex, newLine);

			// Adjust the buffer position and return it.
			results.BufferPosition = new BufferPosition(
				results.BufferPosition.LineIndex,
				Math.Max(0, results.BufferPosition.CharacterIndex - difference));

			return results;
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
			LineBufferOperationResults results = base.Do(operation);

			return CheckForStyleChanged(operation.LineIndex, results);
		}

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
			LineBufferOperationResults results = base.Do(operation);

			return CheckForStyleChanged(operation.BufferPosition.LineIndex, results);
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
			LineBufferOperationResults results = base.Do(operation);

			return CheckForStyleChanged(operation.LineIndex, results);
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
			// First shift the style lines up for the new ones. We go from the
			// bottom to avoid overlapping the line numbers.
			for (int lineIndex = LineCount - 1; lineIndex >= 0; lineIndex--)
			{
				// If we have a key, shift it.
				if (lineIndex >= operation.LineIndex && styles.Contains(lineIndex))
				{
					styles[lineIndex + operation.Count] = styles[lineIndex];
					styles.Remove(lineIndex);
				}
			}

			// Now, perform the operation on the buffer.
			return base.Do(operation);
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
			// Shift the styles down for the deleted lines.
			for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
			{
				// If we have a key, shift it.
				if (lineIndex >= operation.LineIndex && styles.Contains(lineIndex))
				{
					styles[lineIndex - operation.Count] = styles[lineIndex];
					styles.Remove(lineIndex);
				}
			}

			// Now, perform the operation on the buffer.
			return base.Do(operation);
		}

		#endregion

		#region Buffers

		private readonly HashDictionary<int, DemoLineStyleType> styles;

		/// <summary>
		/// Gets the name of the line style based on the settings.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or
		/// Int32.MaxValue for the last line.</param>
		/// <param name="lineContexts">The line contexts.</param>
		/// <returns></returns>
		public override string GetLineStyleName(
			int lineIndex,
			LineContexts lineContexts)
		{
			// See if we have the line in the styles.
			if (styles.Contains(lineIndex))
			{
				// If this is a heading line, and it has no value, and it is
				// not the current line, we color it differently to make it
				// obvious we are adding dynamic data.
				DemoLineStyleType lineType = styles[lineIndex];

				if (lineType == DemoLineStyleType.Heading &&
				    base.GetLineLength(lineIndex, LineContexts.None) == 0)
				{
					return "Inactive Heading";
				}

				// Otherwise, return the normal style name.
				return lineType.ToString();
			}

			return DemoLineStyleType.Default.ToString();
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
			// See if we have the line in the styles.
			if (styles.Contains(lineIndex))
			{
				// If this is a heading line, and it has no value, and it is
				// not the current line, we put in different text for a
				// placeholder.
				DemoLineStyleType lineType = styles[lineIndex];

				if (lineType == DemoLineStyleType.Heading &&
				    base.GetLineLength(lineIndex, LineContexts.None) == 0)
				{
					return "<Heading>";
				}
			}

			// We don't have a special case, so just return the base.
			return base.GetLineText(lineIndex, characters, lineContexts);
		}

		#endregion
	}
}