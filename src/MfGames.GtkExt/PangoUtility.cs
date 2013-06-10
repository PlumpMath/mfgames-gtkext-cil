// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Text;

namespace MfGames.GtkExt
{
	/// <summary>
	/// Utility functions for dealing with Pango.
	/// </summary>
	public class PangoUtility
	{
		#region Methods

		/// <summary>
		/// Escapes the specified input for Pango.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public static string Escape(string input)
		{
			// If we have a blank or null string, just pass it on.
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}

			// Go through the string and come out with a marked up version.
			var buffer = new StringBuilder();

			foreach (char c in input)
			{
				switch (c)
				{
					case '&':
						buffer.Append("&amp;");
						break;
					case '<':
						buffer.Append("&lt;");
						break;
					case '>':
						buffer.Append("&gt;");
						break;
					default:
						buffer.Append(c);
						break;
				}
			}

			// Return the resulting buffer.
			return buffer.ToString();
		}

		#endregion
	}
}
