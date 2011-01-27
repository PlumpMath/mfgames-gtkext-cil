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

using System.Text;

#endregion

namespace MfGames.GtkExt
{
	/// <summary>
	/// Utility functions for dealing with Pango.
	/// </summary>
	public class PangoUtility
	{
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
	}
}