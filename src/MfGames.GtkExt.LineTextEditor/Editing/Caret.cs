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

using MfGames.GtkExt.Extensions.Cairo;
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
		}

		#endregion

		#region Position and Range

		private readonly IDisplayContext displayContext;

		/// <summary>
		/// Contains the selection of the caret.
		/// </summary>
		public BufferSegment Selection;

		/// <summary>
		/// Gets or sets the buffer position of the caret.
		/// </summary>
		/// <value>The buffer position.</value>
		public BufferPosition Position
		{
			[DebuggerStepThrough]
			get { return Selection.TailPosition; }
			[DebuggerStepThrough]
			set
			{
				Selection.TailPosition = value;
				Selection.AnchorPosition = value;
			}
		}

		#endregion

		#region Rendering

		/// <summary>
		/// Draws the caret using the given context objects.
		/// </summary>
		/// <param name="renderContext">The render context.</param>
		public void Draw(IRenderContext renderContext)
		{
			// Get the draw region.
			Rectangle drawRegion = GetDrawRegion();

			// Make sure the render area intersects with the caret.
			if (!renderContext.RenderRegion.IntersectsWith(drawRegion))
			{
				// Not visible, don't show anything.
				return;
			}

			// Turn off antialiasing for a sharper, thin line.
			Context context = renderContext.CairoContext;

			Antialias oldAntialias = context.Antialias;
			context.Antialias = Antialias.None;

			// Draw the caret on the screen.
			try
			{
				context.LineWidth = 1;
				context.Color = new Color(0, 0, 0, 1);

				context.MoveTo(drawRegion.X, drawRegion.Y);
				context.LineTo(drawRegion.X, drawRegion.Y + drawRegion.Height);
				context.Stroke();
			}
			finally
			{
				// Restore the context.
				context.Antialias = oldAntialias;
			}
		}

		/// <summary>
		/// Gets the region that the caret would be drawn in.
		/// </summary>
		/// <returns></returns>
		public Rectangle GetDrawRegion()
		{
			// Get the coordinates on the screen and the height of the current line.
			int lineHeight;
			PointD point = Position.ToScreenCoordinates(displayContext, out lineHeight);
			double x = point.X;
			double y = point.Y;

			// Translate the buffer coordinates into the screen visible coordinates.
			y -= displayContext.BufferOffsetY;

			// Shift the contents to compenstate for the margins.
			x += displayContext.TextX;
			x += displayContext.TextRenderer.GetLineStyle(Position.LineIndex).Left;

			// Return the resulting rectangle.
			return new Rectangle(x, y, 1, lineHeight);
		}

		#endregion
	}
}