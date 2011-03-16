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
using System.Collections.Generic;
using System.Text;

using MfGames.GtkExt.LineTextEditor.Buffers;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Renderers
{
	/// <summary>
	/// Utility class for taking Pango markup and applying a selection to it.
	/// </summary>
	public static class SelectionHelper
	{
		/// <summary>
		/// Takes the given string (which assumes a valid Pango markup) and
		/// adds the markup to apply a selection to the string.
		/// </summary>
		/// <param name="markup">The markup.</param>
		/// <param name="characters">The selection.</param>
		/// <returns></returns>
		public static string GetSelectionMarkup(
			string markup,
			CharacterRange characters)
		{
			// Check for nulls and invalid strings.
			if (String.IsNullOrEmpty(markup))
			{
				// We can't really do anything with this.
				return markup;
			}

			// The primary concern for applying the selection is that we already
			// have Pango markup in the string and we have to maintain that
			// markup while adjusting it. Pango doesn't allow for non-XML rules
			// such as nested spans, so we go through the string and keep track
			// of the spans as they apply. When we get to the selection, we
			// disable those spans and replace them with ones of our own.
			//
			// In addition, the character range does not apply to entities such
			// as &amp; so the entity is treated as a single string.

			// Keep track of an ordered list of tags we've processed.
			var tags = new LinkedList<string>();

			// Loop through the input markup
			var buffer = new StringBuilder();
			bool inSelection = false;
			int selectionIndex = 0;

			for (int markupIndex = 0;
			     markupIndex < markup.Length;
			     markupIndex++)
			{
				// Pull out the character we are processing.
				char c = markup[markupIndex];

				Console.WriteLine("c {3} s {0} m {1} cr {2}", selectionIndex, markupIndex, characters, c);

				// Check for tags since they are special.
				if (c == '<')
				{
					// We are in a Pango markup (since the normal < is escaped).
					// Pull out the tag into a string so we can parse it.
					var tagBuffer = new StringBuilder();

					while (c != '>')
					{
						tagBuffer.Append(c);
						markupIndex++;
						c = markup[markupIndex];
					}

					tagBuffer.Append(c);

					string tag = tagBuffer.ToString();

					// Figure out what to do with the tag.
					if (tag == "</span>")
					{
						// This is an end tag, so remove the last item.
						tags.RemoveLast();
					}
					else
					{
						// Append this to the both the list and the buffer.
						tags.AddLast(tag);
					}

					buffer.Append(tag);

					// Since tags are non-characters, we just continue the loop.
					continue;
				}

				// See if we are at the beginning of the selection.
				if (selectionIndex == characters.StartIndex)
				{
					// We are now in a selection buffer. First remove any
					// spans that we are currently using.
					for (int i = 0; i < tags.Count; i++)
					{
						buffer.Append("</span>");
					}

					// Add the text to format our selection.
					buffer.Append("<span background='#CCCCFF'>");

					// Apply the spans that should have existed here.
					foreach (string tag in tags)
					{
						buffer.Append(tag);
					}

					// Mark that we are in the selection.
					inSelection = true;
				}

				// Check to see if we are at the end of the selection.
				if (selectionIndex == characters.EndIndex)
				{
					// We are now in a selection buffer. First remove any
					// spans that we are currently using.
					for (int i = 0; i < tags.Count; i++)
					{
						buffer.Append("</span>");
					}

					// We are finishing up the selection, so remove any
					// selection-specific spans we are using.
					buffer.Append("</span>");

					// Apply the spans that should have existed here.
					foreach (string tag in tags)
					{
						buffer.Append(tag);
					}

					// Leave the selection so we don't close it twice.
					inSelection = false;
				}

				// Check for special characters.
				if (c == '&')
				{
					// This is an entity character, so skip through the entire
					// thing and just add it to the buffer. We don't add the
					// ';' because we'll be adding it at the end of the loop.
					while (c != ';')
					{
						buffer.Append(c);
						markupIndex++;
						c = markup[markupIndex];
					}
				}

				// Add to the selection index which is the index without formatting.
				selectionIndex++;

				// Add the character to the buffer.
				buffer.Append(c);
			}

			// If the end index is -1, then we finish up the spans at the end.
			if (inSelection)
			{
				// Remove any spans currently in effect for the selection.
				buffer.Append("</span>");
			}

			// Return the resulting markup.
			return buffer.ToString();
		}
	}
}