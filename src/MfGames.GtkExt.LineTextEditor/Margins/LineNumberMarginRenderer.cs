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

using Gtk;

using MfGames.GtkExt.LineTextEditor.Visuals;

using Context=Cairo.Context;
using Layout=Pango.Layout;
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
			int lineCount = textEditor.LineLayoutBuffer.LineCount;

			if (lineCount == 0)
			{
				Width = 0;
				return;
			}

			// Create a layout object and set its values.
			layout = new Layout(textEditor.PangoContext);
			BlockStyle style = textEditor.Theme.BlockStyles[Theme.LineNumberStyle];

			textEditor.SetLayout(layout, textEditor.Theme.BlockStyles[Theme.TextStyle]);

			// Get the width of the first line.
			int width = 0;
			int newWidth, newHeight;
			string firstLineNumber = textEditor.LineLayoutBuffer.GetLineNumber(0);

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
				string lastLineNumber =
					textEditor.LineLayoutBuffer.GetLineNumber(lineCount - 1);

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
		/// <param name="textEditor">The text editor.</param>
		/// <param name="cairoContext">The cairo context.</param>
		/// <param name="line">The line.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="height">The height.</param>
		public override void Draw(
			TextEditor textEditor,
			Context cairoContext,
			int line,
			int x,
			int y,
			int height)
		{
			// Get the style for the line number.
			BlockStyle style = textEditor.Theme.BlockStyles[Theme.LineNumberStyle];
			Spacing margins = style.GetMargins();
			Spacing padding = style.GetPadding();
			Borders borders = style.GetBorders();
			double marginLeftX = margins.Left + borders.Left.LineWidth;
			double paddingLeftX = marginLeftX + padding.Left;
			double marginRightX = margins.Right + borders.Right.LineWidth;
			double paddingRightX = marginRightX + padding.Right;

			// Draw the background color.
			var cairoArea = new Rectangle(
				x + marginLeftX, y, Width - marginLeftX - marginRightX, height);
			cairoContext.Color = style.GetBackgroundColor();
			cairoContext.Rectangle(cairoArea);
			cairoContext.Fill();

			// Draw the border lines.
			if (borders.Left.LineWidth > 0)
			{
				cairoContext.LineWidth = borders.Left.LineWidth;
				cairoContext.Color = borders.Left.Color;

				cairoContext.MoveTo(x + margins.Left, y);
				cairoContext.LineTo(x + margins.Left, y + height);
				cairoContext.Stroke();
			}

			if (borders.Right.LineWidth > 0)
			{
				cairoContext.LineWidth = borders.Right.LineWidth;
				cairoContext.Color = borders.Right.Color;

				cairoContext.MoveTo(x + Width - margins.Right, y);
				cairoContext.LineTo(x + Width - margins.Right, y + height);
				cairoContext.Stroke();
			}

			// Create a layout of the line number.
			string lineNumber = textEditor.LineLayoutBuffer.GetLineNumber(line);

			if (string.IsNullOrEmpty(lineNumber))
			{
				// No line, we don't draw any text.
				return;
			}

			// Create a layout object and set its values.
			if (layout == null)
			{
				layout = new Layout(textEditor.PangoContext);
				textEditor.SetLayout(layout, textEditor.Theme.BlockStyles[Theme.TextStyle]);
			}

			layout.SetText(lineNumber);

			// Render out the line number. Since this is right-aligned, we need
			// to get the text and start it to the right.
			int layoutWidth, layoutHeight;
			layout.GetPixelSize(out layoutWidth, out layoutHeight);

			textEditor.GdkWindow.DrawLayout(
				textEditor.Style.TextGC(StateType.Normal),
				(int) (x + Width - layoutWidth - paddingRightX),
				y,
				layout);
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