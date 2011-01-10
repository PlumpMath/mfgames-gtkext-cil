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

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
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
		public PatternLineBuffer(
			int lines,
			int width)
		{
			this.lines = lines;
			this.width = width;
		}

		#endregion

		#region Buffer Viewing

		private static readonly string[] Words = new[]
		                                         { "one", "two", "three", "four", };

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

		public int GetLineLength(int line)
		{
			return GetLineText(line, 0, -1).Length;
		}

		public string GetLineNumber(int line)
		{
			// Line numebers are given as 1-based instead of 0-based.
			return (line + 1).ToString("N0");
		}

		public string GetLineText(
			int line,
			int startIndex,
			int endIndex)
		{
			// Build up a string buffer with the line text. This will always
			// be no more than the width of the line.
			var buffer = new StringBuilder();
			int index = line % Words.Length;

			while (buffer.Length < width)
			{
				// Get the next word we'll be adding to the line.
				string nextWord = Words[index];
				index = (index + 1) % Words.Length;

				// Check to see if the selected word would bring us beyond the
				// limit.
				if (buffer.Length + nextWord.Length > width)
				{
					break;
				}

				// Append the word to the string along with a space.
				buffer.Append(nextWord);
				buffer.Append(" ");
			}

			// Trim off the trailing space and return it.
			return buffer.ToString().Trim();
		}

		#endregion

		#region Buffer Editing

		public void DeleteLines(
			int startLine,
			int endLine)
		{
			throw new InvalidOperationException();
		}

		public void InsertLines(
			int afterLine,
			int count)
		{
			throw new InvalidOperationException();
		}

		public void SetLineText(
			int line,
			int startIndex,
			int endIndex,
			string text)
		{
			throw new InvalidOperationException();
		}

		#endregion
	}
}