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

using Pango;

#endregion

namespace MfGames.GtkExt.Extensions.Pango
{
	/// <summary>
	/// Defines extensions to the Pango.LayoutLine class.
	/// </summary>
	public static class PangoLayoutLineExtensions
	{
		/// <summary>
		/// Gets the index of the layout line inside the layout.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <returns></returns>
		public static int GetLayoutLineIndex(this LayoutLine layoutLine)
		{
			// Go through all the lines in the layout and find the index.
			for (int index = 0; index < layoutLine.Layout.LineCount; index++)
			{
				if (layoutLine.Layout.Lines[index].StartIndex == layoutLine.StartIndex)
				{
					return index;
				}
			}

			// We can't find this layout line inside the layout.
			throw new Exception("Cannot find layout line inside layout");
		}

		/// <summary>
		/// Gets the size of the wrapped line in pixels and relative to the layout.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <param name="size">The size.</param>
		public static void GetPixelExtents(
			this LayoutLine layoutLine,
			out Rectangle size)
		{
			// Go through and populate the size regions.
			size = new Rectangle();
			size.X = 0;
			size.Width = layoutLine.Layout.Width;

			// Build up the Y coordinates of all the lines before it.
			int layoutLineIndex = layoutLine.GetLayoutLineIndex();
			int y = 0;

			for (int index = 0; index < layoutLineIndex; index++)
			{
				LayoutLine preceedingLayoutLine = layoutLine.Layout.Lines[index];
				var ink = new Rectangle();
				var logical = new Rectangle();

				preceedingLayoutLine.GetPixelExtents(ref ink, ref logical);

				y += logical.Height;
			}

			size.Y = y;

			// Figure out the height of the given wrapped line.
			var ink2 = new Rectangle();
			var logical2 = new Rectangle();

			layoutLine.GetPixelExtents(ref ink2, ref logical2);

			size.Height = logical2.Height;
		}

		/// <summary>
		/// Determines whether this layout line is the last line in the layout.
		/// </summary>
		/// <param name="layoutLine">The layout line.</param>
		/// <returns>
		/// 	<c>true</c> if [is last line] [the specified layout line]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsLastLineInLayout(this LayoutLine layoutLine)
		{
			Layout layout = layoutLine.Layout;
			return layout.Lines[layout.LineCount - 1].StartIndex == layoutLine.StartIndex;
		}
	}
}