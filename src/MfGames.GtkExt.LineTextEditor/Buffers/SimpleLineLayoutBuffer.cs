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

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a basic line layout buffer that performs no caching or
	/// additional formatting beyond a simple font.
	/// </summary>
	public class SimpleLineLayoutBuffer : LineMarkupBufferProxy, ILineLayoutBuffer
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
			: base(lineMarkupBuffer)
		{
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
				layout = null;
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
			int textWidth = Units.FromPixels(Width);
			layout.Width = textWidth;
			layout.Wrap = WrapMode.Word;
			layout.Alignment = Alignment.Left;
			//layout.FontDescription = FontDescription.FromString("Courier New 12");
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
		/// <param name="textEditor">The text editor.</param>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		public Layout GetLineLayout(
			TextEditor textEditor,
			int line)
		{
			// Check to see if the last line was the one requested.
			if (line == lastLine && layout != null)
			{
				return layout;
			}

			// Make sure the layout exists and is properly setup.
			EnsureLayoutExists();

			// Set the markup for the layout.
			layout.SetMarkup(GetLineMarkup(line));
			lastLine = line;

			// Set the style to the default line.
			textEditor.Theme.BlockStyles[Theme.TextStyle].SetLayout(layout);

			// Return the resulting layout.
			return layout;
		}

		/// <summary>
		/// Gets the height of a single line layout.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="textEditor">The text editor.</param>
		/// <returns></returns>
		private int GetLineLayoutHeight(
			TextEditor textEditor,
			int line)
		{
			layout = null;
			Context = textEditor.PangoContext;
			Layout lineLayout = GetLineLayout(textEditor, line);

			// Get the extents for the line while rendered.
			int lineWidth, lineHeight;
			lineLayout.GetPixelSize(out lineWidth, out lineHeight);
			return lineHeight;
		}

		/// <summary>
		/// Gets the lines that are visible in the given view area.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="viewArea">The view area.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		public void GetLineLayoutRange(
			TextEditor textEditor,
			Rectangle viewArea,
			out int startLine,
			out int endLine)
		{
			// Reset the start line to negative to indicate we don't have a
			// visible line yet.
			startLine = -1;

			// Go through all the lines until we find one that is in the range.
			int currentY = 0;

			for (int line = 0; line < LineCount; line++)
			{
				// Get the height for this line.
				int height = GetLineLayoutHeight(textEditor, line);

				// If we don't have a starting line, then check to see if this
				// line is visible.
				if (startLine < 0)
				{
					if (currentY + height >= viewArea.Y)
					{
						startLine = line;
					}
				}

				// Set the end line equal to the current line so it moves forward
				// until we break out.
				endLine = line;

				// Add the height to the current Y offset. If it exceeds the
				// viewport, then we are done.
				currentY += height;

				if (currentY > viewArea.Y + viewArea.Height)
				{
					// We are done, so return the values.
					return;
				}
			}

			// If we got this far, nothing is visible.
			startLine = endLine = 0;
			return;
		}

		/// <summary>
		/// Gets the pixel height of the lines in the buffer. If endLine is -1
		/// it means the last line in the buffer.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		/// <returns></returns>
		public int GetLineLayoutHeight(
			TextEditor textEditor,
			int startLine,
			int endLine)
		{
			// If we have no lines, we have no height.
			if (LineCount == 0)
			{
				return 0;
			}

			// Normalize the last line, if we have one.
			if (endLine == -1)
			{
				endLine = LineCount - 1;
			}

			// Get a total of all the heights.
			int height = 0;

			for (int line = startLine; line <= endLine; line++)
			{
				// Get the height for this line.
				int lineHeight = GetLineLayoutHeight(textEditor, line);

				// Add the height to the total.
				height += lineHeight;
			}

			// Return the resulting height.
			return height;
		}

		/// <summary>
		/// Gets the height of a single line of "normal" text.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <returns></returns>
		public int GetTextLayoutLineHeight(TextEditor textEditor)
		{
			// Set the markup for the layout.
			EnsureLayoutExists();
			layout.SetText("W");
			lastLine = -1;

			// Set the style to the default line.
			textEditor.Theme.BlockStyles[Theme.TextStyle].SetLayout(layout);

			// Get the height of the default line.
			int height, width;
			layout.GetPixelSize(out width, out height);
			return height;
		}

		#endregion
	}
}