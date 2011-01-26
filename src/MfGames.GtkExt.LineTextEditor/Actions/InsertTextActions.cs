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
	public static class InsertTextActions
	{
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
			string before = lineText.Substring(0, position.CharacterIndex);
			string after = lineText.Substring(position.CharacterIndex);

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
			caret.Position.CharacterIndex++;

			// Perform the operation on the buffer.
			actionContext.Do(operation);

			// Scroll to the caret to keep it on screen.
			displayContext.ScrollToCaret();
		}
	}
}