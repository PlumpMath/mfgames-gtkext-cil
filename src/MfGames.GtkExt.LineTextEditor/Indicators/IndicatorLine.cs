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

using C5;

using Cairo;

using MfGames.GtkExt.LineTextEditor.Enumerations;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Renderers;
using MfGames.GtkExt.LineTextEditor.Visuals;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Indicators
{
	/// <summary>
	/// Represents a single visible line on the indicator.
	/// </summary>
	internal class IndicatorLine
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
			TextRenderer buffer)
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
					buffer.LineBuffer.GetLineIndicators(displayContext, lineIndex);

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
		/// <param name="displayContext">The display context.</param>
		/// <param name="cairoContext">The cairo context.</param>
		/// <param name="y">The y.</param>
		/// <param name="width">The width.</param>
		public void Draw(
			IDisplayContext displayContext,
			Context cairoContext,
			double y,
			double width)
		{
			// If we don't have a color, then we don't do anything.
			if (colors == null || ratios == null)
			{
				return;
			}

			// Adjust the width to handle the gap.
			width -= (ratios.Length - 1) * displayContext.Theme.IndicatorRatioPixelGap;

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
				x += displayContext.Theme.IndicatorRatioPixelGap;
			}
		}

		#endregion
	}
}