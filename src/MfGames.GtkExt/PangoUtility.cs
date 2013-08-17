// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
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

		/// <summary>
		/// Converts a Unicode index from Pango into a C# character index.
		/// </summary>
		/// <param name="text">The line text.</param>
		/// <param name="pangoIndex">The unicode character.</param>
		/// <returns></returns>
		public static int TranslatePangoToStringIndex(
			string text,
			int pangoIndex)
		{
			// Go through and figure out the appropriate index.
			int characterIndex = 0;

			foreach (int c in text)
			{
				if (c < 128)
				{
					pangoIndex -= 1;
				}
				else if (c < 2048)
				{
					pangoIndex -= 2;
				}
				else if (c < 65536)
				{
					pangoIndex -= 3;
				}
				else
				{
					pangoIndex -= 4;
				}

				// If we are zero or less, we're done.
				if (pangoIndex < 0)
				{
					break;
				}

				// Increment the character index.
				characterIndex++;
			}

			return characterIndex;
		}

		/// <summary>
		/// Calculates the UTF-8-based character index from the C#  character
		/// index.
		/// </summary>
		/// <param name="text">The line text to parse for the index.</param>
		/// <param name="stringIndex">The character index into the given string.</param>
		/// <returns>The Unicode character index into the underlying C string.</returns>
		public static int TranslateStringToPangoIndex(
			string text,
			int stringIndex)
		{
			// Make sure we have a valid arguments.
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			if (stringIndex < 0)
			{
				throw new ArgumentOutOfRangeException(
					"stringIndex",
					string.Format(
						"Supplied index ({0}) cannot be less than zero.", stringIndex));
			}

			// If the string index is greater than the length of the string,
			// then we don't allow it. This does mean that the index can equal
			// the length, which means a position to the right of the last
			// character.
			if (stringIndex > text.Length)
			{
				throw new ArgumentOutOfRangeException(
					"stringIndex",
					string.Format(
						"Provided index ({0}) cannot be greater than the length of the text ({1}).",
						stringIndex,
						text.Length));
			}

			// Go through and calculate the UTF-8 index from the input text.
			int unicodeCharacter = 0;

			for (int index = 0;
				index < stringIndex;
				index++)
			{
				// Get the character at this point
				int c = text[index];

				if (c < 128)
				{
					unicodeCharacter += 1;
				}
				else if (c < 2048)
				{
					unicodeCharacter += 2;
				}
				else if (c < 65536)
				{
					unicodeCharacter += 3;
				}
				else
				{
					unicodeCharacter += 4;
				}
			}

			return unicodeCharacter;
		}

		#endregion
	}
}
