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

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
    /// <summary>
    /// Contains spacing values used for margins and padding. Each of them
    /// can also be null to indicate no value.
    /// </summary>
    public class OptionalSpacing
    {
        #region Directions

        /// <summary>
        /// Gets or sets the bottom spacing.
        /// </summary>
        /// <value>The bottom.</value>
        public double? Bottom { get; set; }

        /// <summary>
        /// Gets a value indicating whether all four directions have values.
        /// </summary>
        /// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
        public bool Complete
        {
            get
            {
                return Right.HasValue && Left.HasValue && Top.HasValue &&
                       Bottom.HasValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether all four directions have no values.
        /// </summary>
        /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
        public bool Empty
        {
            get
            {
                return !Right.HasValue && !Left.HasValue && !Top.HasValue &&
                       !Bottom.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the left spacing.
        /// </summary>
        /// <value>The left.</value>
        public double? Left { get; set; }

        /// <summary>
        /// Gets or sets the right spacing.
        /// </summary>
        /// <value>The right.</value>
        public double? Right { get; set; }

        /// <summary>
        /// Gets or sets the top spacing.
        /// </summary>
        /// <value>The top.</value>
        public double? Top { get; set; }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts the optional spacing to a spacing object.
        /// </summary>
        /// <returns></returns>
        public Spacing ToSpacing()
        {
            return new Spacing(
                Top.HasValue ? Top.Value : 0,
                Right.HasValue ? Right.Value : 0,
                Bottom.HasValue ? Bottom.Value : 0,
                Left.HasValue ? Left.Value : 0);
        }

        #endregion
    }
}