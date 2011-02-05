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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using C5;

using Cairo;

using Gdk;

using GLib;

using Gtk;

using MfGames.GtkExt.LineTextEditor.Enumerations;
using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;
using MfGames.Locking;

using Pango;

using CairoHelper=Gdk.CairoHelper;
using Color=Cairo.Color;
using Context=Cairo.Context;
using Layout=Pango.Layout;
using Rectangle=Gdk.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor
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
		/// <param name="lineIndicatorBuffer">The line indicator buffer.</param>
		public LineIndicatorBar(
			IDisplayContext displayContext,
			ILineIndicatorBuffer lineIndicatorBuffer)
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

		private ILineIndicatorBuffer lineIndicatorBuffer;

		/// <summary>
		/// Gets or sets the line indicator buffer associated with this view.
		/// </summary>
		/// <value>The line indicator buffer.</value>
		public ILineIndicatorBuffer LineIndicatorBuffer
		{
			get { return lineIndicatorBuffer; }
			private set
			{
				lineIndicatorBuffer = value;

				lineIndicatorBuffer.LineChanged += OnLineChanged;
				lineIndicatorBuffer.LinesInserted += OnBufferChanged;
				lineIndicatorBuffer.LinesDeleted += OnBufferChanged;

				AssignLines();
			}
		}

		/// <summary>
		/// Goes through the buffer lines and assigns those lines to the
		/// indicator lines.
		/// </summary>
		private void AssignLines()
		{
			// If we don't have any lines or if we have no height, then don't
			// do anything.
			int lineCount = lineIndicatorBuffer.LineCount;

			if (visibleLineCount == 0 || lineIndicatorBuffer == null || lineCount == 0)
			{
				return;
			}

			// Figure out how many indicator lines we'll be using.
			int bufferLinesPerIndicatorLine = BufferLinesPerIndicatorLine;
			int indicatorLinesUsed = 1 +
			                         lineIndicatorBuffer.LineCount /
									 bufferLinesPerIndicatorLine;

			// Reset the lines we're using and clear out the lines we aren't.
			for (int indicatorLineIndex = 0; indicatorLineIndex < visibleLineCount; indicatorLineIndex++)
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
					indicatorLine.Visible = false;
					indicatorLine.StartLineIndex = -1;
					indicatorLine.EndLineIndex = -1;
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
		/// Gets or sets the display context.
		/// </summary>
		/// <value>The display context.</value>
		public IDisplayContext DisplayContext { get; private set; }

		/// <summary>
		/// Gets or sets the height of an individual indicator line. This will
		/// determine how many actual wrapped lines will be combined into a
		/// single displayed line.
		/// </summary>
		/// <value>The height of the indicator pixel.</value>
		public int IndicatorPixelHeight { get; set; }

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
					Math.Ceiling((double) lineIndicatorBuffer.LineCount / visibleLineCount));
			}
		}

		#endregion

		#region Gtk

		private bool idleRunning;
		private readonly ReaderWriterLockSlim sync;

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

				cairoContext.LineWidth = IndicatorPixelHeight;
				cairoContext.Antialias = Antialias.None;

				for (int index = 0; index < VisibleLineCount; index++)
				{
					// Make sure the line has been processed and it visible.
					IndicatorLine indicatorLine = indicatorLines[index];

					if (!indicatorLine.NeedIndicators && indicatorLine.Visible)
					{
						indicatorLine.Draw(cairoContext, y, Allocation.Width);
					}

					// Shift the y-coordinate down.
					y += IndicatorPixelHeight;
				}
			}

			// Return the render.
			return base.OnExposeEvent(exposeEvent);
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
			VisibleLineCount = Allocation.Height / IndicatorPixelHeight;
		}

		#endregion

		#region Indicators

		/// <summary>
		/// Represents a single visible line on the indicator.
		/// </summary>
		private class IndicatorLine
		{
			#region Indicators

			private Color[] colors;
			private ArrayList<ILineIndicator> indicators;
			private double[] ratios;

			/// <summary>
			/// Gets or sets the indexes of the last line.
			/// </summary>
			/// <value>The end index of the line.</value>
			public int EndLineIndex { private get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this instance needs to
			/// be populated with indicators.
			/// </summary>
			/// <value><c>true</c> if [need indicators]; otherwise, <c>false</c>.</value>
			public bool NeedIndicators { get; private set; }

			/// <summary>
			/// Gets or sets the index of the first line.
			/// </summary>
			/// <value>The start index of the line.</value>
			public int StartLineIndex { private get; set; }

			/// <summary>
			/// Resets this indicator so we can query for more information.
			/// </summary>
			public void Reset()
			{
				colors = null;
				ratios = null;
				indicators = null;

				Visible = false;
				NeedIndicators = true;
			}

			/// <summary>
			/// Sets the color ratios based on the indicators.
			/// </summary>
			/// <param name="displayContext">The display context.</param>
			private void SetColorRatios(IDisplayContext displayContext)
			{
				// Create a dictionary of the individual styles and their counts.
				Theme theme = displayContext.Theme;
				var styles = new HashDictionary<string, IndicatorStyle>();
				var counts = new HashDictionary<string, int>();
				double total = 0;

				foreach (var indicator in indicators)
				{
					// Make sure this indicator has a style.
					string styleName = indicator.LineIndicatorStyle;

					if (!theme.IndicatorStyles.Contains(styleName))
					{
						continue;
					}

					// Check to see if we already have the style.
					if (!styles.Contains(styleName))
					{
						IndicatorStyle style = theme.IndicatorStyles[styleName];
						styles[styleName] = style;
					}

					// Increment the counter for this style.
					if (counts.Contains(styleName))
					{
						counts[styleName]++;
					}
					else
					{
						counts[styleName] = 1;
					}

					total++;
				}

				// Get a list of sorted styles, ordered by priority.
				var sortedStyles = new ArrayList<IndicatorStyle>();

				sortedStyles.AddAll(styles.Values);
				sortedStyles.Sort();

				// Go through the styles and build up the ratios and colors.
				colors = new Color[sortedStyles.Count];
				ratios = new double[sortedStyles.Count];

				for (int index = 0; index < sortedStyles.Count; index++)
				{
					IndicatorStyle style = sortedStyles[index];

					colors[index] = style.Color;
					ratios[index] = counts[style.Name] / total;
				}

				// This line is visible.
				Visible = true;
			}

			/// <summary>
			/// Finds the color of the highest priority style and returns it.
			/// </summary>
			/// <param name="displayContext">The display context.</param>
			private void SetHighestColor(IDisplayContext displayContext)
			{
				// Go through all the indicators and look for a style.
				Theme theme = displayContext.Theme;
				IndicatorStyle highestStyle = null;

				for (int index = 0; index < indicators.Count; index++)
				{
					// Try to get a style for this indicator. If we don't have
					// one, then just skip it.
					ILineIndicator indicator = indicators[index];

					if (!theme.IndicatorStyles.Contains(indicator.LineIndicatorStyle))
					{
						// No style, nothing to render.
						continue;
					}

					// Get the style for this indicator.
					IndicatorStyle style = theme.IndicatorStyles[indicator.LineIndicatorStyle];

					if (highestStyle == null || highestStyle.Priority < style.Priority)
					{
						highestStyle = style;
					}
				}

				// If we don't have a style at the bottom, we aren't visible.
				Visible = highestStyle != null;

				if (highestStyle != null)
				{
					colors = new[] { highestStyle.Color };
					ratios = new[] { 1.0 };
				}
				else
				{
					colors = null;
					ratios = null;
				}
			}

			/// <summary>
			/// Updates the indicator line with contents from the display.
			/// </summary>
			/// <param name="displayContext">The display context.</param>
			/// <param name="buffer">The buffer.</param>
			public void Update(
				IDisplayContext displayContext,
				ILineIndicatorBuffer buffer)
			{
				// Clear out our indicators and reset our fields.
				Reset();

				// If the start and end are negative, then don't do anything.
				if (StartLineIndex < 0)
				{
					return;
				}

				// Gather up all the indicators for the lines assigned to this
				// indicator line.
				for (int lineIndex = StartLineIndex; lineIndex <= EndLineIndex; lineIndex++)
				{
					// Get the line indicators for that character range and add
					// them to the current range of indicators.
					IEnumerable<ILineIndicator> lineIndicators =
						buffer.GetLineIndicators(displayContext, lineIndex);

					if (lineIndicators != null)
					{
						if (indicators == null)
						{
							indicators = new ArrayList<ILineIndicator>();
						}

						indicators.AddAll(lineIndicators);
					}
				}

				// If we have indicators created, but they are empty, null out
				// the field again.
				if (indicators != null && indicators.Count == 0)
				{
					indicators = null;
				}

				// We have the indicators, so set our flag.
				NeedIndicators = false;

				// If we don't have any indicators, then we aren't showing anything.
				if (indicators == null)
				{
					return;
				}

				// If we have indicators, we need to figure out how to render
				// this line.
				switch (displayContext.Theme.IndicatorRenderStyle)
				{
					case IndicatorRenderStyle.Highest:
						// Find the most important style and use that.
						SetHighestColor(displayContext);
						break;

					case IndicatorRenderStyle.Ratio:
						// Build up a ratio of all the indicators and keep the
						// list.
						SetColorRatios(displayContext);
						break;

					default:
						throw new Exception(
							"Cannot identify indicator render style: " +
							displayContext.Theme.IndicatorRenderStyle);
				}
			}

			#endregion

			#region Drawing

			/// <summary>
			/// Determines if this indicator is visible.
			/// </summary>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			public bool Visible { get; set; }

			/// <summary>
			/// Draws the specified indicator line to the context.
			/// </summary>
			/// <param name="cairoContext">The cairo context.</param>
			/// <param name="y">The y.</param>
			/// <param name="width">The width.</param>
			public void Draw(
				Context cairoContext,
				double y,
				double width)
			{
				// If we don't have a color, then we don't do anything.
				if (colors == null || ratios == null)
				{
					return;
				}

				// Go through the the various colors/ratios and render each one.
				double x = 0;

				for (int index = 0; index < ratios.Length; index++)
				{
					// Pull out the ratio and adjust it for the width.
					double ratio = ratios[index];
					double currentWidth = ratio * width;

					// Draw out the line.
					cairoContext.Color = colors[index];

					cairoContext.MoveTo(x, y);
					cairoContext.LineTo(x + currentWidth, y);
					cairoContext.Stroke();

					// Shift the X coordinate over.
					x += currentWidth;
				}
			}

			#endregion
		}

		#endregion
	}
}