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

namespace MfGames.GtkExt.LineTextEditor.Buffers
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
		public BufferPosition AnchorPosition { get; set; }

		/// <summary>
		/// Gets the highest position between the anchor and tail.
		/// </summary>
		/// <value>The end position.</value>
		public BufferPosition EndPosition
		{
			get { return AnchorPosition > TailPosition ? AnchorPosition : TailPosition; }
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
		/// Gets the start position which is defined as the lessor of the anchor
		/// or tail.
		/// </summary>
		/// <value>The start position.</value>
		public BufferPosition StartPosition
		{
			get { return AnchorPosition < TailPosition ? AnchorPosition : TailPosition; }
		}

		/// <summary>
		/// Gets or sets the tail position (end) of the selection.
		/// </summary>
		/// <value>The tail position.</value>
		public BufferPosition TailPosition { get; set; }

		#endregion
	}
}