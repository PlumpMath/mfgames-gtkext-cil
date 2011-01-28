using System.Text;

using MfGames.GtkExt.LineTextEditor.Interfaces;

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Defines a line markup buffer that displays the selection with a
	/// background, otherwise shows the text normal.
	/// </summary>
	public class SimpleSelectionLineMarkupBuffer : LineMarkupBufferProxy
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleSelectionLineMarkupBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public SimpleSelectionLineMarkupBuffer(ILineMarkupBuffer buffer)
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
		public override string GetLineMarkup(IDisplayContext displayContext, int lineIndex)
		{
			// Get the line markup from the underlying buffer.
			string markup = base.GetLineMarkup(displayContext, lineIndex);

			// Check to see if we are in the selection.
			int startCharacterIndex, endCharacterIndex;
			bool containsLine = displayContext.Caret.Selection.ContainsLine(lineIndex, out startCharacterIndex, out endCharacterIndex);

			if (containsLine)
			{
				// Loop through the markup and strip off the existing markup
				// for the selection area.
				StringBuilder buffer = new StringBuilder();
				bool inSelection = false;

				for (int characterIndex = 0; characterIndex < markup.Length; characterIndex++)
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