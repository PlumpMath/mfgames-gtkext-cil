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
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Margins;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

using CairoHelper=Gdk.CairoHelper;
using Context=Cairo.Context;
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
			layout.Width = Units.FromPixels(TextWidth - style.Width);
		}

		#endregion

		#region Display

		private readonly MarginRendererCollection margins;
		private Theme theme;

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

		#endregion

		#region Rendering Events

		/// <summary>
		/// Gets the width of the area that can be used for rendering text.
		/// </summary>
		/// <value>The width of the text.</value>
		private int TextWidth
		{
			get { return Allocation.Width - margins.Width; }
		}

		/// <summary>
		/// Called when the widget is exposed or drawn.
		/// </summary>
		/// <param name="e">The e.</param>
		/// <returns></returns>
		protected override bool OnExposeEvent(EventExpose e)
		{
			Console.WriteLine(
				"{0} expose {1},{2} {3}x{4}",
				(long) (DateTime.Now - createdTimestamp).TotalMilliseconds,
				e.Area.X,
				e.Area.Y,
				e.Area.Width,
				e.Area.Height);

			Rectangle area = e.Region.Clipbox;
			var cairoArea = new Cairo.Rectangle(area.X, area.Y, area.Width, area.Height);

			using (Context cairoContext = CairoHelper.Create(e.Window))
			{
				// Create a render context.
				var renderContext = new RenderContext(cairoContext);

				// Paint the background color of the window.
				var backgroundColor = theme.BlockStyles[Theme.BodyStyle].GetBackgroundColor();

				if (backgroundColor.HasValue)
				{
					cairoContext.Color = backgroundColor.Value;
					cairoContext.Rectangle(cairoArea);
					cairoContext.Fill();
				}

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
				int currentY = startLineY - offsetY;

				for (int line = startLine; line <= endLine; line++)
				{
					// Get the layout and style for the current line.
					Layout layout = lineLayoutBuffer.GetLineLayout(this, line);
					BlockStyle style = lineLayoutBuffer.GetLineStyle(this, line);
					Spacing styleMargins = style.GetMargins();
					Spacing stylePadding = style.GetPadding();

					// Get the extents for that line.
					int layoutWidth, layoutHeight;
					layout.GetPixelSize(out layoutWidth, out layoutHeight);

					var height =
						(int) (layoutHeight + styleMargins.Height + stylePadding.Height);

					// Draw out the line.
					GdkWindow.DrawLayout(
						Style.TextGC(StateType.Normal), margins.Width, currentY, layout);

					// Render out the margin renderers.
					margins.Draw(this, renderContext, line, new PointD(0, currentY), height);

					// Move down a line.
					currentY += height;
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
				lineLayoutBuffer.GetTextLayoutLineHeight(this),
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