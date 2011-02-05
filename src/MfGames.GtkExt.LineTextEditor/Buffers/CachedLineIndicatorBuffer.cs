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

using System.Collections.Generic;
using System.Diagnostics;

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Extends the <see cref="CachedLineLayoutBuffer"/> to also cache indicator
	/// information from a given buffer.
	/// </summary>
	public class CachedLineIndicatorBuffer
		: CachedLineLayoutBuffer, ILineIndicatorBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedLineIndicatorBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public CachedLineIndicatorBuffer(ILineIndicatorBuffer buffer)
			: this(buffer, 8, 128)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedLineIndicatorBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="maximumLoadedWindows">The maximum loaded windows.</param>
		/// <param name="windowSize">Size of the window.</param>
		public CachedLineIndicatorBuffer(
			ILineIndicatorBuffer buffer,
			int maximumLoadedWindows,
			int windowSize)
			: base(buffer, maximumLoadedWindows, windowSize)
		{
		}

		#endregion

		#region Line Indicators

		/// <summary>
		/// Gets the line indicator buffer.
		/// </summary>
		/// <value>The line indicator buffer.</value>
		protected ILineIndicatorBuffer LineIndicatorBuffer
		{
			[DebuggerStepThrough]
			get { return (ILineIndicatorBuffer) LineBuffer; }
		}

		/// <summary>
		/// Gets the line indicators for a given character range.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="startCharacterIndex">Start character in the line text.</param>
		/// <param name="endCharacterIndex">End character in the line text.</param>
		/// <returns></returns>
		public virtual IEnumerable<ILineIndicator> GetLineIndicators(
			IDisplayContext displayContext,
			int lineIndex,
			int startCharacterIndex,
			int endCharacterIndex)
		{
			return LineIndicatorBuffer.GetLineIndicators(
				displayContext, lineIndex, startCharacterIndex, endCharacterIndex);
		}

		#endregion
	}
}