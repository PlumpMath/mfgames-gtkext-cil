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

using C5;

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace GtkExtDemo.LineTextEditor
{
	/// <summary>
	/// Implements an <see cref="ILineIndicatorBuffer"/> that identifies errors and
	/// warnings.
	/// </summary>
	public class KeywordLineIndicatorBuffer
		: LineLayoutBufferProxy, ILineIndicatorBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="KeywordLineIndicatorBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public KeywordLineIndicatorBuffer(ILineLayoutBuffer buffer)
			: base(buffer)
		{
		}

		#endregion

		#region Line Indicators

		/// <summary>
		/// Gets the line indicators for a given character range.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="startCharacterIndex">Start character in the line text.</param>
		/// <param name="endCharacterIndex">End character in the line text.</param>
		/// <returns>
		/// An enumerable with the indicators or <see langword="null"/> for
		/// none.
		/// </returns>
		public IEnumerable<ILineIndicator> GetLineIndicators(
			IDisplayContext displayContext,
			int lineIndex,
			int startCharacterIndex,
			int endCharacterIndex)
		{
			// Get the escaped line markup.
			string text = this.GetLineText(lineIndex);
			string partialText = text.Substring(
				startCharacterIndex, endCharacterIndex - startCharacterIndex);

			// Parse through the markup and get a list of entries. If we don't
			// get any out of this, return null.
			ArrayList<KeywordMarkupEntry> entries = KeywordMarkupEntry.ParseText(partialText);

			if (entries.Count == 0)
			{
				return null;
			}

			// Return the list of keyword entries which are also indicators.
			var indicators = new ArrayList<ILineIndicator>(entries.Count);

			indicators.AddAll(entries);

			return indicators;
		}

		#endregion
	}
}