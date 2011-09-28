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

using Cairo;

#endregion

namespace MfGames.GtkExt.Extensions.Cairo
{
    /// <summary>
    /// Defines extensions for Cairo.Rectangle.
    /// </summary>
    public static class CairoRectangleExtensions
    {
        /// <summary>
        /// Determines whether the inner rectangle is completely inside the outer one.
        /// </summary>
        /// <param name="outerRectangle">The rectangle.</param>
        /// <param name="innerRectangle">The inner rectangle.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified rectangle]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(
            this Rectangle outerRectangle,
            Rectangle innerRectangle)
        {
            // If the inner rectangle starts outside of the outer, then return false.
            if (outerRectangle.X > innerRectangle.X ||
                outerRectangle.Y > innerRectangle.Y)
            {
                return false;
            }

            // Make sure the right and bottom sides are within the outer.
            double innerRight = innerRectangle.X + innerRectangle.Width;
            double outerRight = outerRectangle.X + outerRectangle.Width;
            double innerBottom = innerRectangle.Y + innerRectangle.Height;
            double outerBottom = outerRectangle.Y + outerRectangle.Height;

            if (innerRight > outerRight || innerBottom > outerBottom)
            {
                return false;
            }

            // At this point, the inner rectangle is completely inside the outer.
            return true;
        }

        /// <summary>
        /// Determines if the two rectangles intersect with each other.
        /// </summary>
        /// <param name="rectangle1">The rectangle.</param>
        /// <param name="rectangle2">The rectangle2.</param>
        /// <returns></returns>
        public static bool IntersectsWith(
            this Rectangle rectangle1,
            Rectangle rectangle2)
        {
            // Check horizontal overlap.
            double left1 = rectangle1.X;
            double left2 = rectangle2.X;
            double right1 = rectangle1.X + rectangle1.Width;
            double right2 = rectangle2.X + rectangle2.Width;

            if (right1 < left2 || right2 < left1 || left1 > right2 ||
                left2 > right1)
            {
                // There is no overlap horizontally.
                return false;
            }

            // Check for vertical overlap.
            double top1 = rectangle1.X;
            double top2 = rectangle2.X;
            double bottom1 = rectangle1.X + rectangle1.Width;
            double bottom2 = rectangle2.X + rectangle2.Width;

            if (bottom1 < top2 || bottom2 < top1 || top1 > bottom2 ||
                top2 > bottom1)
            {
                // There is no overlap vertically.
                return false;
            }

            // There is an overlap.
            return true;
        }
    }
}