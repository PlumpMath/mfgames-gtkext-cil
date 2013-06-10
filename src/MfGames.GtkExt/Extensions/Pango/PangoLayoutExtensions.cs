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

		#endregion
	}
}
