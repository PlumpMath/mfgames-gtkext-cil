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

using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models.Styles;

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.TextEditor.Renderers
{
	/// <summary>
	/// Implements a margin renderer which displays the line number, if there is
	/// one, on the side of the screen.
	/// </summary>
	public class LineNumberMarginRenderer : MarginRenderer
	{
		#region Drawing

		private Layout layout;
		private int maximumCharactersRendered;

		/// <summary>
		/// Draws the margin at the given position.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="renderContext">The render context.</param>
		/// <param name="lineIndex">The line index being rendered.</param>
		/// <param name="point">The point of the specific line number.</param>
		/// <param name="height">The height of the rendered line.</param>
		/// <param name="lineBlockStyle"></param>
		public override void Draw(
			IDisplayContext displayContext,
			IRenderContext renderContext,
			int lineIndex,
			PointD point,
			double height,
			LineBlockStyle lineBlockStyle)
		{
			// Figure out the style we need to use.
			MarginBlockStyle style =
				lineBlockStyle.MarginStyles.Get(Theme.LineNumberStyle);

			// Create a layout object if we don't have one.
			if (layout == null)
			{
				layout = new Layout(displayContext.PangoContext);
				displayContext.SetLayout(layout, style, Width);
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

			// Figure out if the current width of the margin is wider than what
			// we've already calculated.
			if (lineNumber.Length > maximumCharactersRendered)
			{
				// Get the new width as if we don't have line-wrapping.
				int layoutWidth, layoutHeight;

				layout.Width = Int32.MaxValue;
				layout.GetSize(out layoutWidth, out layoutHeight);

				// Set the layout width so we don't have to redo the entire
				// layout and update our margin width (including style).
				layout.Width = layoutWidth;
				SetWidth((int) (Units.ToPixels(layoutWidth) + style.Width));

				// Since we are looking at a large length of line number, update
				// it so we don't continually calculate the line width.
				maximumCharactersRendered = lineNumber.Length;

				// Request a full redraw since this will change even the lines
				// we already drew. We draw the line anyways to avoid seeing a
				// white block.
				displayContext.RequestScrollToCaret();
				displayContext.RequestRedraw();
			}

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
			maximumCharactersRendered = 0;
		}

		#endregion
	}
}