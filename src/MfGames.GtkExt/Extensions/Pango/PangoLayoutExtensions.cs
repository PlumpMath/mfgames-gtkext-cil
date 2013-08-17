// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Pango;

namespace MfGames.GtkExt.Extensions.Pango
{
	/// <summary>
	/// Contains various utility extensions for Layout.
	/// </summary>
	public static class PangoLayoutExtensions
	{
		#region Methods

		/// <summary>
		/// Gets the pixel height of a layout.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <returns></returns>
		public static int GetPixelHeight(this Layout layout)
		{
			int width,
				height;

			layout.GetPixelSize(out width, out height);

			return height;
		}

		/// <summary>
		/// Gets the cursor position inside the layout.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <param name="stringIndex">The string index into the layout text.</param>
		/// <param name="strongPosition">The resulting strong position.</param>
		/// <param name="weakPosition">The resulting weak position.</param>
		public static void GetTranslatedCursorPos(
			this Layout layout,
			int stringIndex,
			out Rectangle strongPosition,
			out Rectangle weakPosition)
		{
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(
				layout.Text, stringIndex);

			layout.GetCursorPos(pangoIndex, out strongPosition, out weakPosition);
		}

		/// <summary>
		/// Gets the line X coordinate for a given index.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <param name="stringIndex">The string index of the string.</param>
		/// <param name="trailing">if set to <c>true</c> the position is trailing the glyph.</param>
		/// <param name="line">The resulting logical line in the layout.</param>
		/// <param name="x">The resulting x coordinate (in Pango units) in the layout.</param>
		public static void TranslatedIndexToLineX(
			this Layout layout,
			int stringIndex,
			bool trailing,
			out int line,
			out int x)
		{
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(
				layout.Text, stringIndex);

			layout.IndexToLineX(pangoIndex, trailing, out line, out x);
		}

		/// <summary>
		/// Get the rectangle position of the given index.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <param name="stringIndex">Index of the string.</param>
		/// <returns></returns>
		public static Rectangle TranslatedIndexToPos(
			this Layout layout,
			int stringIndex)
		{
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(
				layout.Text, stringIndex);
			Rectangle translatedIndexToPos = layout.IndexToPos(pangoIndex);

			return translatedIndexToPos;
		}

		/// <summary>
		/// Translates the X, Y coordinates to a character index.
		/// </summary>
		/// <param name="layout">The layout.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="stringIndex">The resulting index into the string.</param>
		/// <param name="trailing">The trailing.</param>
		/// <returns></returns>
		public static bool XyToTranslatedIndex(
			this Layout layout,
			int x,
			int y,
			out int stringIndex,
			out int trailing)
		{
			// Grab the results fromt he Pango index.
			int pangoIndex;
			bool results = layout.XyToIndex(x, y, out pangoIndex, out trailing);

			// Translate the Pango index.
			stringIndex = PangoUtility.TranslatePangoToStringIndex(
				layout.Text, pangoIndex);

			// Return the results.
			return results;
		}

		#endregion
	}
}
