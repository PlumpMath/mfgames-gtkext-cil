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

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	public class MemoryLineBuffer : ILineBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryLineBuffer"/> class.
		/// </summary>
		public MemoryLineBuffer()
		{
			lines = new List<string>();
		}

		/// <summary>
		/// Copies the given buffer into a memory buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public MemoryLineBuffer(ILineBuffer buffer)
		{
			int lineCount = buffer.LineCount;

			lines = new List<string>(lineCount);

			for (int line = 0; line < lineCount; line++)
			{
				lines.Add(buffer.GetLineText(line, 0, -1));
			}
		}

		#endregion

		#region Buffer Viewing

		private readonly List<string> lines;

		public int LineCount
		{
			get { return lines.Count; }
		}

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		public bool ReadOnly { get; set; }

		public int GetLineLength(int line)
		{
			return lines[line].Length;
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
			// Get the entire line from the buffer. This will throw the argument
			// out of range exception if the line can't be found.
			string text = lines[line];

			// If we are from 0 to -1, then we want the entire string and don't
			// need to do anything else.
			if (startIndex == 0 && endIndex == -1)
			{
				return text;
			}

			// Make sure the indexes are normalized. The end index can be -1 which
			// means the end of the string.
			if (endIndex == -1)
			{
				endIndex = text.Length;
			}

			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}

			if (startIndex > endIndex)
			{
				throw new ArgumentOutOfRangeException(
					"endIndex cannot be before startIndex");
			}

			// Substrings use lengths, not end indexes.
			int length = endIndex - startIndex;

			// Return the substring of the text based on indexes.
			return text.Substring(startIndex, length);
		}

		#endregion

		#region Buffer Editing

		public void DeleteLines(
			int startLine,
			int endLine)
		{
			if (endLine == -1)
			{
				endLine = lines.Count - 1;
			}

			if (startLine > endLine)
			{
				throw new ArgumentOutOfRangeException("endLine cannot be before startLine");
			}

			int count = endLine - startLine;
			lines.RemoveRange(startLine, count);
		}

		public void InsertLines(
			int afterLine,
			int count)
		{
			if (afterLine == -1)
			{
				afterLine = lines.Count;
			}

			for (int index = 0; index < count; index++)
			{
				lines.Insert(afterLine, string.Empty);
			}
		}

		public void SetLineText(
			int line,
			int startIndex,
			int endIndex,
			string text)
		{
			// If we don't have a valid text, throw an exception.
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			// Get the entire line from the buffer. This will throw the argument
			// out of range exception if the line can't be found.
			string lineText = lines[line];

			// If we are from 0 to -1, then we want to replace the entire string.
			if (startIndex == 0 && endIndex == -1)
			{
				lines[line] = text;
				return;
			}

			// Make sure the indexes are normalized. The end index can be -1 which
			// means the end of the string.
			if (endIndex == -1)
			{
				endIndex = lineText.Length;
			}

			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}

			if (startIndex > endIndex)
			{
				throw new ArgumentOutOfRangeException(
					"endIndex cannot be before startIndex");
			}

			// Break out the components of the intial line before and after the
			// indexes.
			string before = lineText.Substring(0, startIndex);
			string after = lineText.Substring(endIndex);

			// Add all three together and put them back into the array.
			lines[line] = before + text + after;
		}

		#endregion
	}
}