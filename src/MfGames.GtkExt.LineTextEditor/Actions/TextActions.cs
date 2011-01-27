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

using Gdk;

using MfGames.GtkExt.LineTextEditor.Attributes;
using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Commands;
using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
	/// <summary>
	/// Contains the various actions used for moving the caret (cursor) around
	/// the text buffer.
	/// </summary>
	[ActionFixture]
	public static class TextActions
	{
        /// <summary>
        /// Deletes the character to the left.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        [Action]
        [KeyBinding(Key.BackSpace)]
        public static void BackSpace(IActionContext actionContext)
        {
            // Get the position in the buffer.
            IDisplayContext displayContext = actionContext.DisplayContext;
            BufferPosition position = displayContext.Caret.Position;

            if (position.IsBeginningOfBuffer(actionContext.DisplayContext))
            {
                // We are in the beginning of the buffer, so we don't do anything.
                return;
            }

            // If we are at the beginning of the line, then we are combining paragraphs.
            ILineLayoutBuffer lineLayoutBuffer = displayContext.LineLayoutBuffer;
            string lineText = lineLayoutBuffer.GetLineText(position.LineIndex);

            if (position.CharacterIndex == 0)
            {
                // This is the beginning of a paragraph and not the first one in
                // the buffer. This operation combines the text of the two paragraphs
                // together.
                string previousText =
                    lineLayoutBuffer.GetLineText(position.LineIndex - 1);
                string newText = previousText + lineText;

                // Set up the operations.
                var delete = new DeleteLinesOperation(position.LineIndex, 1);
                var join = new SetTextOperation(position.LineIndex - 1, newText);

                // Relocate the caret position to the previous line's end.
                position.LineIndex--;
                position.CharacterIndex = previousText.Length;

                // Perform the actions.
                actionContext.Do(delete);
                actionContext.Do(join);
            }
            else
            {
                // This is a single-line manipulation, so delete the character.
                lineText = lineText.Substring(0, position.CharacterIndex - 1) +
                           lineText.Substring(position.CharacterIndex);

                // Create the set text operation.
                var operation = new SetTextOperation(
                    position.LineIndex, lineText);

                // Shift the caret back.
                position.CharacterIndex--;

                // Perform the action.
                actionContext.Do(operation);
            }

            // Scroll to the caret to keep it on screen.
            displayContext.Caret.Position = position;
            displayContext.ScrollToCaret();
        }

        /// <summary>
        /// Deletes the character to the right.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        [Action]
        [KeyBinding(Key.Delete)]
        public static void Delete(IActionContext actionContext)
        {
            // Get the position in the buffer.
            IDisplayContext displayContext = actionContext.DisplayContext;
            BufferPosition position = displayContext.Caret.Position;

            if (position.IsEndOfBuffer(actionContext.DisplayContext))
            {
                // We are in the end of the buffer, so we don't do anything.
                return;
            }

            // If we are at the beginning of the line, then we are combining paragraphs.
            ILineLayoutBuffer lineLayoutBuffer = displayContext.LineLayoutBuffer;
            string lineText = lineLayoutBuffer.GetLineText(position.LineIndex);

            if (position.CharacterIndex == lineText.Length)
            {
                // This is the end of a paragraph and not the first one in
                // the buffer. This operation combines the text of the two paragraphs
                // together.
                string nextText =
                    lineLayoutBuffer.GetLineText(position.LineIndex + 1);
                string newText = lineText + nextText;

                // Set up the operations.
                var delete = new DeleteLinesOperation(position.LineIndex + 1, 1);
                var join = new SetTextOperation(position.LineIndex, newText);

                // Perform the actions.
                actionContext.Do(delete);
                actionContext.Do(join);
            }
            else
            {
                // This is a single-line manipulation, so delete the character.
                lineText = lineText.Substring(0, position.CharacterIndex) +
                           lineText.Substring(position.CharacterIndex + 1);

                // Create the set text operation.
                var operation = new SetTextOperation(
                    position.LineIndex, lineText);

                // Perform the action.
                actionContext.Do(operation);
            }

            // We don't need to scroll since the caret isn't moving.
        }

        /// <summary>
		/// Inserts the paragraph at the current buffer position.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.Return)]
		public static void InsertParagraph(IActionContext actionContext)
		{
			// Get the text of the current line.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			string lineText =
				displayContext.LineLayoutBuffer.GetLineText(position.LineIndex);

			// Split the line based on the character index.
			string before = lineText.Substring(0, position.CharacterIndex).Trim();
			string after = lineText.Substring(position.CharacterIndex).Trim();

			// Create an operation to insert a line after this point.
			var insertOperation = new InsertLinesOperation(position.LineIndex, 1);

			// Set the text for both lines.
			var setTextOperation1 = new SetTextOperation(position.LineIndex, before);
			var setTextOperation2 = new SetTextOperation(position.LineIndex + 1, after);

			// Change the buffer position.
			position.LineIndex++;
			position.CharacterIndex = 0;

			// Perform the operations.
			actionContext.Do(insertOperation);
			actionContext.Do(setTextOperation1);
			actionContext.Do(setTextOperation2);

			// Scroll to the caret to keep it on screen.
	        displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
		}

		/// <summary>
		/// Moves the caret down one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		/// <param name="unicode">The unicode.</param>
		public static void InsertText(
			IActionContext actionContext,
			char unicode)
		{
			// Get the text of the current line.
			IDisplayContext displayContext = actionContext.DisplayContext;
			Caret caret = actionContext.DisplayContext.Caret;
			BufferPosition position = caret.Position;
			string lineText =
				displayContext.LineLayoutBuffer.GetLineText(caret.Position.LineIndex);

			// Make the changes in the line.
			lineText = lineText.Insert(position.CharacterIndex, unicode.ToString());

			// Create a set text operation.
			var operation = new SetTextOperation(position.LineIndex, lineText);

			// Shift the cursor over since we know this won't be changing lines
			// and we can avoid some additional refreshes.
		    position.CharacterIndex++;

			// Perform the operation on the buffer.
			actionContext.Do(operation);

			// Scroll to the caret to keep it on screen.
		    displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
            displayContext.RequestRedraw();
		}
	}
}