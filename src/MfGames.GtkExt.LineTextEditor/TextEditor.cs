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

using Gdk;

using Gtk;

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Margins;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

using CairoHelper=Gdk.CairoHelper;
using Color=Cairo.Color;
using Context=Cairo.Context;
using Key=Gdk.Key;
using Layout=Pango.Layout;
using Rectangle=Gdk.Rectangle;
using Style=Gtk.Style;
using Window=Gdk.Window;
using WindowType=Gdk.WindowType;

#endregion

namespace MfGames.GtkExt.LineTextEditor
{
	/// <summary>
	/// The primary editor control for the virtualized line text editor.
	/// </summary>
	public class TextEditor : Widget, IDisplayContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TextEditor"/> class.
		/// </summary>
		public TextEditor()
			: this(new SimpleLineLayoutBuffer(new MemoryLineBuffer()))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextEditor"/> class.
		/// </summary>
		public TextEditor(ILineLayoutBuffer lineLayoutBuffer)
		{
			// Set up the basic characterstics of the widget.
			Events = EventMask.PointerMotionMask | EventMask.ButtonPressMask |
			         EventMask.ButtonReleaseMask | EventMask.EnterNotifyMask |
			         EventMask.LeaveNotifyMask | EventMask.VisibilityNotifyMask |
			         EventMask.FocusChangeMask | EventMask.ScrollMask |
			         EventMask.KeyPressMask | EventMask.KeyReleaseMask;
			DoubleBuffered = true;
			CanFocus = true;
			WidgetFlags |= WidgetFlags.NoWindow;

			// Set up the rest of the screen elements.
			margins = new MarginRendererCollection();
			margins.Add(new LineNumberMarginRenderer());
			theme = new Theme();

			// Save the line buffer which configures a number of other elements.
			LineLayoutBuffer = lineLayoutBuffer;

			// Set up the caret, this must be done after the buffer is set.
			caret = new Caret(this);

			// Set up the text editor controller.
			controller = new TextEditorController(this);
			wordSplitter = new OffsetWordSplitter();
		}

		protected TextEditor(IntPtr raw)
			: base(raw)
		{
		}

		#endregion

		#region Debugging

		private readonly DateTime createdTimestamp = DateTime.Now;

		#endregion

		#region Line Buffer

		private ILineLayoutBuffer lineLayoutBuffer;

		/// <summary>
		/// Gets or sets the line layout buffer.
		/// </summary>
		/// <value>The line layout buffer.</value>
		public ILineLayoutBuffer LineLayoutBuffer
		{
			get { return lineLayoutBuffer; }
			set
			{
				// Set the new buffer.
				lineLayoutBuffer = value;

				// Configure the new layout buffer.
				if (lineLayoutBuffer != null)
				{
					// Reset the margins and force them to resize themselves.
					margins.Resize(this);
				}
			}
		}

		#endregion

		#region Layout

		/// <summary>
		/// Sets the layout using the given block style.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <param name="style">The style.</param>
		public void SetLayout(
			Layout layout,
			BlockStyle style)
		{
			// Set the style elements.
			layout.Wrap = style.GetWrap();
			layout.Alignment = style.GetAlignment();
			layout.FontDescription = style.GetFontDescription();

			// Check to see if we are doing line wrapping and set the width,
			// minus the padding, margins, and borders.
			layout.Width = Units.FromPixels((int) Math.Ceiling(TextWidth - style.Width));
		}

		#endregion

		#region Display

		private readonly MarginRendererCollection margins;
		private Theme theme;
		private IWordSplitter wordSplitter;

		/// <summary>
		/// Gets the GTK style associated with this context.
		/// </summary>
		/// <value>The GTK style.</value>
		public Style GtkStyle
		{
			get { return Style; }
		}

		/// <summary>
		/// Gets or sets the theme.
		/// </summary>
		/// <value>The theme.</value>
		public Theme Theme
		{
			get { return theme; }
			set { theme = value ?? new Theme(); }
		}

		/// <summary>
		/// Gets or sets the word splitter.
		/// </summary>
		/// <value>The word splitter.</value>
		public IWordSplitter WordSplitter
		{
			get { return wordSplitter; }
			set { wordSplitter = value ?? new OffsetWordSplitter(); }
		}

		#endregion

		#region Editing

		private readonly Caret caret;
		private readonly TextEditorController controller;

		/// <summary>
		/// Gets the caret used to indicate where the user is editing.
		/// </summary>
		/// <value>The caret.</value>
		public Caret Caret
		{
			get { return caret; }
		}

		/// <summary>
		/// Called when a key is pressed.
		/// </summary>
		/// <param name="eventKey">The event key.</param>
		/// <returns></returns>
		protected override bool OnKeyPressEvent(EventKey eventKey)
		{
			// Decompose the key into its components.
			ModifierType modifier;
			Key key;
			GdkUtility.DecomposeKeys(eventKey, out key, out modifier);

			// Get the unicode character for this key.
			uint unicodeChar = Keyval.ToUnicode(eventKey.KeyValue);

			// Pass it on to the controller.
			Console.WriteLine("Key " + key + ", modifier " + modifier);
			return controller.HandleKeypress(key, unicodeChar, modifier);
		}

		protected override bool OnKeyReleaseEvent(EventKey evnt)
		{
			return base.OnKeyReleaseEvent(evnt);
		}

		#endregion

		#region Rendering Events

		/// <summary>
		/// Gets the width of the area that can be used for rendering text.
		/// </summary>
		/// <value>The width of the text.</value>
		public int TextWidth
		{
			get { return Allocation.Width - margins.Width; }
		}

		/// <summary>
		/// Gets the text X coordinate.
		/// </summary>
		/// <value>The text X.</value>
		public int TextX
		{
			get { return margins.Width; }
		}

		/// <summary>
		/// Called when the widget is exposed or drawn.
		/// </summary>
		/// <param name="e">The e.</param>
		/// <returns></returns>
		protected override bool OnExposeEvent(EventExpose e)
		{
			// Figure out the area we are rendering into.
			Rectangle area = e.Region.Clipbox;
			var cairoArea = new Cairo.Rectangle(area.X, area.Y, area.Width, area.Height);

			using (Context cairoContext = CairoHelper.Create(e.Window))
			{
				// Create a render context.
				var renderContext = new RenderContext(cairoContext);
				renderContext.RenderRegion = cairoArea;
				renderContext.VerticalAdjustment = verticalAdjustment.Value;

				// Paint the background color of the window.
				cairoContext.Color = theme.BackgroundColor;
				cairoContext.Rectangle(cairoArea);
				cairoContext.Fill();

				// Reset the layout and its properties.
				lineLayoutBuffer.Width = area.Width - margins.Width;

				// Figure out the viewport area we'll be drawing.
				int offsetY = 0;

				if (verticalAdjustment != null)
				{
					offsetY += (int) verticalAdjustment.Value;
				}

				var viewArea = new Cairo.Rectangle(
					area.X, area.Y + offsetY, area.Width, area.Height);

				// Determine the line range visible in the given area.
				int startLine, endLine;
				lineLayoutBuffer.GetLineLayoutRange(
					this, viewArea, out startLine, out endLine);

				// Determine where the first line actually starts.
				int startLineY = 0;

				if (startLine > 0)
				{
					startLineY = lineLayoutBuffer.GetLineLayoutHeight(this, 0, startLine - 1);
				}

				// Go through the lines and draw each one in the correct position.
				double currentY = startLineY - offsetY;

				for (int line = startLine; line <= endLine; line++)
				{
					// Pull out the layout and style since we'll use it.
					Layout layout = lineLayoutBuffer.GetLineLayout(this, line);
					BlockStyle style = lineLayoutBuffer.GetLineStyle(this, line);

					// Get the extents for that line.
					int layoutWidth, layoutHeight;
					layout.GetPixelSize(out layoutWidth, out layoutHeight);

					// Figure out the height of the line including padding.
					double height = layoutHeight + style.Height;

					// Draw the current line along with wrapping and padding.
					DrawingUtility.DrawLayout(
						this,
						renderContext,
						new Cairo.Rectangle(TextX, currentY, TextWidth, height),
						layout,
						style);

					// Render out the margin renderers.
					margins.Draw(this, renderContext, line, new PointD(0, currentY), height);

					// Move down a line.
					currentY += height;
				}

				// Draw the caret on the screen, but only if we have focus.
				if (IsFocus)
				{
					caret.Draw(renderContext);
				}
			}

			return true;
		}

		#endregion

		#region Scrollbars

		private Adjustment verticalAdjustment;

		/// <summary>
		/// Called when the scroll adjustements are requested.
		/// </summary>
		/// <param name="hadj">The hadj.</param>
		/// <param name="vadj">The vadj.</param>
		protected override void OnSetScrollAdjustments(
			Adjustment hadj,
			Adjustment vadj)
		{
			// Determine if we need to remove ourselves from the previous adjustment
			// events.
			if (verticalAdjustment != null)
			{
				verticalAdjustment.ValueChanged -= OnVerticalAdjustment;
			}

			// And set the bounds based on our size.
			verticalAdjustment = vadj;

			SetAdjustments();

			// Add the events, if we have an adjustment.
			if (verticalAdjustment != null)
			{
				verticalAdjustment.ValueChanged += OnVerticalAdjustment;
			}
		}

		/// <summary>
		/// Called when the vertical adjustment is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnVerticalAdjustment(
			object sender,
			EventArgs args)
		{
			// Redraw the entire window.
			QueueDraw();
		}

		/// <summary>
		/// Used to set the adjustments on the scrollbars for the text editor
		/// </summary>
		private void SetAdjustments()
		{
			// We have to have a size and an adjustment.
			if (verticalAdjustment == null)
			{
				return;
			}

			// If the text width is negative, then we can't format.
			if (TextWidth < 0)
			{
				return;
			}

			// Set the line buffer's width and then request the height for all
			// the lines in the buffer.
			lineLayoutBuffer.Width = TextWidth;
			int height = lineLayoutBuffer.GetLineLayoutHeight(this, 0, -1);

			// Set the adjustments based on those values.
			verticalAdjustment.SetBounds(
				0.0,
				height,
				lineLayoutBuffer.GetLineLayoutHeight(this),
				(int) (Allocation.Height / 2.0),
				Allocation.Height);
		}

		#endregion

		#region Window Events

		/// <summary>
		/// Called when the window is realized (shown).
		/// </summary>
		protected override void OnRealized()
		{
			WidgetFlags |= WidgetFlags.Realized;
			var attributes = new WindowAttr();
			attributes.WindowType = WindowType.Child;
			attributes.X = Allocation.X;
			attributes.Y = Allocation.Y;
			attributes.Width = Allocation.Width;
			attributes.Height = Allocation.Height;
			attributes.Wclass = WindowClass.InputOutput;
			attributes.Visual = Visual;
			attributes.Colormap = Colormap;
			attributes.EventMask = (int) (Events | EventMask.ExposureMask);
			attributes.Mask = Events | EventMask.ExposureMask;

			const WindowAttributesType mask =
				WindowAttributesType.X | WindowAttributesType.Y |
				WindowAttributesType.Colormap | WindowAttributesType.Visual;
			GdkWindow = new Window(ParentWindow, attributes, mask);
			GdkWindow.UserData = Raw;
			Style = Style.Attach(GdkWindow);
			WidgetFlags &= ~WidgetFlags.NoWindow;
		}

		/// <summary>
		/// Called when the widget is resized.
		/// </summary>
		/// <param name="allocation">The allocation.</param>
		protected override void OnSizeAllocated(Rectangle allocation)
		{
			// Call the base implementation.
			base.OnSizeAllocated(allocation);

			// If we have a GdkWindow, move it.
			if (GdkWindow != null)
			{
				GdkWindow.MoveResize(allocation);
			}

			// We need to reset the buffer so it can recalculate all the widths
			// and clear any caches.
			LineLayoutBuffer.Reset();

			// Change the adjustments (scrollbars).
			SetAdjustments();

			// Force the entire widget to draw.
			QueueDraw();
		}

		/// <summary>
		/// Called when the widget is unrealized (hidden).
		/// </summary>
		protected override void OnUnrealized()
		{
			if (GdkWindow != null)
			{
				GdkWindow.UserData = IntPtr.Zero;
				GdkWindow.Destroy();
				WidgetFlags |= WidgetFlags.NoWindow;
			}

			base.OnUnrealized();
		}

		#endregion
	}
}