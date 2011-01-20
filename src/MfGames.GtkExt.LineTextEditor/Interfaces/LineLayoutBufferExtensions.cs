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

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Interfaces
{
	/// <summary>
	/// Implement common extensions for ILineLayoutBuffers.
	/// </summary>
	public static class LineLayoutBufferExtensions
	{
		#region Wrapped Lines

		/// <summary>
		/// Gets the wrapped line layout for a given buffer Y coordinate.
		/// </summary>
		/// <param name="lineLayoutBuffer">The line layout buffer.</param>
		/// <param name="displayContext">The display context.</param>
		/// <param name="bufferY">The buffer Y.</param>
		/// <returns></returns>
		public static LayoutLine GetWrappedLineLayout(
			this ILineLayoutBuffer lineLayoutBuffer,
			IDisplayContext displayContext,
			double bufferY)
		{
			int wrappedLineIndex;
			return lineLayoutBuffer.GetWrappedLineLayout(
				displayContext, bufferY, out wrappedLineIndex);
		}

		/// <summary>
		/// Gets the wrapped line layout for a given buffer Y coordinate and the
		/// associated index.
		/// </summary>
		/// <param name="lineLayoutBuffer">The line layout buffer.</param>
		/// <param name="displayContext">The display context.</param>
		/// <param name="bufferY">The buffer Y.</param>
		/// <param name="wrappedLineIndex">Index of the wrapped line.</param>
		/// <returns></returns>
		public static LayoutLine GetWrappedLineLayout(
			this ILineLayoutBuffer lineLayoutBuffer,
			IDisplayContext displayContext,
			double bufferY,
			out int wrappedLineIndex)
		{
			// Get the line that contains the given Y coordinate.
			int lineIndex, endLineIndex;
			lineLayoutBuffer.GetLineLayoutRange(
				displayContext,
				new Rectangle(0, bufferY, 0, bufferY),
				out lineIndex,
				out endLineIndex);

			// Get the layout-relative Y coordinate.
			double layoutY = bufferY -
			                 lineLayoutBuffer.GetLineLayoutHeight(
			                 	displayContext, 0, lineIndex);

			// Figure out which line inside the layout.
			Layout layout = lineLayoutBuffer.GetLineLayout(displayContext, lineIndex);
			int trailing;

			layout.XyToIndex(0, (int) layoutY, out wrappedLineIndex, out trailing);

			// Return the layout line.
			return layout.Lines[wrappedLineIndex];
		}

		/// <summary>
		/// Gets the index of the wrapped line layout.
		/// </summary>
		/// <param name="lineLayoutBuffer">The line layout buffer.</param>
		/// <param name="displayContext">The display context.</param>
		/// <param name="bufferY">The buffer Y.</param>
		/// <returns></returns>
		public static int GetWrappedLineLayoutIndex(
			this ILineLayoutBuffer lineLayoutBuffer,
			IDisplayContext displayContext,
			double bufferY)
		{
			int wrappedLineIndex;
			lineLayoutBuffer.GetWrappedLineLayout(
				displayContext, bufferY, out wrappedLineIndex);
			return wrappedLineIndex;
		}

		#endregion
	}
}