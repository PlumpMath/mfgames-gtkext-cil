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

using Color=Cairo.Color;
using Rectangle=Gdk.Rectangle;
using Window=Gdk.Window;
using WindowType=Gdk.WindowType;

#endregion

namespace MfGames.GtkExt.LineTextEditor
{
	/// <summary>
	/// The primary editor control for the virtualized line text editor.
	/// </summary>
	public class TextEditor : Widget
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

		#region Screen Elements

		private readonly MarginRendererCollection margins;
		private Theme theme;

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

		protected override bool OnExposeEvent(Gdk.EventExpose e)
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
				// Paint the background color of the window.
				cairoContext.Color = theme.Selectors[Theme.BodyStyle].GetBackgroundColor();
				cairoContext.Rectangle(cairoArea);
				cairoContext.Fill();

				// Reset the layout and its properties.
				lineLayoutBuffer.Reset();
				lineLayoutBuffer.Context = PangoContext;
				lineLayoutBuffer.Width = area.Width - margins.Width;

				// Figure out which lines we can draw on the screen.
				int startLine = 0;
				int endLine = 2;

				// Go through the lines and draw each one in the correct position.
				int lineY = 0;

				for (int line = startLine; line <= endLine; line++)
				{
					// Get the layout for the current line.
					Pango.Layout layout = lineLayoutBuffer.GetLineLayout(line);
					theme.Selectors[Theme.TextStyle].SetLayout(layout);
					GdkWindow.DrawLayout(Style.TextGC(StateType.Normal), margins.Width, lineY, layout);

					// Get the extents for that line.
					int width, height;
					layout.GetPixelSize(out width, out height);

					// Render out the margin renderers.
					margins.Draw(this, cairoContext, line, 0, lineY, height);

					// Move down a line.
					lineY += height;
				}
			}

			return true;
		}

		#endregion

		#region Window Events

		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated(allocation);

			if (this.GdkWindow != null)
				this.GdkWindow.MoveResize(allocation);

			QueueDraw();
		}
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
				WindowAttributesType.X | 
				WindowAttributesType.Y |
				WindowAttributesType.Colormap |
				WindowAttributesType.Visual;
			GdkWindow = new Window(ParentWindow, attributes, mask);
			GdkWindow.UserData = Raw;
			Style = Style.Attach(GdkWindow);
			WidgetFlags &= ~WidgetFlags.NoWindow;
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