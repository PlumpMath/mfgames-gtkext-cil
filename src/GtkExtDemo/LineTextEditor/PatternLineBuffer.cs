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
using System.Text;

using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace GtkExtDemo.LineTextEditor
{
	/// <summary>
	/// Implements a line buffer that produces a pattern of text which is suitable
	/// for debugging or displaying the results of the line buffer.
	/// </summary>
	public class PatternLineBuffer : ILineBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternLineBuffer"/> class.
		/// </summary>
		/// <param name="lines">The lines.</param>
		/// <param name="width">The width.</param>
		/// <param name="divisor">The divisor which is used to create different lengths of lines.</param>
		public PatternLineBuffer(
			int lines,
			int width,
			int divisor)
		{
			this.lines = lines;
			this.width = width;
			this.divisor = divisor;
		}

		#endregion

		#region Buffer Viewing

		private static readonly string[] Words = new[]
		                                         { "one", "two", "three", "four", };

		private readonly int divisor;

		private readonly int lines;
		private readonly int width;

		/// <summary>
		/// Gets the line count.
		/// </summary>
		/// <value>The line count.</value>
		public int LineCount
		{
			get { return lines; }
		}

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		public bool ReadOnly
		{
			get { return true; }
		}

		public int GetLineLength(int lineIndex)
		{
			return GetLineText(lineIndex, 0, Int32.MaxValue).Length;
		}

		public string GetLineNumber(int lineIndex)
		{
			// Line numebers are given as 1-based instead of 0-based.
			return (lineIndex + 1).ToString("N0");
		}

		public string GetLineText(
			int lineIndex,
			int startIndex,
			int endIndex)
		{
			// Build up a string buffer with the line text. This will always
			// be no more than the width of the line.
			var buffer = new StringBuilder();

			buffer.AppendFormat("Line {0:N0}: ", lineIndex + 1);

			// Create a patterned line with a varying length.
			int index = lineIndex % Words.Length;
			int lineWidth = width / (1 + lineIndex % divisor);

			while (buffer.Length < lineWidth)
			{
				// Get the next word we'll be adding to the line.
				string nextWord = Words[index];
				index = (index + 1) % Words.Length;

				// Scatter in some warnings and errors.
				if (index == 0 && (lineIndex % 229) == 0)
				{
					nextWord = "error";
				}

				if (index == 0 && (lineIndex % 113) == 0)
				{
					nextWord = "warning";
				}

				// Append the word to the string along with a space.
				buffer.Append(nextWord);
				buffer.Append(" ");
			}

			// Trim off the trailing space and return it.
			return buffer.ToString().Trim();
		}

		#endregion

		#region Buffer Operations

		/// <summary>
		/// Performs the given operation, raising any events for changing.
		/// </summary>
		/// <param name="operation">The operation.</param>
		public virtual void Do(ILineBufferOperation operation)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Fires the line changed event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The args.</param>
		private void FireLineChanged(
			object sender,
			LineChangedArgs args)
		{
			if (LineChanged != null)
			{
				LineChanged(sender, args);
			}
		}

		/// <summary>
		/// Fires the lines deleted event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The args.</param>
		private void FireLinesDeleted(
			object sender,
			LineRangeEventArgs args)
		{
			if (LinesDeleted != null)
			{
				LinesDeleted(sender, args);
			}
		}

		/// <summary>
		/// Fires the lines inserted event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The args.</param>
		private void FireLinesInserted(
			object sender,
			LineRangeEventArgs args)
		{
			if (LinesInserted != null)
			{
				LinesInserted(sender, args);
			}
		}

		/// <summary>
		/// Used to indicate that a line changed.
		/// </summary>
		public event EventHandler<LineChangedArgs> LineChanged;

		/// <summary>
		/// Occurs when lines are inserted into the buffer.
		/// </summary>
		public event EventHandler<LineRangeEventArgs> LinesDeleted;

		/// <summary>
		/// Occurs when lines are inserted into the buffer.
		/// </summary>
		public event EventHandler<LineRangeEventArgs> LinesInserted;

		#endregion
	}
}