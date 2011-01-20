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

using System.Diagnostics;

using Cairo;

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Editing
{
	/// <summary>
	/// Represents the elements needed for displaying and rendering the caret.
	/// </summary>
	public class Caret
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Caret"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public Caret(IDisplayContext displayContext)
		{
			this.displayContext = displayContext;
			position = new BufferPosition(displayContext.LineLayoutBuffer);
		}

		#endregion

		#region Position

		private readonly IDisplayContext displayContext;
		private BufferPosition position;

		/// <summary>
		/// Gets or sets the buffer position of the caret.
		/// </summary>
		/// <value>The buffer position.</value>
		public BufferPosition Position
		{
			[DebuggerStepThrough]
			get { return position; }
			set { position = value ?? new BufferPosition(displayContext.LineLayoutBuffer); }
		}

		#endregion

		#region Rendering

		/// <summary>
		/// Draws the caret using the given context objects.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="renderContext">The render context.</param>
		public void Draw(
			IDisplayContext displayContext,
			IRenderContext renderContext)
		{
			// Get the coordinates on the screen and the height of the current line.
			int lineHeight;
			PointD point = Position.ToScreenCoordinates(displayContext, out lineHeight);
			double x = point.X;
			double y = point.Y;

			// Translate the buffer coordinates into the screen visible coordinates.
			y -= renderContext.VerticalAdjustment;

			// If the caret would not be visible in the render area, then don't
			// perform any drawing.
			if (y > renderContext.RenderRegion.Y + renderContext.RenderRegion.Height ||
			    y + lineHeight < renderContext.RenderRegion.Y)
			{
				// Don't show anything because it would be off-screen.
				return;
			}

			// Shift the contents to compenstate for the margins.
			x += displayContext.TextX;
			x +=
				displayContext.LineLayoutBuffer.GetLineStyle(
					displayContext, Position.LineIndex).Left;

			// Turn off antialiasing for a sharper, thin line.
			Context context = renderContext.CairoContext;

			Antialias oldAntialias = context.Antialias;
			context.Antialias = Antialias.None;

			// Draw the caret on the screen.
			try
			{
				context.LineWidth = 1;
				context.Color = new Color(0, 0, 0, 1);

				context.MoveTo(x, y);
				context.LineTo(x, y + lineHeight);
				context.Stroke();
			}
			finally
			{
				// Restore the context.
				context.Antialias = oldAntialias;
			}
		}

		#endregion
	}
}