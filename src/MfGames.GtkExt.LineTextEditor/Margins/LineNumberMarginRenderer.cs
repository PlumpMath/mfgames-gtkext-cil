using System;

using Gtk;

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Layout=Pango.Layout;
using Style=Pango.Style;

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
			Layout layout = new Layout(textEditor.PangoContext);
			textEditor.Theme.Selectors[Theme.LineNumberStyle].SetLayout(layout);

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

			// Set the new width in the margin.
			Width = width;
		}

		#endregion


		#region Drawing

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
			Cairo.Context cairoContext,
			int line,
			int x,
			int y,
			int height)
		{
			// Get the style for the line number.
			SelectorStyle style = textEditor.Theme.Selectors[Theme.LineNumberStyle];

			// Draw the background color.
			var cairoArea = new Cairo.Rectangle(x, y, Width, height);
			cairoContext.Color = style.GetBackgroundColor();
			cairoContext.Rectangle(cairoArea);
			cairoContext.Fill();

			// Create a layout of the line number.
			string lineNumber = textEditor.LineLayoutBuffer.GetLineNumber(line);

			if (string.IsNullOrEmpty(lineNumber))
			{
				// No line, we don't draw any text.
				return;
			}

			// Create a layout object and set its values.
			Layout layout = new Layout(textEditor.PangoContext);
			style.SetLayout(layout);
			layout.SetText(lineNumber);

			// Render out the line number. Since this is right-aligned, we need
			// to get the text and start it to the right.
			int layoutWidth, layoutHeight;
			layout.GetPixelSize(out layoutWidth, out layoutHeight);

			textEditor.GdkWindow.DrawLayout(
				textEditor.Style.TextGC(StateType.Normal),
				x + Width - layoutWidth,
				y,
				layout);
		}

		#endregion
	}
}