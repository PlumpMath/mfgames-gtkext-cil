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

using System.Diagnostics;

using MfGames.GtkExt.LineTextEditor.Interfaces;

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a proxy class that implements all the ILineLayoutBuffer
	/// members while wrapping around another ILineLayoutBuffer.
	/// </summary>
	public class LineLayoutBufferProxy : LineMarkupBufferProxy, ILineLayoutBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LineLayoutBufferProxy"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public LineLayoutBufferProxy(ILineLayoutBuffer buffer)
			: base(buffer)
		{
		}

		#endregion

		#region Buffer

		/// <summary>
		/// Gets the line layout buffer.
		/// </summary>
		/// <value>The line layout buffer.</value>
		protected ILineLayoutBuffer LineLayoutBuffer
		{
			[DebuggerStepThrough]
			get
			{
				// This works because ILineLayoutBuffer extends ILineMarkupBuffer.
				return (ILineLayoutBuffer) LineMarkupBuffer;
			}
		}

		/// <summary>
		/// Indicates that the underlying text editor has changed in some manner
		/// and any cache or size calculations are invalidated.
		/// </summary>
		public virtual void Reset()
		{
			LineLayoutBuffer.Reset();
		}

		#endregion

		#region Line Layout

		/// <summary>
		/// Sets the pixel width of a layout in case the layout uses word
		/// wrapping.
		/// </summary>
		/// <value>The width.</value>
		public virtual int Width
		{
			get { return LineLayoutBuffer.Width; }
			set { LineLayoutBuffer.Width = value; }
		}

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public virtual Layout GetLineLayout(
			IDisplayContext displayContext,
			int lineIndex)
		{
			return LineLayoutBuffer.GetLineLayout(displayContext, lineIndex);
		}

		/// <summary>
		/// Gets the pixel height of the lines in the buffer. If endLine is -1
		/// it means the last line in the buffer.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="startLineIndex">The start line.</param>
		/// <param name="endLineIndex">The end line.</param>
		/// <returns></returns>
		public virtual int GetLineLayoutHeight(
			IDisplayContext displayContext,
			int startLineIndex,
			int endLineIndex)
		{
			return LineLayoutBuffer.GetLineLayoutHeight(
				displayContext, startLineIndex, endLineIndex);
		}

		/// <summary>
		/// Gets the height of a single line of "normal" text.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns></returns>
		public virtual int GetLineLayoutHeight(IDisplayContext displayContext)
		{
			return LineLayoutBuffer.GetLineLayoutHeight(displayContext);
		}

		/// <summary>
		/// Gets the lines that are visible in the given view area.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="viewArea">The view area.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		public virtual void GetLineLayoutRange(IDisplayContext displayContext,
		                                       Rectangle viewArea,
		                                       out int startLine,
		                                       out int endLine)
		{
			LineLayoutBuffer.GetLineLayoutRange(
				displayContext, viewArea, out startLine, out endLine);
		}

		#endregion
	}
}