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

using Cairo;

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Models.Styles;

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Margins
{
	/// <summary>
	/// Implements a margin renderer which displays the line number, if there is
	/// one, on the side of the screen.
	/// </summary>
	public class LineNumberMarginRenderer : MarginRenderer
	{
		#region Size and Visibility

		/// <summary>
		/// Resizes the specified margin to fit the width of the line numbers
		/// from the top and bottom lines.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		public override void Resize(TextEditor textEditor)
		{
			// If we don't have any lines, we don't do anything.
			int lineCount = textEditor.LineBuffer.LineCount;

			if (lineCount == 0)
			{
				Width = 0;
				return;
			}

			// Create a layout object and set its values.
			layout = new Layout(textEditor.PangoContext);
			LineBlockStyle style = textEditor.Theme.LineNumberLineStyle;

			textEditor.SetLayout(layout, style);

			// Get the width of the first line.
			int width = 0;
			int newWidth, newHeight;
			string firstLineNumber = textEditor.LineBuffer.GetLineNumber(0);

			if (!string.IsNullOrEmpty(firstLineNumber))
			{
				// Set the text so it can be formatted.
				layout.SetText(firstLineNumber);

				// Determine if the width is greater and set it.
				layout.GetPixelSize(out newWidth, out newHeight);
				width = Math.Max(width, newWidth);
			}

			// Get the width of the last line.
			if (lineCount != 1)
			{
				string lastLineNumber = textEditor.LineBuffer.GetLineNumber(lineCount - 1);

				if (!string.IsNullOrEmpty(lastLineNumber))
				{
					// Set the text so it can be formatted.
					layout.SetText(lastLineNumber);

					// Determine if the width is greater and set it.
					layout.GetPixelSize(out newWidth, out newHeight);
					width = Math.Max(width, newWidth);
				}
			}

			// Add in the padding, borders, and margins.
			width +=
				(int)
				Math.Ceiling(
					style.GetMargins().Width + style.GetPadding().Width +
					style.GetBorders().Width);

			// Set the new width in the margin.
			Width = width;
		}

		#endregion

		#region Drawing

		private Layout layout;

		/// <summary>
		/// Draws the margin at the given position.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="renderContext">The render context.</param>
		/// <param name="lineIndex">The line index being rendered.</param>
		/// <param name="point">The point of the specific line number.</param>
		/// <param name="height">The height of the rendered line.</param>
		public override void Draw(
			IDisplayContext displayContext,
			IRenderContext renderContext,
			int lineIndex,
			PointD point,
			double height)
		{
			// Figure out the style we need to use.
			LineBlockStyle style;

			if (displayContext.Caret.Position.LineIndex == lineIndex)
			{
				// This is the current line.
				style = displayContext.Theme.CurrentLineBlockNumberLineBlockStyle;
			}
			else
			{
				style = displayContext.Theme.LineNumberLineStyle;
			}

			// Create a layout object if we don't have one.
			if (layout == null)
			{
				layout = new Layout(displayContext.PangoContext);
				displayContext.SetLayout(layout, style);
			}

			// Figure out the line number.
			string lineNumber = displayContext.LineBuffer.GetLineNumber(lineIndex);

			if (string.IsNullOrEmpty(lineNumber))
			{
				lineNumber = String.Empty;
			}

			// Wrap the text in a markup that includes the foreground color.
			string markup = DrawingUtility.WrapColorMarkup(
				lineNumber, style.GetForegroundColor());
			layout.SetMarkup(markup);

			// Use the common drawing routine to handle the borders and padding.
			DrawingUtility.DrawLayout(
				displayContext,
				renderContext,
				new Rectangle(point.X, point.Y, Width, height),
				layout,
				style);
		}

		/// <summary>
		/// Resets this margin to the default width and setting.
		/// </summary>
		public override void Reset()
		{
			// Reset the base implemention.
			base.Reset();

			// Get rid of our cached layout.
			layout = null;
		}

		#endregion
	}
}