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
    /// Represents a range inside a buffer.
    /// </summary>
    public struct BufferSegment
    {
        #region Properties

        /// <summary>
        /// Gets or sets the anchor (or beginning) of the selection.
        /// </summary>
        /// <value>The anchor position.</value>
        public BufferPosition AnchorPosition;

        /// <summary>
        /// Gets or sets the tail position (end) of the selection.
        /// </summary>
        /// <value>The tail position.</value>
        public BufferPosition TailPosition;

        /// <summary>
        /// Gets the highest position between the anchor and tail.
        /// </summary>
        /// <value>The end position.</value>
        public BufferPosition EndPosition
        {
            get
            {
                return AnchorPosition > TailPosition
                           ? AnchorPosition
                           : TailPosition;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this segment is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return AnchorPosition == TailPosition; }
        }

        /// <summary>
        /// Gets a value indicating whether this segment is entirely on one line.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is same line; otherwise, <c>false</c>.
        /// </value>
        public bool IsSameLine
        {
            get { return AnchorPosition.LineIndex == TailPosition.LineIndex; }
        }

        /// <summary>
        /// Gets the start position which is defined as the lessor of the anchor
        /// or tail.
        /// </summary>
        /// <value>The start position.</value>
        public BufferPosition StartPosition
        {
            get
            {
                return AnchorPosition < TailPosition
                           ? AnchorPosition
                           : TailPosition;
            }
        }

        #endregion

        #region Ranges

        /// <summary>
        /// Determines whether the given line index is inside the segment.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <returns>
        /// 	<c>true</c> if the segment contains the list, otherwise <c>false</c>.
        /// </returns>
        public bool ContainsLine(int lineIndex)
        {
            int startCharacterIndex, endCharacterIndex;

            return ContainsLine(
                lineIndex, out startCharacterIndex, out endCharacterIndex);
        }

        /// <summary>
        /// Determines whether the given line index is inside the segment.
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="startCharacterIndex">Start index of the character.</param>
        /// <param name="endCharacterIndex">End index of the character.</param>
        /// <returns>
        /// 	<c>true</c> if the segment contains the list, otherwise <c>false</c>.
        /// </returns>
        public bool ContainsLine(
            int lineIndex,
            out int startCharacterIndex,
            out int endCharacterIndex)
        {
            // Pull out the positions.
            BufferPosition startPosition = StartPosition;
            BufferPosition endPosition = EndPosition;

            // If the positions are equal, then no.
            if (startPosition == endPosition)
            {
                startCharacterIndex = 0;
                endCharacterIndex = 0;
                return false;
            }

            // If the line is less than the start or greater than the end, it
            // isn't in the range.
            if (lineIndex < startPosition.LineIndex ||
                lineIndex > endPosition.LineIndex)
            {
                startCharacterIndex = 0;
                endCharacterIndex = 0;
                return false;
            }

            // If the line is in the middle of the segment, then the entire
            // line is included.
            if (lineIndex > startPosition.LineIndex &&
                lineIndex < endPosition.LineIndex)
            {
                startCharacterIndex = 0;
                endCharacterIndex = Int32.MaxValue;
                return true;
            }

            // See if the start and end are on the same line.
            if (startPosition.LineIndex == endPosition.LineIndex)
            {
                startCharacterIndex = startPosition.CharacterIndex;
                endCharacterIndex = endPosition.CharacterIndex;
                return true;
            }

            // Check the start of the line.
            if (lineIndex == startPosition.LineIndex)
            {
                startCharacterIndex = startPosition.CharacterIndex;
                endCharacterIndex = Int32.MaxValue;
                return true;
            }

            if (lineIndex == endPosition.LineIndex)
            {
                startCharacterIndex = 0;
                endCharacterIndex = endPosition.CharacterIndex;
                return true;
            }

            // If we got this far, then we have a logic flaw.
            throw new Exception("Cannot determine ContainsLine: " + lineIndex);
        }

        /// <summary>
        /// Determines whether the specified segment contains position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        /// 	<c>true</c> if the specified segment contains position; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPosition(BufferPosition position)
        {
            // Determine if the position is in the segment.
            int startCharacterIndex, endCharacterIndex;

            bool results = ContainsLine(
                position.LineIndex,
                out startCharacterIndex,
                out endCharacterIndex);

            if (!results)
            {
                // The line is not in the segment.
                return false;
            }

            // Normalize the end coordinate, if we got a negative.
            if (endCharacterIndex < 0)
            {
                endCharacterIndex = Int32.MaxValue;
            }

            // Check for the range.
            return position.CharacterIndex >= startCharacterIndex &&
                   position.CharacterIndex <= endCharacterIndex;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Compares this segment to another one.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(BufferSegment other)
        {
            return other.AnchorPosition.Equals(AnchorPosition) &&
                   other.TailPosition.Equals(TailPosition);
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

            if (obj.GetType() != typeof(BufferSegment))
            {
                return false;
            }

            return Equals((BufferSegment) obj);
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
                return (AnchorPosition.GetHashCode() * 397) ^
                       TailPosition.GetHashCode();
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BufferSegment a,
                                       BufferSegment b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BufferSegment a,
                                       BufferSegment b)
        {
            return !a.Equals(b);
        }

        #endregion
    }
}