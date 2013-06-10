// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Cairo;

namespace MfGames.GtkExt.Extensions.Cairo
{
	/// <summary>
	/// Defines extensions for Cairo.Rectangle.
	/// </summary>
	public static class CairoRectangleExtensions
	{
		#region Methods

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
			if (outerRectangle.X > innerRectangle.X
				|| outerRectangle.Y > innerRectangle.Y)
			{
				return false;
			}

			// Make sure the right and bottom sides are within the outer.
			double innerRight = innerRectangle.X + innerRectangle.Width;
			double outerRight = outerRectangle.X + outerRectangle.Width;
			double innerBottom = innerRectangle.Y + innerRectangle.Height;
			double outerBottom = outerRectangle.Y + outerRectangle.Height;

			if (innerRight > outerRight
				|| innerBottom > outerBottom)
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

			if (right1 < left2
				|| right2 < left1
				|| left1 > right2
				|| left2 > right1)
			{
				// There is no overlap horizontally.
				return false;
			}

			// Check for vertical overlap.
			double top1 = rectangle1.X;
			double top2 = rectangle2.X;
			double bottom1 = rectangle1.X + rectangle1.Width;
			double bottom2 = rectangle2.X + rectangle2.Width;

			if (bottom1 < top2
				|| bottom2 < top1
				|| top1 > bottom2
				|| top2 > bottom1)
			{
				// There is no overlap vertically.
				return false;
			}

			// There is an overlap.
			return true;
		}

		#endregion
	}
}
