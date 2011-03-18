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

using MfGames.GtkExt.TextEditor.Editing;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Styles;
using MfGames.GtkExt.TextEditor.Renderers;

using Pango;

using Layout=Pango.Layout;
using Rectangle=Cairo.Rectangle;
using Style=Gtk.Style;
using Window=Gdk.Window;

#endregion

namespace MfGames.GtkExt.TextEditor.Interfaces
{
	/// <summary>
	/// Contains information about the display and its appearance.
	/// </summary>
	public interface IDisplayContext
	{
		/// <summary>
		/// Gets the caret used to indicate where the user is editing.
		/// </summary>
		/// <value>The caret.</value>
		Caret Caret { get; }

		/// <summary>
		/// Gets the clipboard associated with this editor.
		/// </summary>
		/// <value>The clipboard.</value>
		Clipboard Clipboard { get; }

		/// <summary>
		/// Gets the line buffer associated with the context.
		/// </summary>
		/// <value>The line buffer.</value>
		LineBuffer LineBuffer { get; }

		/// <summary>
		/// Gets the line layout buffer.
		/// </summary>
		/// <value>The line layout buffer.</value>
		TextRenderer TextRenderer { get; }

		/// <summary>
		/// Gets the width of the area that can be used for rendering text.
		/// </summary>
		/// <value>The width of the text.</value>
		int TextWidth { get; }

		/// <summary>
		/// Gets the text X coordinate.
		/// </summary>
		/// <value>The text X.</value>
		int TextX { get; }

		/// <summary>
		/// Gets the theme collection for this display.
		/// </summary>
		/// <value>The theme.</value>
		Theme Theme { get; }

		/// <summary>
		/// Gets or sets the word splitter.
		/// </summary>
		/// <value>The word splitter.</value>
		IWordSplitter WordSplitter { get; set; }

		/// <summary>
		/// Sets the layout according to the given layout and style.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <param name="style">The style.</param>
		void SetLayout(
			Layout layout,
			LineBlockStyle style);

		#region Windows

		/// <summary>
		/// Gets or sets the vertical adjustment or offset into the viewing area.
		/// </summary>
		/// <value>The vertical adjustment.</value>
		double BufferOffsetY { get; }

		/// <summary>
		/// Gets the GDK window associated with this context.
		/// </summary>
		/// <value>The GDK window.</value>
		Window GdkWindow { get; }

		/// <summary>
		/// Gets the GTK style associated with this context.
		/// </summary>
		/// <value>The GTK style.</value>
		Style GtkStyle { get; }

		/// <summary>
		/// Gets the Pango context associated with this display.
		/// </summary>
		/// <value>The pango context.</value>
		Context PangoContext { get; }

		/// <summary>
		/// Gets the vertical adjustment.
		/// </summary>
		/// <value>The vertical adjustment.</value>
		Adjustment VerticalAdjustment { get; }

		/// <summary>
		/// Requests the entire widget is redrawn.
		/// </summary>
		void RequestRedraw();

		/// <summary>
		/// Requests the given region is redrawn.
		/// </summary>
		/// <param name="region">The widget-relative region.</param>
		void RequestRedraw(Rectangle region);

		/// <summary>
		/// Scrolls the display to the caret.
		/// </summary>
		void ScrollToCaret();

		#endregion
	}
}