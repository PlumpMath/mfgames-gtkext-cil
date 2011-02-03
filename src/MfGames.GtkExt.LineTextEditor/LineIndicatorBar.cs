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

using C5;

using Cairo;

using Gdk;

using GLib;

using Gtk;

using MfGames.GtkExt.LineTextEditor.Enumerations;
using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

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

			// Hook up to the idle function.
			Idle.Add(OnIdle);
		}

		#endregion

		#region Buffers

		#endregion

		#region Drawing

		private ArrayList<IndicatorLine> indicatorLines;

		/// <summary>
		/// Keeps track of the last indicator line updated.
		/// </summary>
		private int lastIndicatorLineUpdated;

		private ILineIndicatorBuffer lineIndicatorBuffer;

		/// <summary>
		/// Internal flag to determine if we need to update our indicators.
		/// </summary>
		private bool needUpdate;

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
		/// Gets or sets the wrapped line count.
		/// </summary>
		/// <value>The wrapped line count.</value>
		protected int WrappedLineCount { get; set; }

		/// <summary>
		/// Gets the wrapped lines per indicator line.
		/// </summary>
		/// <value>The wrapped lines per indicator line.</value>
		protected int WrappedLinesPerIndicatorLine
		{
			get { return Math.Max(1, WrappedLineCount / visibleLineCount); }
		}

		/// <summary>
		/// Goes through the buffer lines and assigns those lines to the
		/// indicator lines.
		/// </summary>
		private void AssignLines()
		{
			// If we don't have any lines or if we have no height, then don't
			// do anything.
			if (visibleLineCount == 0 || lineIndicatorBuffer == null)
			{
				return;
			}

			WrappedLineCount = lineIndicatorBuffer.GetWrappedLineCount(DisplayContext);

			if (WrappedLineCount == 0)
			{
				return;
			}

			// If we are assigning lines, we need an update.
			needUpdate = true;

			// Figure out how many buffer lines we need to put into a single
			// indicator line.
			int wrappedLinesPerIndicatorLine = WrappedLinesPerIndicatorLine;

			// Go through indicator lines and set up their indexes.
			int wrappedLineIndex = 0;

			for (int indicatorIndex = 0;
			     indicatorIndex < visibleLineCount;
			     indicatorIndex++)
			{
				// If we are beyond the wrapped line count, we don't show the
				// line at all.
				IndicatorLine indicatorLine = indicatorLines[indicatorIndex];

				if (wrappedLineIndex > WrappedLineCount)
				{
					// We are beyond the limits.
					indicatorLine.StartWrappedLineIndex = -1;
					indicatorLine.EndWrappedLineIndex = -1;
					indicatorLine.NeedIndicators = false;
					indicatorLine.Visible = false;
				}
				else
				{
					// Get the wrapped line index range we are using.
					indicatorLine.StartWrappedLineIndex = wrappedLineIndex;
					indicatorLine.EndWrappedLineIndex = Math.Min(
						WrappedLineCount, wrappedLineIndex + wrappedLinesPerIndicatorLine) - 1;
					indicatorLine.Reset();
				}

				// Increment the line index for the next line.
				wrappedLineIndex += wrappedLinesPerIndicatorLine;
			}
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
		/// Called when the GUI is idle and we can finish updating.
		/// </summary>
		/// <returns>True if this idle should remain in effect.</returns>
		private bool OnIdle()
		{
			// Check to see if we need to update at least one.
			if (!needUpdate)
			{
				return true;
			}

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
					// We need to keep processing this even though we have nothing to do.
					needUpdate = false;
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
		/// Called when a single line changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnLineChanged(
			object sender,
			LineChangedArgs args)
		{
			// Figure out which wrapped line indexes are represented by this
			// line.
			int startWrappedLineIndex, endWrappedLineIndex;

			lineIndicatorBuffer.GetWrappedLineIndexes(
				DisplayContext,
				args.LineIndex,
				out startWrappedLineIndex,
				out endWrappedLineIndex);

			// Use the wrapped indexes to determine which indicator lines need
			// to be reset.
			int startIndicatorIndex = Math.Max(
				0, startWrappedLineIndex / WrappedLinesPerIndicatorLine - 1);
			int endIndicatorIndex = Math.Min(
				visibleLineCount - 1, endWrappedLineIndex / WrappedLinesPerIndicatorLine + 1);

			for (int indicatorIndex = startIndicatorIndex;
			     indicatorIndex <= endIndicatorIndex;
			     indicatorIndex++)
			{
				indicatorLines[indicatorIndex].Reset();
			}

			// Since we reset a line, have the idle thread update.
			needUpdate = true;
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

		#region Indicator

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
			/// Gets or sets the indexes of the last wrapped line.
			/// </summary>
			/// <value>The end index of the line.</value>
			public int EndWrappedLineIndex { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this instance needs to
			/// be populated with indicators.
			/// </summary>
			/// <value><c>true</c> if [need indicators]; otherwise, <c>false</c>.</value>
			public bool NeedIndicators { get; set; }

			/// <summary>
			/// Gets or sets the index of the first wrapped line.
			/// </summary>
			/// <value>The start index of the line.</value>
			public int StartWrappedLineIndex { get; set; }

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
					Color = highestStyle.Color;
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
				if (StartWrappedLineIndex < 0 || EndWrappedLineIndex < 0)
				{
					return;
				}

				// Gather up all the indicators for the wrapped lines assigned
				// to this line indicator.
				for (int wrappedLineIndex = StartWrappedLineIndex;
				     wrappedLineIndex <= EndWrappedLineIndex;
				     wrappedLineIndex++)
				{
					// Get the line and layout we are working with.
					int layoutLineIndex;
					int lineIndex = buffer.GetLineIndex(
						displayContext, wrappedLineIndex, out layoutLineIndex);
					Layout layout = buffer.GetLineLayout(displayContext, lineIndex);

					// Figure out the string index of the layout line.
					LayoutLine layoutLine = layout.Lines[layoutLineIndex];
					int startCharacterIndex = layoutLine.StartIndex;
					int endCharacterIndex = startCharacterIndex + layoutLine.Length;

					// Get the line indicators for that character range and add
					// them to the current range of indicators.
					IEnumerable<ILineIndicator> lineIndicators =
						buffer.GetLineIndicators(
							displayContext, lineIndex, startCharacterIndex, endCharacterIndex);

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
						ratios = new double[] { 1 };
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
			/// Gets or sets the color for the indicator line.
			/// </summary>
			/// <value>The color.</value>
			public Color Color { get; private set; }

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