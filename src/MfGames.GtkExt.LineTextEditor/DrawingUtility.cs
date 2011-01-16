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

using Gtk;

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Color=Cairo.Color;
using Layout=Pango.Layout;
using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor
{
	/// <summary>
	/// Common and useful methods for working with Cairo and Pango.
	/// </summary>
	internal static class DrawingUtility
	{
		#region Layout

		#endregion

		/// <summary>
		/// Draws a layout with a given style to the render context.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="renderContext">The render context.</param>
		/// <param name="region">The region for the various elements.</param>
		/// <param name="layout">The layout to use for text.</param>
		/// <param name="style">The style used for borders and padding.</param>
		public static void DrawLayout(
			IDisplayContext displayContext,
			IRenderContext renderContext,
			Rectangle region,
			Layout layout,
			BlockStyle style)
		{
			// Get the style for the line number.
			Spacing margins = style.GetMargins();
			Spacing padding = style.GetPadding();
			Borders borders = style.GetBorders();
			double marginLeftX = margins.Left + borders.Left.LineWidth;
			double paddingLeftX = marginLeftX + padding.Left;
			double marginRightX = margins.Right + borders.Right.LineWidth;
			double paddingRightX = marginRightX + padding.Right;

			// Draw the background color.
			var cairoArea = new Rectangle(
				region.X + marginLeftX,
				region.Y,
				region.Width - marginLeftX - marginRightX,
				region.Height);

			// Paint the background color of the window.
			Color? backgroundColor = style.GetBackgroundColor();

			if (backgroundColor.HasValue)
			{
				renderContext.CairoContext.Color = backgroundColor.Value;
				renderContext.CairoContext.Rectangle(cairoArea);
				renderContext.CairoContext.Fill();
			}

			// Draw the border lines.
			if (borders.Left.LineWidth > 0)
			{
				renderContext.CairoContext.LineWidth = borders.Left.LineWidth;
				renderContext.CairoContext.Color = borders.Left.Color;

				renderContext.CairoContext.MoveTo(region.X + margins.Left, region.Y);
				renderContext.CairoContext.LineTo(
					region.X + margins.Left, region.Y + region.Height);
				renderContext.CairoContext.Stroke();
			}

			if (borders.Right.LineWidth > 0)
			{
				renderContext.CairoContext.LineWidth = borders.Right.LineWidth;
				renderContext.CairoContext.Color = borders.Right.Color;

				renderContext.CairoContext.MoveTo(
					region.X + region.Width - margins.Right, region.Y);
				renderContext.CairoContext.LineTo(
					region.X + region.Width - margins.Right, region.Y + region.Height);
				renderContext.CairoContext.Stroke();
			}

			// Render out the line number. Since this is right-aligned, we need
			// to get the text and start it to the right.
			int layoutWidth, layoutHeight;
			layout.GetPixelSize(out layoutWidth, out layoutHeight);

			displayContext.GdkWindow.DrawLayout(
				displayContext.GtkStyle.TextGC(StateType.Normal),
				(int) (region.X + region.Width - layoutWidth - paddingRightX),
				(int) region.Y,
				layout);
		}
	}
}