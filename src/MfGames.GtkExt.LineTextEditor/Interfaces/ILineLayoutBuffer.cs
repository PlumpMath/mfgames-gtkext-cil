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
	/// Implements the functionality for handling Pango layout rendering for
	/// the text editor.
	/// </summary>
	public interface ILineLayoutBuffer : ILineMarkupBuffer
	{
		#region Layout

		/// <summary>
		/// Sets the pixel width of a layout in case the layout uses word
		/// wrapping.
		/// </summary>
		/// <value>The width.</value>
		int Width { get; set; }

		/// <summary>
		/// Gets the index of the line from a given wrapped line index. This also
		/// returns the relative line index inside the layout.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="wrappedLineIndex">Index of the wrapped line.</param>
		/// <param name="layoutLineIndex">Index of the layout line.</param>
		/// <returns></returns>
		int GetLineIndex(
			IDisplayContext displayContext,
			int wrappedLineIndex,
			out int layoutLineIndex);

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">The line index in the buffer or -1 for the last line.</param>
		/// <returns></returns>
		Layout GetLineLayout(
			IDisplayContext displayContext,
			int lineIndex);

		/// <summary>
		/// Gets the pixel height of the lines in the buffer. If endLine is -1
		/// it means the last line in the buffer.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="startLineIndex">The start line.</param>
		/// <param name="endLineIndex">The end line index in the buffer or -1 for the last line.</param>
		/// <returns></returns>
		int GetLineLayoutHeight(
			IDisplayContext displayContext,
			int startLineIndex,
			int endLineIndex);

		/// <summary>
		/// Gets the height of a single line of "normal" text.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns></returns>
		int GetLineLayoutHeight(IDisplayContext displayContext);

		/// <summary>
		/// Gets the lines that are visible in the given view area.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="viewArea">The view area.</param>
		/// <param name="startLineIndex">Start index of the line.</param>
		/// <param name="endLineIndex">End index of the line.</param>
		void GetLineLayoutRange(
			IDisplayContext displayContext,
			Rectangle viewArea,
			out int startLineIndex,
			out int endLineIndex);

		/// <summary>
		/// Gets the wrapped line count for all the lines in the buffer.
		/// </summary>
		/// <value>The wrapped line count.</value>
		int GetWrappedLineCount(IDisplayContext displayContext);

		/// <summary>
		/// Gets the wrapped line indexes for a given line index.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">The line index.</param>
		/// <param name="startWrappedLineIndex">Start index of the wrapped line.</param>
		/// <param name="endWrappedLineIndex">End index of the wrapped line.</param>
		void GetWrappedLineIndexes(
			IDisplayContext displayContext,
			int lineIndex,
			out int startWrappedLineIndex,
			out int endWrappedLineIndex);

		/// <summary>
		/// Indicates that the underlying text editor has changed in some manner
		/// and any cache or size calculations are invalidated.
		/// </summary>
		void Reset();

		#endregion
	}
}