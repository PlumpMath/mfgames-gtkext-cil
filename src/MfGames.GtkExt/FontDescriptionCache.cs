// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using Pango;

namespace MfGames.GtkExt
{
	/// <summary>
	/// Implements a basic font change for Pango fonts.
	/// </summary>
	public static class FontDescriptionCache
	{
		#region Methods

		/// <summary>
		/// Loads and caches a font description from the given name.
		/// </summary>
		/// <param name="fontName">Name of the font.</param>
		/// <returns></returns>
		public static FontDescription GetFontDescription(string fontName)
		{
			// Check to see if the cache already has it.
			if (!fonts.ContainsKey(fontName))
			{
				fonts[fontName] = FontDescription.FromString(fontName);
			}

			// Return the cached font.
			return fonts[fontName];
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="FontDescriptionCache"/> class.
		/// </summary>
		static FontDescriptionCache()
		{
			fonts = new Dictionary<string, FontDescription>();
		}

		#endregion

		#region Fields

		private static readonly Dictionary<string, FontDescription> fonts;

		#endregion
	}
}
