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

using C5;

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

			// Parse through the markup and get a list of entries. We go through
			// the list in reverse so we can use the character entries without
			// adjusting for the text we're adding.
			ArrayList<KeywordMarkupEntry> entries = KeywordMarkupEntry.ParseText(markup);

			entries.Reverse();

			foreach (var entry in entries)
			{
				// Insert the final span at the end.
				markup = markup.Insert(entry.EndCharacterIndex, "</span>");

				// Figure out the attributes.
				string attributes = string.Empty;

				switch (entry.Markup)
				{
					case KeywordMarkupType.Error:
						attributes = "underline='error' underline_color='red' color='red'";
						break;

					case KeywordMarkupType.Warning:
						attributes = "underline='error' underline_color='orange' color='orange'";
						break;
				}

				// Add in the attributes for the start index.
				markup = markup.Insert(
					entry.StartCharacterIndex, "<span " + attributes + ">");
			}

			// Return the resulting markup.
			return markup;
		}

		#endregion
	}
}