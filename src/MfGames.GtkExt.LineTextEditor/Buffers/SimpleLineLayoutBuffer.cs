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

using Pango;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a basic line layout buffer that performs no caching or
	/// additional formatting beyond a simple font.
	/// </summary>
	public class SimpleLineLayoutBuffer : ILineLayoutBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleLineLayoutBuffer"/> class.
		/// </summary>
		/// <param name="lineBuffer">The line buffer.</param>
		public SimpleLineLayoutBuffer(ILineBuffer lineBuffer)
			: this(new UnformattedLineMarkupBuffer(lineBuffer))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleLineLayoutBuffer"/> class.
		/// </summary>
		/// <param name="lineMarkupBuffer">The line markup buffer.</param>
		public SimpleLineLayoutBuffer(ILineMarkupBuffer lineMarkupBuffer)
		{
			// Save the inner buffer.
			if (lineMarkupBuffer == null)
			{
				throw new ArgumentNullException("lineMarkupBuffer");
			}

			buffer = lineMarkupBuffer;
		}

		#endregion

		#region Style

		private Context context;
		private Layout layout;

		/// <summary>
		/// Gets or sets the context for the layout.
		/// </summary>
		/// <value>The context.</value>
		public Context Context
		{
			get { return context; }
			set
			{
				context = value;
				Reset();
			}
		}

		/// <summary>
		/// Ensures the layout exists by creating it if needed.
		/// </summary>
		private void EnsureLayoutExists()
		{
			// If the layout already exists, then we don't need to re-recreate.
			if (layout != null)
			{
				return;
			}

			// Create a new layout from the given properties.
			layout = new Layout(Context);
			layout.Width = Units.FromPixels(Width);
			layout.Wrap = WrapMode.Word;
			layout.Alignment = Alignment.Left;
			//layout.FontDescription = FontDescription.FromString("Courier New 12");
		}

		#endregion

		#region Inner Buffer

		private readonly ILineMarkupBuffer buffer;

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
			return buffer.GetLineMarkup(line);
		}

		#endregion

		#region Layout

		private int lastLine = -1;

		/// <summary>
		/// Sets the pixel width of a layout in case the layout uses word
		/// wrapping.
		/// </summary>
		/// <value>The width.</value>
		public int Width { get; set; }

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		public Layout GetLineLayout(int line)
		{
			// Check to see if the last line was the one requested.
			if (line == lastLine)
			{
				return layout;
			}

			// Make sure the layout exists and is properly setup.
			EnsureLayoutExists();

			// Set the markup for the layout.
			layout.SetMarkup(GetLineMarkup(line));
			lastLine = line;

			// Return the resulting layout.
			return layout;
		}

		/// <summary>
		/// Resets the layout operations.
		/// </summary>
		public void Reset()
		{
			layout = null;
			lastLine = -1;
		}

		#endregion
	}
}