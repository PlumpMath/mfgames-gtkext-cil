// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Pango;

namespace MfGames.GtkExt.Extensions.Pango
{
	/// <summary>
	/// Defines extensions to the Pango.LayoutLine class.
	/// </summary>
	public static class PangoLayoutLineExtensions
	{
		#region Methods

		/// <summary>
		/// Gets the index of the layout line inside the layout.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <returns></returns>
		public static int GetLayoutLineIndex(this LayoutLine layoutLine)
		{
			// Go through all the lines in the layout and find the index.
			for (int index = 0;
				index < layoutLine.Layout.LineCount;
				index++)
			{
				if (layoutLine.Layout.Lines[index].StartIndex == layoutLine.StartIndex)
				{
					return index;
				}
			}

			// We can't find this layout line inside the layout.
			throw new Exception("Cannot find layout line inside layout");
		}

		/// <summary>
		/// Gets the size of the wrapped line in pixels and relative to the layout.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <param name="size">The size.</param>
		public static void GetPixelExtents(
			this LayoutLine layoutLine,
			out Rectangle size)
		{
			// Go through and populate the size regions.
			size = new Rectangle();
			size.X = 0;
			size.Width = layoutLine.Layout.Width;

			// Build up the Y coordinates of all the lines before it.
			int layoutLineIndex = layoutLine.GetLayoutLineIndex();
			int y = 0;

			for (int index = 0;
				index < layoutLineIndex;
				index++)
			{
				LayoutLine preceedingLayoutLine = layoutLine.Layout.Lines[index];
				var ink = new Rectangle();
				var logical = new Rectangle();

				preceedingLayoutLine.GetPixelExtents(ref ink, ref logical);

				y += logical.Height;
			}

			size.Y = y;

			// Figure out the height of the given wrapped line.
			var ink2 = new Rectangle();
			var logical2 = new Rectangle();

			layoutLine.GetPixelExtents(ref ink2, ref logical2);

			size.Height = logical2.Height;
		}

		/// <summary>
		/// Gets the X ranges from the given string indexes.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <param name="startStringIndex">Start index of the string.</param>
		/// <param name="endStringIndex">End index of the string.</param>
		/// <param name="ranges">The ranges.</param>
		public static void GetTranslatedXRanges(
			this LayoutLine layoutLine,
			int startStringIndex,
			int endStringIndex,
			out int[][] ranges)
		{
			int startPangoIndex =
				PangoUtility.TranslateStringToPangoIndex(
					layoutLine.Layout.Text, startStringIndex);
			int endPangoIndex =
				PangoUtility.TranslateStringToPangoIndex(
					layoutLine.Layout.Text, endStringIndex);
			layoutLine.GetXRanges(startPangoIndex, endPangoIndex, out ranges);
		}

		/// <summary>
		/// Determines whether this layout line is the last line in the layout.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <returns>
		/// 	<c>true</c> if [is last line] [the specified layout line]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsLastLineInLayout(this LayoutLine layoutLine)
		{
			Layout layout = layoutLine.Layout;
			return layout.Lines[layout.LineCount - 1].StartIndex == layoutLine.StartIndex;
		}

		/// <summary>
		/// Gets the string index for an X coordinate.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <param name="stringIndex">Index of the string.</param>
		/// <param name="trailing">if set to <c>true</c> [trailing].</param>
		/// <returns></returns>
		public static int TranslatedIndexToX(
			this LayoutLine layoutLine,
			int stringIndex,
			bool trailing)
		{
			int pangoIndex =
				PangoUtility.TranslateStringToPangoIndex(
					layoutLine.Layout.Text, stringIndex);
			return layoutLine.IndexToX(pangoIndex, trailing);
		}

		/// <summary>
		/// Gets the string index for a given X coordinate (in Pango units).
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <param name="x">The x.</param>
		/// <param name="stringIndex">Index of the string.</param>
		/// <param name="trailing">The trailing.</param>
		/// <returns></returns>
		public static bool XToTranslatedIndex(
			this LayoutLine layoutLine,
			int x,
			out int stringIndex,
			out int trailing)
		{
			int pangoIndex;
			bool results = layoutLine.XToIndex(x, out pangoIndex, out trailing);
			stringIndex = PangoUtility.TranslatePangoToStringIndex(
				layoutLine.Layout.Text, pangoIndex);
			return results;
		}

		#endregion
	}
}
