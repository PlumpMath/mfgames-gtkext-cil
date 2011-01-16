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

using C5;

using Cairo;

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Margins
{
	/// <summary>
	/// Encapsulates a list of margin renderers. This handles the packing code
	/// and processing visbility, widths, and heights.
	/// </summary>
	public class MarginRendererCollection : LinkedList<MarginRenderer>
	{
		#region Size and Width

		private bool supressEvents;
		private int width;

		/// <summary>
		/// Gets the width of the entire collection.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return width; }
		}

		/// <summary>
		/// Fires the WidthChanged event.
		/// </summary>
		private void FireWidthChanged()
		{
			if (WidthChanged != null && !supressEvents)
			{
				WidthChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Resets all the individual margin renderers in the collection.
		/// </summary>
		public void Reset()
		{
			supressEvents = true;

			try
			{
				foreach (MarginRenderer marginRenderer in this)
				{
					marginRenderer.Reset();
				}
			}
			finally
			{
				supressEvents = false;
			}
		}

		/// <summary>
		/// Resizes the margins to fit the new line buffer.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		public void Resize(TextEditor textEditor)
		{
			Reset();

			foreach (MarginRenderer marginRenderer in this)
			{
				marginRenderer.Resize(textEditor);
			}
		}

		/// <summary>
		/// Occurs when the width or visiblity of the margin changes.
		/// </summary>
		public event EventHandler WidthChanged;

		#endregion

		#region Collection

		public override bool Add(MarginRenderer item)
		{
			item.WidthChanged += OnWidthChanged;
			RecalculateWidth();
			return base.Add(item);
		}

		public override void Insert(
			int i,
			MarginRenderer item)
		{
			item.WidthChanged += OnWidthChanged;
			RecalculateWidth();
			base.Insert(i, item);
		}

		/// <summary>
		/// Called when the width of a child element is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnWidthChanged(
			object sender,
			EventArgs e)
		{
			RecalculateWidth();
		}

		/// <summary>
		/// Recalculates the width of the entire collection by adding up the
		/// widths of all the margins within it.
		/// </summary>
		private void RecalculateWidth()
		{
			// Gather up the new total width of the collection.
			int newWidth = 0;

			foreach (MarginRenderer marginRenderer in this)
			{
				if (marginRenderer.Visible)
				{
					newWidth += marginRenderer.Width;
				}
			}

			// If the width has changed, then fire an event.
			if (newWidth != width)
			{
				width = newWidth;
				FireWidthChanged();
			}
		}

		public override bool Remove(MarginRenderer item)
		{
			item.WidthChanged -= OnWidthChanged;
			RecalculateWidth();
			return base.Remove(item);
		}

		#endregion

		#region Drawing

		/// <summary>
		/// Draws the margins at the given position.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="renderContext">The render context.</param>
		/// <param name="lineIndex">The line index being rendered.</param>
		/// <param name="point">The point of the specific line number.</param>
		/// <param name="height">The height of the rendered line.</param>
		public void Draw(
			IDisplayContext displayContext,
			IRenderContext renderContext,
			int lineIndex,
			PointD point,
			int height)
		{
			// Go through the margins and draw each one so they don't overlap.
			double dx = point.X;

			foreach (MarginRenderer marginRenderer in this)
			{
				// If it isn't visible, then we do nothing.
				if (!marginRenderer.Visible)
				{
					continue;
				}

				// Draw out the individual margin.
				marginRenderer.Draw(
					displayContext, renderContext, lineIndex, new PointD(dx, point.Y), height);

				// Add to the x coordinate so we don't overlap the renders.
				dx += marginRenderer.Width;
			}
		}

		#endregion
	}
}