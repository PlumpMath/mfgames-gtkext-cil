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

using System.Text;

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace GtkExtDemo.LineTextEditor
{
	/// <summary>
	/// Wraps around a line buffer and marks up anything with a number of keywords
	/// with Pango markup.
	/// </summary>
	public class KeywordLineMarkupBuffer : UnformattedLineMarkupBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="KeywordLineMarkupBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public KeywordLineMarkupBuffer(ILineBuffer buffer)
			: base(buffer)
		{
		}

		#endregion

		#region Markup

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public override string GetLineMarkup(
			IDisplayContext displayContext,
			int lineIndex)
		{
			// Get the escaped line markup.
			string markup = base.GetLineMarkup(displayContext, lineIndex);

			// Markup the word "error" with a wavy underline and put the text
			// in red.
			markup = Replace(displayContext.WordSplitter,
				markup, "error", "underline='error' underline_color='red' color='red'");

			// Mark up the word "warning" with orange.
			markup = Replace(displayContext.WordSplitter,
				markup,
				"warning",
				"underline='error' underline_color='orange' color='orange'");

			// Return the resulting markup.
			return markup;
		}

		/// <summary>
		/// Searches for a specific text and wraps it in a Pango markup after
		/// expanding to include the entire word. This does a case-insensitive
		/// search.
		/// </summary>
		/// <param name="wordSplitter">The word splitter.</param>
		/// <param name="markup">The markup.</param>
		/// <param name="search">The search text.</param>
		/// <param name="markupAttributes">The markup attributes.</param>
		/// <returns></returns>
		private static string Replace(
			IWordSplitter wordSplitter,
			string markup,
			string search,
			string markupAttributes)
		{
			// Do a case-insensitive search by making everything uppercase.
			string upperMarkup = markup.ToUpper();
			string upperSearch = search.ToUpper();

			if (!upperMarkup.Contains(upperSearch))
			{
				// It isn't in the string, so we don't have to do anything.
				return markup;
			}

			// We have the search term at least once in the string. Start by
			// figuring out the first occurance, shifted back to adjust for
			// word boundaries.
			int searchIndex = upperMarkup.IndexOf(upperSearch);
			int index = wordSplitter.GetPreviousWordBoundary(markup, searchIndex + 1);

			// Add everything before this index point to the buffer.
			StringBuilder buffer = new StringBuilder();

			buffer.Append(markup.Substring(0, index));

			// Loop until we stop finding the search terms.
			do
			{
				// Get the word boundary to the right of the start point.
				int endOfWord = wordSplitter.GetNextWordBoundary(markup, index);
				string foundText = markup.Substring(index, endOfWord - index);

				// Add the found text, with Pango markup, to the buffer.
				buffer.Append("<span ");
				buffer.Append(markupAttributes);
				buffer.Append(">");
				buffer.Append(foundText);
				buffer.Append("</span>");

				// Shift the index by the length of the term.
				index += foundText.Length;

				// Figure out where the next keyword is.
				int nextIndex = upperMarkup.IndexOf(upperSearch, index);

				if (nextIndex < 0)
				{
					// We are at the end of the string.
					buffer.Append(markup.Substring(index));
					break;
				}

				nextIndex = wordSplitter.GetPreviousWordBoundary(markup, nextIndex + 1);
				buffer.Append(markup.Substring(index, nextIndex - index));
				index = nextIndex;
			}
			while (true);

			// Return the resulting markup.
			return buffer.ToString();
		}

		#endregion
	}
}