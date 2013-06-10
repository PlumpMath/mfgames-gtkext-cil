// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Cairo;

namespace MfGames.GtkExt
{
	/// <summary>
	/// Contains various useful utility methods for Cairo.
	/// </summary>
	public static class CairoUtility
	{
		#region Methods

		/// <summary>
		/// Converts the Cairo color into a RGB hex string.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns></returns>
		public static string ToRgbHexString(this Color color)
		{
			return String.Format(
				"{0:X2}{1:X2}{2:X2}",
				(int) Math.Round(color.R * 255),
				(int) Math.Round(color.G * 255),
				(int) Math.Round(color.B * 255));
		}

		#endregion
	}
}
