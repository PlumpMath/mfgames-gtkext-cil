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

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a word splitter that uses a standard "word" length of 5
	/// characters for the offset.
	/// </summary>
	public class OffsetWordSplitter : IWordSplitter
	{
        /// <summary>
        /// Gets the next word boundary from the given string and character index.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="characterIndex">Index of the character.</param>
        /// <returns></returns>
	    public int GetNextWordBoundary(string text,
	                                   int characterIndex)
	    {
            // If we are at the beginning, return -1.
            if (characterIndex == text.Length)
            {
                return -1;
            }

            // Move back five characters or to the beginning.
            return Math.Min(text.Length, characterIndex + 5);
        }

        /// <summary>
        /// Gets the previous word boundary from the given string and character index.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="characterIndex">Index of the character.</param>
        /// <returns></returns>
	    public int GetPreviousWordBoundary(string text,
	                                       int characterIndex)
	    {
            // If we are at the beginning, return -1.
            if (characterIndex == 0)
            {
                return -1;
            }

            // Move back five characters or to the beginning.
            return Math.Max(0, characterIndex - 5);
	    }
	}
}