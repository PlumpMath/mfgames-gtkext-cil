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

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a word splitter that uses a standard "word" length of 5
	/// characters for the offset.
	/// </summary>
	public class OffsetWordSplitter : IWordSplitter
	{
		/// <summary>
		/// Finds the word boundaries in a given text.
		/// </summary>
		/// <param name="text">The text to parse.</param>
		/// <param name="character">The character index inside the text.</param>
		/// <param name="leftBoundary">The left boundary or -1 if there is no boundary to the left.</param>
		/// <param name="rightBoundary">The right boundary or -1 if there is no boundary to the right.</param>
		public void FindWordBoundaries(
			string text,
			int character,
			out int leftBoundary,
			out int rightBoundary)
		{
			// If we are at the left-most, we have no boundary or we simply move
			// the distance to the left.
			if (character == 0)
			{
				leftBoundary = -1;
			}
			else
			{
				leftBoundary = Math.Max(0, character - 5);
			}

			// If we are at the end of the string, then there is no boundary, otherwise
			// we just move five characters to the right.
			if (character == text.Length)
			{
				rightBoundary = -1;
			}
			else
			{
				rightBoundary = Math.Min(text.Length, character + 5);
			}
		}
	}
}