#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using System.Collections.Generic;

using Pango;

#endregion

namespace MfGames.GtkExt
{
    /// <summary>
    /// Implements a basic font change for Pango fonts.
    /// </summary>
    public static class FontDescriptionCache
    {
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="FontDescriptionCache"/> class.
        /// </summary>
        static FontDescriptionCache()
        {
            fonts = new Dictionary<string, FontDescription>();
        }

        #endregion

        #region Caching

        private static readonly Dictionary<string, FontDescription> fonts;

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
    }
}