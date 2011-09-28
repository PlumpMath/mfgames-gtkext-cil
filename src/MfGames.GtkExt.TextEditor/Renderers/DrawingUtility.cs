#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using Gtk;

using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models.Styles;

using Color = Cairo.Color;
using Context = Cairo.Context;
using Layout = Pango.Layout;
using Rectangle = Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.TextEditor.Renderers
{
    /// <summary>
    /// Common and useful methods for working with Cairo and Pango.
    /// </summary>
    internal static class DrawingUtility
    {
        #region Layout

        /// <summary>
        /// Draws a layout with a given style to the render context.
        /// </summary>
        /// <param name="displayContext">The display context.</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="region">The region for the various elements.</param>
        /// <param name="style">The style used for borders and padding.</param>
        public static void DrawLayout(
            IDisplayContext displayContext,
            IRenderContext renderContext,
            Rectangle region,
            BlockStyle style)
        {
            // If we don't have a style, then don't do anything.
            if (style == null)
            {
                return;
            }

            // Get the style for the line number.
            Spacing margins = style.GetMargins();
            Borders borders = style.GetBorders();

            double marginLeftX = margins.Left + borders.Left.LineWidth;
            double marginRightX = margins.Right + borders.Right.LineWidth;

            // Get the context and save settings because we use anti-aliasing
            // to get a sharper line.
            Context cairoContext = renderContext.CairoContext;
            Antialias oldAntialias = cairoContext.Antialias;

            try
            {
                // Draw the background color.
                Color? backgroundColor = style.GetBackgroundColor();

                if (backgroundColor.HasValue)
                {
                    var cairoArea = new Rectangle(
                        region.X + marginLeftX,
                        region.Y,
                        region.Width - marginLeftX - marginRightX,
                        region.Height);

                    cairoContext.Color = backgroundColor.Value;
                    cairoContext.Rectangle(cairoArea);
                    cairoContext.Fill();
                }

                // Figure out the width for rendering the horizontal borders so
                // they line up a little bit nicer. We adjust for half the border
                // width because Cairo draws in the middle and we have to shift by
                // half that to get the side to line up with the end of the
                // horizontal.
                double topMarginY = region.Y + margins.Top;
                double bottomMarginY = region.Y + region.Height - margins.Bottom;

                double leftMarginX = region.X + margins.Left +
                                     (borders.Left.LineWidth / 2);
                double rightMarginX = leftMarginX + region.Width - margins.Width;

                // Draw the border lines.
                cairoContext.Antialias = Antialias.None;

                if (borders.Top.LineWidth > 0)
                {
                    // Set up the line width and colors.
                    cairoContext.LineWidth = borders.Top.LineWidth;
                    cairoContext.Color = borders.Top.Color;

                    cairoContext.MoveTo(leftMarginX, topMarginY);
                    cairoContext.LineTo(rightMarginX, topMarginY);
                    cairoContext.Stroke();
                }

                if (borders.Bottom.LineWidth > 0)
                {
                    cairoContext.LineWidth = borders.Bottom.LineWidth;
                    cairoContext.Color = borders.Bottom.Color;

                    cairoContext.MoveTo(leftMarginX, bottomMarginY);
                    cairoContext.LineTo(rightMarginX, bottomMarginY);
                    cairoContext.Stroke();
                }

                if (borders.Left.LineWidth > 0)
                {
                    cairoContext.LineWidth = borders.Left.LineWidth;
                    cairoContext.Color = borders.Left.Color;

                    cairoContext.MoveTo(region.X + marginLeftX, topMarginY);
                    cairoContext.LineTo(region.X + marginLeftX, bottomMarginY);
                    cairoContext.Stroke();
                }

                if (borders.Right.LineWidth > 0)
                {
                    cairoContext.LineWidth = borders.Right.LineWidth;
                    cairoContext.Color = borders.Right.Color;

                    cairoContext.MoveTo(
                        region.X + region.Width - margins.Right, topMarginY);
                    cairoContext.LineTo(
                        region.X + region.Width - margins.Right, bottomMarginY);
                    cairoContext.Stroke();
                }
            }
            finally
            {
                // Restore the context.
                cairoContext.Antialias = oldAntialias;
            }
        }

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

            // Draw the layout borders and background.
            DrawLayout(displayContext, renderContext, region, style);

            // Figure out the extents of the layout.
            int layoutWidth, layoutHeight;
            layout.GetPixelSize(out layoutWidth, out layoutHeight);

            // Add the padding to the x coordinate since the only thing left is
            // to render the text.
            double textX = region.X + paddingLeftX;

            // Shift down based on the top-spacing.
            double textY = region.Y + style.Top;

            // Render out the line number. Since this is right-aligned, we need
            // to get the text and start it to the right.
            displayContext.GdkWindow.DrawLayout(
                displayContext.GtkStyle.TextGC(StateType.Normal),
                (int) textX,
                (int) textY,
                layout);
        }

        /// <summary>
        /// Wraps the given text in a color span tag.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static string WrapColorMarkup(
            string text,
            Color color)
        {
            return String.Format(
                "<span color=\"#{1}\">{0}</span>", text, color.ToRgbHexString());
        }

        #endregion
    }
}