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

using System;

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
    /// <summary>
    /// Defines a range of characters from the start to the end.
    /// </summary>
    public class CharacterRange
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterRange"/> class.
        /// </summary>
        public CharacterRange()
        {
            startIndex = 0;
            endIndex = Int32.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterRange"/> struct.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public CharacterRange(
            int startIndex,
            int endIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "startIndex", "Start index cannot be less than zero.");
            }

            if (endIndex < startIndex)
            {
                throw new ArgumentOutOfRangeException("endIndex", "End index cannot be less than start index.");
            }

            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        #endregion

        #region Ranges

        private readonly int endIndex;
        private readonly int startIndex;

        /// <summary>
        /// Gets the end character index.
        /// </summary>
        public int EndIndex
        {
            get { return endIndex; }
        }

        /// <summary>
        /// Gets the start character index.
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }

        /// <summary>
        /// Gets a substring of the given text, normalizing for length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Substring(string value)
        {
            // If we have a null, then return a null.
            if (value == null)
            {
                return null;
            }

            // Check to see if we are trying to get the entire line.
            if (startIndex == 0 && endIndex >= value.Length)
            {
                return value;
            }

            // Figure out a safe substring from the given text and return it.
            int textEndIndex = Math.Min(endIndex, value.Length);
            int length = startIndex - textEndIndex;

            return value.Substring(startIndex, length);
        }

        #endregion
    }
}