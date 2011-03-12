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
using System.Text;

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Renderers
{
	/// <summary>
	/// Defines a line markup buffer that displays the selection with a
	/// background, otherwise shows the text normal.
	/// </summary>
	public class CaretSelectionLineBuffer : LineBufferDecorator
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CaretSelectionLineBuffer"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="buffer">The buffer.</param>
		public CaretSelectionLineBuffer(
			IDisplayContext displayContext,
			LineBuffer buffer)
			: base(buffer)
		{
			if (displayContext == null)
			{
				throw new ArgumentNullException("displayContext");
			}

			this.displayContext = displayContext;
		}

		#endregion

		#region Display

		private readonly IDisplayContext displayContext;

		#endregion

		#region Markup

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public override string GetLineMarkup(int lineIndex)
		{
			// Get the line markup from the underlying buffer.
			string markup = base.GetLineMarkup(lineIndex);

			// Check to see if we are in the selection.
			int startCharacterIndex, endCharacterIndex;
			bool containsLine = displayContext.Caret.Selection.ContainsLine(
				lineIndex, out startCharacterIndex, out endCharacterIndex);

			if (containsLine)
			{
				// Loop through the markup and strip off the existing markup
				// for the selection area.
				var buffer = new StringBuilder();
				bool inSelection = false;

				for (int characterIndex = 0;
				     characterIndex < markup.Length;
				     characterIndex++)
				{
					// Pull out the character we are processing.
					char c = markup[characterIndex];

					// See if we are at the beginning of the selection.
					if (characterIndex == startCharacterIndex)
					{
						// We are now in a selection buffer. First remove any
						// spans that we are currently using.

						// Add the text to format our selection.
						buffer.Append("<span background='#CCCCFF'>");

						// Mark that we are in the selection.
						inSelection = true;
					}

					// Check to see if we are at the end of the selection.
					if (characterIndex == endCharacterIndex)
					{
						// We are finishing up the selection, so remove any
						// selection-specific spans we are using.
						buffer.Append("</span>");

						// Apply the spans that should have existed here.

						// Leave the selection so we don't close it twice.
						inSelection = false;
					}

					// Add the character to the buffer.
					buffer.Append(c);
				}

				// If the end index is -1, then we finish up the spans at the end.
				if (inSelection)
				{
					// Remove any spans currently in effect for the selection.
					buffer.Append("</span>");
				}

				// Replace the markup with the modified one.
				markup = buffer.ToString();
			}

			// Return the resulting markup.
			return markup;
		}

		#endregion
	}
}