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

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Creates a line markup that does no formatting on a inner line object.
	/// </summary>
	public class UnformattedLineMarkupBuffer : ILineMarkupBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UnformattedLineMarkupBuffer"/> class.
		/// </summary>
		/// <param name="lineBuffer">The line buffer.</param>
		public UnformattedLineMarkupBuffer(ILineBuffer lineBuffer)
		{
			if (lineBuffer == null)
			{
				throw new ArgumentNullException("lineBuffer");
			}

			this.buffer = lineBuffer;
		}

		#endregion

		#region Inner Buffer

		private readonly ILineBuffer buffer;

		#endregion

		#region Buffer Viewing

		public int LineCount
		{
			get { return buffer.LineCount; }
		}

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		public bool ReadOnly
		{
			get { return buffer.ReadOnly; }
		}

		public int GetLineLength(int line)
		{
			return buffer.GetLineLength(line);
		}

		public string GetLineNumber(int line)
		{
			return buffer.GetLineNumber(line);
		}

		public string GetLineText(
			int line,
			int startIndex,
			int endIndex)
		{
			return buffer.GetLineText(line, startIndex, endIndex);
		}

		#endregion

		#region Buffer Editing

		public void DeleteLines(
			int startLine,
			int endLine)
		{
			buffer.DeleteLines(startLine, endLine);
		}

		public void InsertLines(
			int afterLine,
			int count)
		{
			buffer.InsertLines(afterLine, count);
		}

		public void SetLineText(
			int line,
			int startIndex,
			int endIndex,
			string text)
		{
			buffer.SetLineText(line, startIndex, endIndex, text);
		}

		#endregion

		#region Markup

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		public string GetLineMarkup(int line)
		{
			string text = GetLineText(line, 0, -1);

			return text
				.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}

		#endregion
	}
}