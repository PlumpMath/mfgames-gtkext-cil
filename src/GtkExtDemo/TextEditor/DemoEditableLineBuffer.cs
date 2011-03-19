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
		/// Performs the set text operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected override LineBufferOperationResults Do(SetTextOperation operation)
		{
			// If the text is at least a given length, make additional changes.
			if (operation.Text.Length >= 2 && operation.Text.Substring(1, 1) == ":")
			{
				switch (Char.ToUpper(operation.Text[0]))
				{
					case 'D':
						styles.Remove(operation.LineIndex);
						break;

					case 'H':
						styles[operation.LineIndex] = DemoLineStyleType.Heading;
						break;
				}
			}

			// Return the results of the base set.
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
				    (lineContexts & LineContexts.CurrentLine) == 0 &&
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
				    (lineContexts & LineContexts.CurrentLine) == 0 &&
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