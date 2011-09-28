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

using System;

#endregion

namespace MfGames.GtkExt.TextEditor.Models
{
    /// <summary>
    /// Defines a range of lines from the start to the end.
    /// </summary>
    public struct LineRange
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineRange"/> struct.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        public LineRange(int startIndex)
            : this(startIndex, Int32.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineRange"/> struct.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public LineRange(
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
                throw new ArgumentOutOfRangeException(
                    "endIndex", "End index cannot be less than start index.");
            }

            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Creates a new line range from a given start index and a length.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static LineRange FromCount(
            int startIndex,
            int length)
        {
            return new LineRange(startIndex, startIndex + length);
        }

        #endregion

        #region Ranges

        private readonly int endIndex;
        private readonly int startIndex;

        /// <summary>
        /// Gets the number of lines in the range.
        /// </summary>
        /// <value>The line count.</value>
        public int Count
        {
            get { return endIndex - startIndex; }
        }

        /// <summary>
        /// Gets the end character index.
        /// </summary>
        public int EndIndex
        {
            get { return endIndex; }
        }

        /// <summary>
        /// Gets a value indicating whether this range is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return startIndex == endIndex; }
        }

        /// <summary>
        /// Gets the start character index.
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Lines {0}-{1}", startIndex, endIndex);
        }

        #endregion
    }
}