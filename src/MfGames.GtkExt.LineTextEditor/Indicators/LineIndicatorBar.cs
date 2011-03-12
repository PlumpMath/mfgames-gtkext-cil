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
using System.Diagnostics;
using System.Threading;

using C5;

using Cairo;

using Gdk;

using GLib;

using Gtk;

using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Renderers;
using MfGames.GtkExt.LineTextEditor.Visuals;
using MfGames.Locking;

using Rectangle=Gdk.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Indicators
{
	/// <summary>
	/// Implements a visual bar of indicators from a given line buffer.
	/// </summary>
	public class LineIndicatorBar : DrawingArea
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LineIndicatorBar"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public LineIndicatorBar(
			IDisplayContext displayContext)
			: this(displayContext, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LineIndicatorBar"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndicatorBuffer">The line indicator buffer.</param>
		public LineIndicatorBar(
			IDisplayContext displayContext,
			TextRenderer lineIndicatorBuffer)
		{
			// Save the control variables.
			DisplayContext = displayContext;
			LineIndicatorBuffer = lineIndicatorBuffer;

			// Start the background update.
			sync = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			StartBackgroundUpdate();
		}

		#endregion

		#region Indicator Lines

		private ArrayList<IndicatorLine> indicatorLines;

		private TextRenderer lineIndicatorBuffer;

		/// <summary>
		/// Gets or sets the line indicator buffer associated with this view.
		/// </summary>
		/// <value>The line indicator buffer.</value>
		public TextRenderer LineIndicatorBuffer
		{
			get { return lineIndicatorBuffer; }
			set
			{
				// Check to see if we have a buffer already.
				if (lineIndicatorBuffer != null)
				{
					lineIndicatorBuffer.LineChanged -= OnLineChanged;
					lineIndicatorBuffer.LinesInserted -= OnBufferChanged;
					lineIndicatorBuffer.LinesDeleted -= OnBufferChanged;
				}

				// Set the new indicator buffer.
				lineIndicatorBuffer = value;

				// If we have a new indicator buffer, attach events.
				if (lineIndicatorBuffer != null)
				{
					lineIndicatorBuffer.LineChanged += OnLineChanged;
					lineIndicatorBuffer.LinesInserted += OnBufferChanged;
					lineIndicatorBuffer.LinesDeleted += OnBufferChanged;
				}

				// Rebuild the lines in the buffer.
				AssignLines();
				QueueDraw();
			}
		}

		/// <summary>
		/// Goes through the buffer lines and assigns those lines to the
		/// indicator lines.
		/// </summary>
		private void AssignLines()
		{
			// Go through all the lines and put them in a known and reset state.
			// Reset the lines we're using and clear out the lines we aren't.
			for (int indicatorLineIndex = 0;
				 indicatorLineIndex < visibleLineCount;
				 indicatorLineIndex++)
			{
				IndicatorLine indicatorLine = indicatorLines[indicatorLineIndex];
				
				indicatorLine.Visible = false;
				indicatorLine.StartLineIndex = -1;
				indicatorLine.EndLineIndex = -1;
			}

			// If we don't have any lines or if we have no height, then don't
			// do anything.
			if (visibleLineCount == 0 || lineIndicatorBuffer == null)
			{
				return;
			}

			// Check for lines in the buffer. If we have none, then we can't
			// do anything.
			int lineCount = DisplayContext.LineBuffer.LineCount;

			if (lineCount == 0)
			{
				return;
			}

			// Figure out how many indicator lines we'll be using.
			int bufferLinesPerIndicatorLine = BufferLinesPerIndicatorLine;
			int indicatorLinesUsed = 1 +
									 DisplayContext.LineBuffer.LineCount /
			                         bufferLinesPerIndicatorLine;

			// Reset the lines we're using and clear out the lines we aren't.
			for (int indicatorLineIndex = 0;
			     indicatorLineIndex < visibleLineCount;
			     indicatorLineIndex++)
			{
				IndicatorLine indicatorLine = indicatorLines[indicatorLineIndex];

				if (indicatorLineIndex < indicatorLinesUsed)
				{
					indicatorLine.Reset();
					indicatorLine.StartLineIndex = indicatorLineIndex *
					                               bufferLinesPerIndicatorLine;
					indicatorLine.EndLineIndex = Math.Min(
						lineCount - 1, (indicatorLineIndex + 1) * bufferLinesPerIndicatorLine - 1);
				}
				else
				{
					break;
				}
			}

			// Since we assigned the lines, we start updating the idle.
			StartBackgroundUpdate();
		}

		/// <summary>
		/// Called when the buffer changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnBufferChanged(
			object sender,
			EventArgs args)
		{
			AssignLines();
		}

		/// <summary>
		/// Called when a single line changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnLineChanged(
			object sender,
			LineChangedArgs args)
		{
			// Reset the line indicator so it reparses later.
			int indicatorLineIndex = args.LineIndex / BufferLinesPerIndicatorLine;

			indicatorLines[indicatorLineIndex].Reset();

			// Start the background update, if it isn't already.
			StartBackgroundUpdate();
		}

		/// <summary>
		/// Reallocates the indicator line objects.
		/// </summary>
		private void ReallocateLines()
		{
			// Create a new indicator line which is then populated
			// with null values to reset the bar.
			indicatorLines = new ArrayList<IndicatorLine>(visibleLineCount);

			for (int index = 0; index < visibleLineCount; index++)
			{
				indicatorLines.Add(new IndicatorLine());
			}

			// Assign the lines, since we may have changed.
			AssignLines();
		}

		#endregion

		#region Drawing

		/// <summary>
		/// Keeps track of the last indicator line updated.
		/// </summary>
		private int lastIndicatorLineUpdated;

		private int visibleLineCount;

		/// <summary>
		/// Gets the lines per indicator line.
		/// </summary>
		/// <value>The wrapped lines per indicator line.</value>
		protected int BufferLinesPerIndicatorLine
		{
			get
			{
				return Math.Max(
					1,
					(int)
					Math.Ceiling((double) DisplayContext.LineBuffer.LineCount / visibleLineCount));
			}
		}

		/// <summary>
		/// Gets or sets the display context.
		/// </summary>
		/// <value>The display context.</value>
		public IDisplayContext DisplayContext { get; private set; }

		/// <summary>
		/// Gets the theme associated with this bar.
		/// </summary>
		/// <value>The theme.</value>
		public Theme Theme
		{
			[DebuggerStepThrough]
			get { return DisplayContext.Theme; }
		}

		/// <summary>
		/// Gets the number of lines that can be drawn.
		/// </summary>
		/// <value>The visible line count.</value>
		protected int VisibleLineCount
		{
			[DebuggerStepThrough]
			get { return visibleLineCount; }

			private set
			{
				// Save the visible line count.
				int oldLineCount = visibleLineCount;
				visibleLineCount = value;

				// Reallocate the size of the array, if it is different.
				if (oldLineCount == visibleLineCount)
				{
					return;
				}

				// Reallocate the cached indicator lines.
				ReallocateLines();
			}
		}

		#endregion

		#region Gtk

		private readonly ReaderWriterLockSlim sync;
		private bool idleRunning;

		/// <summary>
		/// Called when the bar is exposed (drawn).
		/// </summary>
		/// <param name="exposeEvent">The drawing event..</param>
		/// <returns></returns>
		protected override bool OnExposeEvent(EventExpose exposeEvent)
		{
			// Figure out the area we are rendering into.
			Rectangle area = exposeEvent.Region.Clipbox;
			var cairoArea = new Cairo.Rectangle(area.X, area.Y, area.Width, area.Height);

			using (Context cairoContext = CairoHelper.Create(exposeEvent.Window))
			{
				// Create a render context.
				var renderContext = new RenderContext(cairoContext);
				renderContext.RenderRegion = cairoArea;

				// Paint the background color of the window.
				cairoContext.Color = Theme.IndicatorBackgroundColor;
				cairoContext.Rectangle(cairoArea);
				cairoContext.Fill();

				// Draw all the indicator lines on the display.
				double y = 0.5;

				cairoContext.LineWidth = DisplayContext.Theme.IndicatorPixelHeight;
				cairoContext.Antialias = Antialias.None;

				for (int index = 0; index < VisibleLineCount; index++)
				{
					// Make sure the line has been processed and it visible.
					IndicatorLine indicatorLine = indicatorLines[index];

					if (!indicatorLine.NeedIndicators && indicatorLine.Visible)
					{
						indicatorLine.Draw(DisplayContext, cairoContext, y, Allocation.Width);
					}

					// Shift the y-coordinate down.
					y += DisplayContext.Theme.IndicatorPixelHeight;
				}
			}

			// Return the render.
			return base.OnExposeEvent(exposeEvent);
		}

		/// <summary>
		/// Called when the GUI is idle and we can finish updating.
		/// </summary>
		/// <returns>True if this idle should remain in effect.</returns>
		private bool OnIdle()
		{
			// Keep track of our start time since we want to limit how long
			// we are in this function. Using UtcNow is more efficient than
			// local time because it does no time zone processing.
			const int maximumProcessTime = 100;
			DateTime start = DateTime.UtcNow;

			try
			{
				// Look for lines that need to be populated. We can do this since
				// we are on the GUI thread.
				bool startedAtBeginning = lastIndicatorLineUpdated == 0;

				for (int index = lastIndicatorLineUpdated;
				     index < indicatorLines.Count;
				     index++)
				{
					// If we need data, then do something.
					IndicatorLine indicatorLine = indicatorLines[index];

					if (indicatorLine.NeedIndicators)
					{
						// We need to update this indicator.
						indicatorLine.Update(DisplayContext, lineIndicatorBuffer);
					}

					// Update the last indicator update.
					lastIndicatorLineUpdated = index;

					// Check to see if we exceeded our time.
					TimeSpan elapsed = DateTime.UtcNow - start;

					if (elapsed.TotalMilliseconds > maximumProcessTime)
					{
						return true;
					}
				}

				// If we didn't start at the beginning, then we start over to see if
				// there are more that need updating. Otherwise, we are done.
				if (startedAtBeginning)
				{
					// We set the flag to indicate we aren't running an idle
					// function and detach from GLib's idle processing. This is
					// done to save CPU cycles when nothing changes.
					using (new WriteLock(sync))
					{
						idleRunning = false;
						return false;
					}
				}
				else
				{
					lastIndicatorLineUpdated = 0;
				}
			}
			finally
			{
				// In all of these cases, we need to queue a redraw.
				QueueDraw();
			}

			return true;
		}

		/// <summary>
		/// Called when the widget is resized.
		/// </summary>
		/// <param name="allocation">The allocation.</param>
		protected override void OnSizeAllocated(Rectangle allocation)
		{
			// Call the base implementation.
			base.OnSizeAllocated(allocation);

			// Determine how many lines we can show on the widget. This is the 
			// height divided by the height of each indicator line.
			VisibleLineCount = Allocation.Height /
			                   DisplayContext.Theme.IndicatorPixelHeight;
		}

		/// <summary>
		/// Starts the background update of the elements if it isn't already
		/// running.
		/// </summary>
		private void StartBackgroundUpdate()
		{
			using (new WriteLock(sync))
			{
				// If the idle is already running, then we don't need to worry
				// about the idle. If it isn't, then set the flag and attach
				// the idle function to the Gtk# loop.
				if (!idleRunning)
				{
					idleRunning = true;
					Idle.Add(OnIdle);
				}
			}
		}

		#endregion
	}
}