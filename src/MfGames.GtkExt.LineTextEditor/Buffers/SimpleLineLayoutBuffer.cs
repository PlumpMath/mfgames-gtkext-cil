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

		#region Layout

		/// <summary>
		/// Sets the pixel width of a layout in case the layout uses word
		/// wrapping.
		/// </summary>
		/// <value>The width.</value>
		public int Width { get; set; }

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public Layout GetLineLayout(
			IDisplayContext displayContext,
			int lineIndex)
		{
			var layout = new Layout(displayContext.PangoContext);

			displayContext.SetLayout(
				layout, displayContext.Theme.BlockStyles[Theme.TextStyle]);
			layout.SetMarkup(GetLineMarkup(lineIndex));

			return layout;
		}

		/// <summary>
		/// Gets the height of a single line layout.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="displayContext">The text editor.</param>
		/// <returns></returns>
		private int GetLineLayoutHeight(
			IDisplayContext displayContext,
			int line)
		{
			// Get the extents for the line while rendered.
			Layout lineLayout = GetLineLayout(displayContext, line);
			int lineWidth, lineHeight;

			lineLayout.GetPixelSize(out lineWidth, out lineHeight);

			// Get the style to include the style's height.
			BlockStyle style = GetLineStyle(displayContext, line);

			lineHeight += style.Height;

			// Return the resulting height.
			return lineHeight;
		}

		/// <summary>
		/// Gets the pixel height of the lines in the buffer. If endLine is -1
		/// it means the last line in the buffer.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="startLineIndex">The start line.</param>
		/// <param name="endLineIndex">The end line.</param>
		/// <returns></returns>
		public int GetLineLayoutHeight(
			IDisplayContext displayContext,
			int startLineIndex,
			int endLineIndex)
		{
			// If we have no lines, we have no height.
			if (LineCount == 0)
			{
				return 0;
			}

			// Normalize the last line, if we have one.
			if (endLineIndex == -1)
			{
				endLineIndex = LineCount - 1;
			}

			// Get a total of all the heights.
			int height = 0;

			for (int line = startLineIndex; line <= endLineIndex; line++)
			{
				// Get the height for this line.
				int lineHeight = GetLineLayoutHeight(displayContext, line);

				// Add the height to the total.
				height += lineHeight;
			}

			// Return the resulting height.
			return height;
		}

		/// <summary>
		/// Gets the height of a single line of "normal" text.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns></returns>
		public int GetLineLayoutHeight(IDisplayContext displayContext)
		{
			// Get a layout for the default text style.
			var layout = new Layout(displayContext.PangoContext);

			displayContext.SetLayout(
				layout, displayContext.Theme.BlockStyles[Theme.TextStyle]);

			// Set the layout to a simple string.
			layout.SetText("W");

			// Get the height of the default line.
			int height, width;

			layout.GetPixelSize(out width, out height);

			return height;
		}

		/// <summary>
		/// Gets the lines that are visible in the given view area.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="viewArea">The view area.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		public void GetLineLayoutRange(IDisplayContext displayContext,
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
				int height = GetLineLayoutHeight(displayContext, line);

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
		/// Indicates that the underlying text editor has changed in some manner
		/// and any cache or size calculations are invalidated.
		/// </summary>
		public void Reset()
		{
			// The simple layout buffer has no calculations.
		}

		#endregion
	}
}