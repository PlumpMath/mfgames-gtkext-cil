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

#endregion

#region Namespaces

using System;

#endregion

namespace MfGames.GtkExt.TextEditor.Models
{
    /// <summary>
    /// Represents a position within the text buffer using the line as a primary
    /// and the character within the line's text.
    /// </summary>
    public struct BufferPosition
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPosition"/> struct.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="characterIndex">Index of the character.</param>
        public BufferPosition(
            int lineIndex,
            int characterIndex)
        {
            if (lineIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "lineIndex", "Line index cannot be negative.");
            }

            if (characterIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "characterIndex", "Character index cannot be negative.");
            }

            this.lineIndex = lineIndex;
            this.characterIndex = characterIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPosition"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public BufferPosition(BufferPosition position)
        {
            lineIndex = position.LineIndex;
            characterIndex = position.CharacterIndex;
        }

        #endregion

        #region Properties

        private int characterIndex;

        private int lineIndex;

        /// <summary>
        /// Gets or sets the character. In terms of caret positions, the position
        /// is always to the left of the character, not trailing it.
        /// </summary>
        /// <value>The character.</value>
        public int CharacterIndex
        {
            get { return characterIndex; }
            set { characterIndex = value; }
        }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>The line.</value>
        public int LineIndex
        {
            get { return lineIndex; }
            set { lineIndex = value; }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if two positions are equal.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(BufferPosition other)
        {
            return other.characterIndex == characterIndex &&
                   other.lineIndex == lineIndex;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (obj.GetType() != typeof(BufferPosition))
            {
                return false;
            }
            return Equals((BufferPosition) obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (characterIndex * 397) ^ lineIndex;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BufferPosition a,
                                       BufferPosition b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(BufferPosition a,
                                      BufferPosition b)
        {
            return !(a < b);
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(BufferPosition a,
                                       BufferPosition b)
        {
            return a > b || a == b;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BufferPosition a,
                                       BufferPosition b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(BufferPosition a,
                                      BufferPosition b)
        {
            if (a.lineIndex < b.lineIndex)
            {
                return true;
            }

            if (a.lineIndex > b.lineIndex)
            {
                return false;
            }

            if (a.characterIndex < b.CharacterIndex)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(BufferPosition a,
                                       BufferPosition b)
        {
            return a < b || a == b;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Buffer Position ({0}, {1})", lineIndex, characterIndex);
        }

        #endregion
    }
}