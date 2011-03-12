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
using System.Text;

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Renderers
{
	/// <summary>
	/// Functions as an adapter between the <see cref="LineBuffer"/> and the text
	/// editor to provide services for laying out text, wrapping words, and
	/// otherwise rendering the renderer.
	/// </summary>
	public abstract class TextRenderer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TextRenderer"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		protected TextRenderer(IDisplayContext displayContext)
		{
			// Set the properties in the renderer.
			if (displayContext == null)
			{
				throw new ArgumentNullException("displayContext");
			}

			DisplayContext = displayContext;
		}

		#endregion

		#region Buffer

		/// <summary>
		/// Gets the line buffer associated with this renderer.
		/// </summary>
		/// <value>The line buffer.</value>
		public abstract LineBuffer LineBuffer { get; }

		/// <summary>
		/// Gets or sets the display context associated with this renderer.
		/// </summary>
		/// <value>The display context.</value>
		public IDisplayContext DisplayContext { get; private set; }

		#endregion

		#region Layout

		/// <summary>
		/// Sets the pixel width of a layout in case the layout uses word
		/// wrapping.
		/// </summary>
		/// <value>The width.</value>
		public virtual int Width { get; set; }

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public virtual Layout GetLineLayout(
			int lineIndex)
		{
			var layout = new Layout(DisplayContext.PangoContext);

			DisplayContext.SetLayout(layout, DisplayContext.Theme.TextBlockStyle);
			layout.SetMarkup(GetSelectionMarkup(lineIndex));

			return layout;
		}

		/// <summary>
		/// Gets the height of a single line layout.
		/// </summary>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		private int GetLineLayoutHeight(
			int lineIndex)
		{
			// Get the extents for the line while rendered.
			Layout lineLayout = GetLineLayout(lineIndex);
			int lineWidth, lineHeight;

			lineLayout.GetPixelSize(out lineWidth, out lineHeight);

			// Get the style to include the style's height.
			BlockStyle style = GetLineStyle(lineIndex);

			lineHeight += (int) Math.Ceiling(style.Height);

			// Return the resulting height.
			return lineHeight;
		}

		/// <summary>
		/// Gets the pixel height of the lines in the buffer.
		/// </summary>
		/// <param name="startLineIndex">The start line.</param>
		/// <param name="endLineIndex">The end line.</param>
		/// <returns></returns>
		public virtual int GetLineLayoutHeight(
			int startLineIndex,
			int endLineIndex)
		{
			// If we have no lines, we have no height.
			int lineCount = LineBuffer.LineCount;

			if (lineCount == 0)
			{
				return 0;
			}

			// The endLineIndex can be beyond the end of the buffer's lines, so
			// cap it by the buffer.
			endLineIndex = Math.Min(endLineIndex, lineCount - 1);

			// Get a total of all the heights.
			int height = 0;

			for (int line = startLineIndex; line <= endLineIndex; line++)
			{
				// Get the height for this line.
				int lineHeight = GetLineLayoutHeight(line);

				// Add the height to the total.
				height += lineHeight;
			}

			// Return the resulting height.
			return height;
		}

		/// <summary>
		/// Gets the height of a single line of "normal" text.
		/// </summary>
		/// <returns></returns>
		public virtual int GetLineLayoutHeight()
		{
			// Get a layout for the default text style.
			var layout = new Layout(DisplayContext.PangoContext);

			DisplayContext.SetLayout(layout, DisplayContext.Theme.TextBlockStyle);

			// Set the layout to a simple string.
			layout.SetText("W");

			// Get the height of the default line.
			int height, width;

			layout.GetPixelSize(out width, out height);

			return height;
		}

		/// <summary>
		/// Gets the line at a specific point and returns it.
		/// </summary>
		/// <param name="bufferY">The buffer Y.</param>
		/// <returns></returns>
		public int GetLineLayoutRange(
			double bufferY)
		{
			var rectangle = new Rectangle(0, bufferY, 0.1, 0.1);
			int startLineIndex, endLineIndex;

			GetLineLayoutRange(rectangle, out startLineIndex, out endLineIndex);

			return startLineIndex;
		}

		/// <summary>
		/// Gets the lines that are visible in the given view area.
		/// </summary>
		/// <param name="viewArea">The view area.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		public void GetLineLayoutRange(
			Rectangle viewArea,
			out int startLine,
			out int endLine)
		{
			// Reset the start line to negative to indicate we don't have a
			// visible line yet.
			startLine = -1;

			// Go through all the lines until we find one that is in the range.
			int currentY = 0;
			int lineCount = LineBuffer.LineCount;

			for (int line = 0; line < lineCount; line++)
			{
				// Get the height for this line.
				int height = GetLineLayoutHeight(line);

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
		public virtual void Reset()
		{
			// The simple layout buffer has no calculations.
		}

		#endregion

		#region Wrapped Lines

		/// <summary>
		/// Gets the wrapped line layout for a given buffer Y coordinate.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="bufferY">The buffer Y.</param>
		/// <returns></returns>
		public LayoutLine GetWrappedLineLayout(
			IDisplayContext displayContext,
			double bufferY)
		{
			int wrappedLineIndex;
			return GetWrappedLineLayout(displayContext, bufferY, out wrappedLineIndex);
		}

		/// <summary>
		/// Gets the wrapped line layout for a given buffer Y coordinate and the
		/// associated index.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="bufferY">The buffer Y.</param>
		/// <param name="wrappedLineIndex">Index of the wrapped line.</param>
		/// <returns></returns>
		public LayoutLine GetWrappedLineLayout(
			IDisplayContext displayContext,
			double bufferY,
			out int wrappedLineIndex)
		{
			// Get the line that contains the given Y coordinate.
			int lineIndex, endLineIndex;
			GetLineLayoutRange(
				new Rectangle(0, bufferY, 0, bufferY),
				out lineIndex,
				out endLineIndex);

			// Get the layout-relative Y coordinate.
			double layoutY = bufferY - GetLineLayoutHeight(0, lineIndex);

			// Figure out which line inside the layout.
			Layout layout = GetLineLayout(lineIndex);
			int trailing;

			layout.XyToIndex(0, (int) layoutY, out wrappedLineIndex, out trailing);

			// Return the layout line.
			return layout.Lines[wrappedLineIndex];
		}

		/// <summary>
		/// Gets the index of the wrapped line layout.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="bufferY">The buffer Y.</param>
		/// <returns></returns>
		public int GetWrappedLineLayoutIndex(
			IDisplayContext displayContext,
			double bufferY)
		{
			int wrappedLineIndex;
			GetWrappedLineLayout(displayContext, bufferY, out wrappedLineIndex);
			return wrappedLineIndex;
		}

		#endregion

		#region Indicators

		/// <summary>
		/// Gets the line indicators for a given character range.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="startCharacterIndex">Start character in the line text.</param>
		/// <param name="endCharacterIndex">End character in the line text.</param>
		/// <returns></returns>
		public IEnumerable<ILineIndicator> GetLineIndicators(
			IDisplayContext displayContext,
			int lineIndex,
			int startCharacterIndex,
			int endCharacterIndex)
		{
			return new List<ILineIndicator>();
		}

		/// <summary>
		/// Gets the line indicators for the entire line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">Index of the line.</param>
		/// <returns></returns>
		public IEnumerable<ILineIndicator> GetLineIndicators(
			IDisplayContext displayContext,
			int lineIndex)
		{
			return GetLineIndicators(displayContext, lineIndex, 0, Int32.MaxValue);
		}

		#endregion

		#region Styles

		/// <summary>
		/// Gets the line style associated with a line.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <returns></returns>
		public virtual BlockStyle GetLineStyle(int lineIndex)
		{
			// Get the style name and normalize it.
			string styleName = LineBuffer.GetLineStyleName(lineIndex);

			if (String.IsNullOrEmpty(styleName))
			{
				styleName = Theme.TextStyle;
			}

			// Retrieve the style.
			return DisplayContext.Theme.BlockStyles[styleName];
		}

		#endregion

		#region Operations

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

		/// <summary>
		/// Called when a line changes in the underlying buffer.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		protected virtual void OnLineChanged(
			object sender,
			LineChangedArgs e)
		{
			RaiseLineChanged(e);
		}

		/// <summary>
		/// Called when lines are deleted from the underlying buffer.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="MfGames.GtkExt.LineTextEditor.Events.LineRangeEventArgs"/> instance containing the event data.</param>
		protected virtual void OnLinesDeleted(
			object sender,
			LineRangeEventArgs e)
		{
			RaiseLinesDeleted(e);
		}

		/// <summary>
		/// Called when lines are inserted into the underlying buffer.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="MfGames.GtkExt.LineTextEditor.Events.LineRangeEventArgs"/> instance containing the event data.</param>
		protected virtual void OnLinesInserted(
			object sender,
			LineRangeEventArgs e)
		{
			RaiseLinesInserted(e);
		}

		/// <summary>
		/// Raises the <see cref="LineChanged"/> event
		/// </summary>
		/// <param name="e">The e.</param>
		protected virtual void RaiseLineChanged(LineChangedArgs e)
		{
			var listeners = LineChanged;

			if (listeners != null)
			{
				listeners(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="LinesDeleted"/> event.
		/// </summary>
		/// <param name="e">The e.</param>
		protected virtual void RaiseLinesDeleted(LineRangeEventArgs e)
		{
			var listeners = LinesDeleted;

			if (listeners != null)
			{
				listeners(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="LinesInserted"/> event.
		/// </summary>
		/// <param name="e">The e.</param>
		protected virtual void RaiseLinesInserted(LineRangeEventArgs e)
		{
			var listeners = LinesInserted;

			if (listeners != null)
			{
				listeners(this, e);
			}
		}

		#endregion

		#region Selection

		/// <summary>
		/// Updates the selection inside the renderer.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="previousSelection">The previous selection.</param>
		public virtual void UpdateSelection(
			IDisplayContext displayContext,
			BufferSegment previousSelection)
		{
		}

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public string GetSelectionMarkup(int lineIndex)
		{
			// Get the line markup from the underlying buffer.
			string markup = LineBuffer.GetLineMarkup(lineIndex);

			// Check to see if we are in the selection.
			int startCharacterIndex, endCharacterIndex;
			bool containsLine = DisplayContext.Caret.Selection.ContainsLine(
				lineIndex, out startCharacterIndex, out endCharacterIndex);

			if (containsLine)
			{
				// Loop through the markup and strip off the existing markup
				// for the selection area.
				var buffer = new StringBuilder();
				bool inSelection = false;

				for (int characterIndex = 0;
				     characterIndex < markup.Length;
				     characterIndex++)
				{
					// Pull out the character we are processing.
					char c = markup[characterIndex];

					// See if we are at the beginning of the selection.
					if (characterIndex == startCharacterIndex)
					{
						// We are now in a selection buffer. First remove any
						// spans that we are currently using.

						// Add the text to format our selection.
						buffer.Append("<span background='#CCCCFF'>");

						// Mark that we are in the selection.
						inSelection = true;
					}

					// Check to see if we are at the end of the selection.
					if (characterIndex == endCharacterIndex)
					{
						// We are finishing up the selection, so remove any
						// selection-specific spans we are using.
						buffer.Append("</span>");

						// Apply the spans that should have existed here.

						// Leave the selection so we don't close it twice.
						inSelection = false;
					}

					// Add the character to the buffer.
					buffer.Append(c);
				}

				// If the end index is -1, then we finish up the spans at the end.
				if (inSelection)
				{
					// Remove any spans currently in effect for the selection.
					buffer.Append("</span>");
				}

				// Replace the markup with the modified one.
				markup = buffer.ToString();
			}

			// Return the resulting markup.
			return markup;
		}

		#endregion
	}
}