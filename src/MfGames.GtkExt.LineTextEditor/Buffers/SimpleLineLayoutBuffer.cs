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
		/// Gets the wrapped line indexes for a given line index.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">The line index.</param>
		/// <param name="startWrappedLineIndex">Start index of the wrapped line.</param>
		/// <param name="endWrappedLineIndex">End index of the wrapped line.</param>
		public virtual void GetWrappedLineIndexes(
			IDisplayContext displayContext,
			int lineIndex,
			out int startWrappedLineIndex,
			out int endWrappedLineIndex)
		{
			// Go through the wrapped lines until we find the right one.
			int count = 0;

			for (int currentLineIndex = 0; lineIndex < LineCount; lineIndex++)
			{
				// Get the layout since we use it for both the the line and
				// all the lines before it.
				Layout layout = GetLineLayout(displayContext, lineIndex);

				if (currentLineIndex == lineIndex)
				{
					startWrappedLineIndex = count;
					endWrappedLineIndex = count + layout.LineCount;
					return;
				}

				count += layout.LineCount;
			}

			// We got to the end of the buffer and didn't find it.
			throw new Exception("Cannot find line index in buffer");
		}

		/// <summary>
		/// Sets the pixel width of a layout in case the layout uses word
		/// wrapping.
		/// </summary>
		/// <value>The width.</value>
		public int Width { get; set; }

		/// <summary>
		/// Gets the index of the line from a given wrapped line index. This also
		/// returns the relative line index inside the layout.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="wrappedLineIndex">Index of the wrapped line.</param>
		/// <param name="layoutLineIndex">Index of the layout line.</param>
		/// <returns></returns>
		public int GetLineIndex(
			IDisplayContext displayContext,
			int wrappedLineIndex,
			out int layoutLineIndex)
		{
			// Go through the wrapped lines until we find the right one.
			int count = 0;

			for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
			{
				// Get the layout and add the lines.
				Layout layout = GetLineLayout(displayContext, lineIndex);

				int newCount = count + layout.LineCount;

				// If the count is greater than our index, we are in the right
				// layout to return the values.
				if (newCount > wrappedLineIndex)
				{
					// Figure out the relative wrapped line index.
					layoutLineIndex = wrappedLineIndex - count;

					// Return the line index.
					return lineIndex;
				}

				// Update the count with the new value.
				count = newCount;
			}

			// We got to the end of the buffer and didn't find it.
			throw new Exception("Cannot find wrapped line index in buffer");
		}

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

			displayContext.SetLayout(layout, displayContext.Theme.TextBlockStyle);
			layout.SetMarkup(GetLineMarkup(displayContext, lineIndex));

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

			lineHeight += (int) Math.Ceiling(style.Height);

			// Return the resulting height.
			return lineHeight;
		}

		/// <summary>
		/// Gets the pixel height of the lines in the buffer.
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

			// The endLineIndex can be beyond the end of the buffer's lines, so
			// cap it by the buffer.
			endLineIndex = Math.Min(endLineIndex, LineCount - 1);

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

			displayContext.SetLayout(layout, displayContext.Theme.TextBlockStyle);

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
		public void GetLineLayoutRange(
			IDisplayContext displayContext,
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
		/// Gets the wrapped line count for all the lines in the buffer.
		/// </summary>
		/// <value>The wrapped line count.</value>
		public int GetWrappedLineCount(IDisplayContext displayContext)
		{
			int count = 0;

			for (int lineIndex = 0; lineIndex < LineCount; lineIndex++)
			{
				Layout layout = GetLineLayout(displayContext, lineIndex);

				count += layout.LineCount;
			}

			return count;
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