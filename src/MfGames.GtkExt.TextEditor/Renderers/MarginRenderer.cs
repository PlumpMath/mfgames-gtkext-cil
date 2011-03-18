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

#endregion

namespace MfGames.GtkExt.LineTextEditor.Margins
{
	/// <summary>
	/// The abstract base class for margin renderers. These can be used to show
	/// margin markers, line numbers, or even folding marks.
	/// </summary>
	public abstract class MarginRenderer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MarginRenderer"/> class.
		/// </summary>
		protected MarginRenderer()
		{
			visible = true;
		}

		#endregion

		#region Size and Visibility

		private bool visible;
		private int width;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MarginRenderer"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible
		{
			get { return visible; }
			set
			{
				visible = value;
				FireWidthChanged();
			}
		}

		/// <summary>
		/// Gets the width of the margin.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return width; }
			set
			{
				bool fireEvent = width != value;

				width = value;

				if (fireEvent)
				{
					FireWidthChanged();
				}
			}
		}

		/// <summary>
		/// Fires the WidthChanged event.
		/// </summary>
		private void FireWidthChanged()
		{
			if (WidthChanged != null)
			{
				WidthChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Resets this margin to the default width and setting.
		/// </summary>
		public virtual void Reset()
		{
			Width = 0;
		}

		/// <summary>
		/// Resizes the specified margin based on the context.
		/// </summary>
		/// <param name="editorView">The text editor.</param>
		public virtual void Resize(EditorView editorView)
		{
			// The default implementation is to do nothing.
		}

		/// <summary>
		/// Occurs when the width or visiblity of the margin changes.
		/// </summary>
		public event EventHandler WidthChanged;

		#endregion

		#region Drawing

		/// <summary>
		/// Draws the margin at the given position.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="renderContext">The render context.</param>
		/// <param name="lineIndex">The line index being rendered.</param>
		/// <param name="point">The point of the specific line number.</param>
		/// <param name="height">The height of the rendered line.</param>
		public abstract void Draw(
			IDisplayContext displayContext,
			IRenderContext renderContext,
			int lineIndex,
			PointD point,
			double height);

		#endregion
	}
}