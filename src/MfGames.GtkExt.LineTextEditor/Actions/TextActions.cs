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

			// Create the command and add the operations.
			var command = new Command(position);

			command.Operations.Add(new InsertLinesOperation(position.LineIndex, 1));
			command.Operations.Add(new SetTextOperation(position.LineIndex, before));
			command.Operations.Add(new SetTextOperation(position.LineIndex + 1, after));

			command.UndoOperations.Add(new DeleteLinesOperation(position.LineIndex, 1));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Change the buffer position.
			position.LineIndex++;
			position.CharacterIndex = 0;

			// Perform the operations in the command.
			command.EndPosition = position;
			actionContext.Do(command);
		}

		/// <summary>
		/// Moves the caret down one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		/// <param name="unicode">The Unicode character.</param>
		public static void InsertText(
			IActionContext actionContext,
			char unicode)
		{
			// Because InsertText isn't a proper "action", we need to manually
			// remove all action states.
			actionContext.States.RemoveAllExcluding(typeof(InsertTextActionState));
			var actionState = actionContext.States.Get<InsertTextActionState>();

			// If we have a selection, we need to delete it and work from the
			// resulting text instead of what is currently in the buffer.
			IDisplayContext displayContext = actionContext.DisplayContext;
			Caret caret = actionContext.DisplayContext.Caret;
			BufferPosition position = caret.Position;
			var command = new Command(position);

			string lineText;

			bool deletedSelection = DeleteSelection(
				actionContext, command, ref position, out lineText);

			if (!deletedSelection)
			{
				// There is no selection, so get the line text from the buffer.
				lineText =
					displayContext.LineLayoutBuffer.GetLineText(caret.Position.LineIndex);
			}

			// Make the changes in the line.
			string newText = lineText.Insert(position.CharacterIndex, unicode.ToString());
			var setTextOperation = new SetTextOperation(position.LineIndex, newText);

			// Figure out if we are doing a new command or joining into the
			// previous one. If we are joining, then we just perform the operations
			// to set the line text and adding to the command.
			if (actionState != null)
			{
				// Pull out the text and see if we are entering a word boundary.
				string stateText = actionState.Text + unicode;
				int wordBoundary = displayContext.WordSplitter.GetNextWordBoundary(stateText, 0);

				if (wordBoundary == stateText.Length)
				{
					// We aren't at a word boundary, so append the text to
					// the state so we can undo/redo it later. Start by setting
					// the text to the text value it would have been if the
					// operations are combined.
					actionState.Text = stateText;

					// Change the redo operation to what would be the text
					// after this and the previous operations. The undo operation
					// doesn't change because it will be the same initial state.
					actionState.SetTextOperation.Text = newText;

					// After the insert completes, the state's end position would
					// shift to the right one more for the new character.
					position.CharacterIndex++;
					actionState.EndPosition = position;

					// Perform the operation and do the various redraws.
					// Scroll to the command's end position.
					actionContext.Do(setTextOperation);

					displayContext.Caret.Position = position;
					displayContext.ScrollToCaret();

					// We are done.
					return;
				}
			}

			// We either don't have a previous state we can append to or there
			// was no state to start with. So, create a new command with both
			// the redo and undo operations.
			command.Operations.Add(setTextOperation);

			if (!deletedSelection)
			{
				// We don't need the undo operation if we deleted a selection.
				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex, lineText));
			}

			// Shift the cursor over since we know this won't be changing lines
			// and we can avoid some additional refreshes.
			position.CharacterIndex++;

			// Perform the operation on the buffer.
			command.EndPosition = position;
			actionContext.Do(command);

			// Create a new action state and add it to the states list.
			if (actionState != null)
			{
				actionContext.States.Remove(actionState);
			}

			actionState = new InsertTextActionState(command, unicode.ToString());
			actionContext.States.Add(actionState);
		}

		#region Deleting

		/// <summary>
		/// If there is a selection already there, then this adds the operations
		/// needed to delete the selection and the corresponding undo operations.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <param name="command">The command.</param>
		/// <returns>True if operations were added to delete the selection.</returns>
		private static bool DeleteSelection(IActionContext actionContext, Command command)
		{
			BufferPosition position = new BufferPosition();
			string lineText;

			return DeleteSelection(actionContext, command, ref position, out lineText);
		}

		/// <summary>
		/// If there is a selection already there, then this adds the operations
		/// needed to delete the selection and the corresponding undo operations.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <param name="command">The command.</param>
		/// <returns>True if operations were added to delete the selection.</returns>
		private static bool DeleteSelection(IActionContext actionContext, Command command, ref BufferPosition position, out string lineText)
		{
			// If we don't have a selection, then we don't do anything.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferSegment selection = displayContext.Caret.Selection;

			if (selection.IsEmpty)
			{
				lineText = null;
				return false;
			}

			// We have a selection, so we need to create operations to delete
			// and restore it.
			BufferPosition startPosition = selection.StartPosition;
			int startLineIndex = startPosition.LineIndex;
			BufferPosition endPosition = selection.EndPosition;
			int endLineIndex =
				displayContext.LineLayoutBuffer.NormalizeLineIndex(endPosition.LineIndex);

			command.EndPosition = startPosition;
			position = startPosition;

			// If we have a single-line selection, then we have a simplier path
			// for these operations.
			ILineLayoutBuffer buffer = displayContext.LineLayoutBuffer;
			string startLine = buffer.GetLineText(startLineIndex);

			if (selection.IsSameLine)
			{
				// Pull out the new line text.
				lineText = startLine.Substring(0, startPosition.CharacterIndex) +
				          startLine.Substring(endPosition.CharacterIndex);

				// Create the operations for undoing and redoing the lines.
				command.Operations.Add(new SetTextOperation(startLineIndex, lineText));

				command.UndoOperations.Add(new SetTextOperation(startLineIndex, startLine));

				// We have the proper commands, so return that we deleted it.
				return true;
			}

			// Multi-line deletes are more complicated. Our new text will be
			// the beginning of the first line and end of the last. We put this
			// into the first line we are editing.
			string endLine = buffer.GetLineText(endLineIndex);
			int endCharacterIndex = Math.Min(endLine.Length, endPosition.CharacterIndex);

			lineText = startLine.Substring(0, startPosition.CharacterIndex) +
			           endLine.Substring(endCharacterIndex);

			command.Operations.Add(new SetTextOperation(startLineIndex, lineText));

			command.UndoOperations.Add(new SetTextOperation(startLineIndex, startLine));

			// For all the remaining lines, we delete them. We can do this in
			// a single operation that deletes all lines past the first one.
			command.Operations.Add(
				new DeleteLinesOperation(
					startLineIndex + 1, endLineIndex - startLineIndex));

			command.UndoOperations.Add(
				new InsertLinesOperation(
					startLineIndex + 1, endLineIndex - startLineIndex));

			// For undo operations, we need to recover the line text. So we
			// create a set text operation for each line.
			for (int lineIndex = startLineIndex + 1; lineIndex <= endLineIndex; lineIndex++)
			{
				// Get the line text. When we delete, we just delete one above
				// the start index because the lines will be shifting up. When
				// we are undoing it, we are moving down so we have to use the index.
				string deletedLineText = buffer.GetLineText(lineIndex);

				command.UndoOperations.Add(new SetTextOperation(lineIndex, deletedLineText));
			}

			// We populated the command index.
			return true;
		}

		/// <summary>
		/// Deletes the character to the left.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.BackSpace)]
		public static void DeleteLeft(IActionContext actionContext)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(actionContext, command))
			{
				// We have a command, so perform it and return.
				actionContext.Do(command);
				return;
			}

			// Get the position in the buffer.
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
				string previousText = lineLayoutBuffer.GetLineText(position.LineIndex - 1);
				string newText = previousText + lineText;

				// Set up the operations in the command.
				command.Operations.Add(new DeleteLinesOperation(position.LineIndex, 1));
				command.Operations.Add(
					new SetTextOperation(position.LineIndex - 1, newText));

				command.UndoOperations.Add(new InsertLinesOperation(position.LineIndex - 1, 1));
				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex - 1, previousText));
				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex, lineText));

				// Relocate the caret position to the previous line's end.
				position.LineIndex--;
				position.CharacterIndex = previousText.Length;
			}
			else
			{
				// This is a single-line manipulation, so delete the character.
				string newText = lineText.Substring(0, position.CharacterIndex - 1) +
				                 lineText.Substring(position.CharacterIndex);

				// Create the set text operation.
				command.Operations.Add(new SetTextOperation(position.LineIndex, newText));

				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex, lineText));

				// Shift the caret back.
				position.CharacterIndex--;
			}

			// Perform the command in the context.
			command.EndPosition = position;
			actionContext.Do(command);
		}

		/// <summary>
		/// Deletes the left word from the caret.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.BackSpace, ModifierType.ControlMask)]
		public static void DeleteLeftWord(IActionContext actionContext)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(actionContext, command))
			{
				// We have a command, so perform it and return.
				actionContext.Do(command);
				return;
			}

			// Get the position in the buffer.
			if (position.IsBeginningOfLine(actionContext.DisplayContext))
			{
				// We are in the beginning of the buffer, so we don't do anything.
				DeleteLeft(actionContext);
				return;
			}

			// Get the index of the previous word.
			ILineLayoutBuffer lineLayoutBuffer = displayContext.LineLayoutBuffer;
			string lineText = lineLayoutBuffer.GetLineText(position.LineIndex);
			int leftBoundary =
				displayContext.WordSplitter.GetPreviousWordBoundary(
					lineText, position.CharacterIndex);

			// Remove the text from the boundary to the caret in an operation.
			string deletedText = lineText.Substring(0, leftBoundary) +
			                     lineText.Substring(position.CharacterIndex);

			// Create the operations we need to perform the action.
			command.Operations.Add(new SetTextOperation(position.LineIndex, deletedText));

			command.UndoOperations.Add(new SetTextOperation(position.LineIndex, lineText));

			// Move the position to the left boundary.
			position.CharacterIndex = leftBoundary;

			// Perform the operation.
			command.EndPosition = position;
			actionContext.Do(command);
		}

		/// <summary>
		/// Deletes the character to the right.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.Delete)]
		public static void DeleteRight(IActionContext actionContext)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(actionContext, command))
			{
				// We have a command, so perform it and return.
				actionContext.Do(command);
				return;
			}

			// Get the position in the buffer.
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
				string nextText = lineLayoutBuffer.GetLineText(position.LineIndex + 1);
				string newText = lineText + nextText;

				// Set up the operations and add them to the command.
				command.Operations.Add(new DeleteLinesOperation(position.LineIndex + 1, 1));
				command.Operations.Add(new SetTextOperation(position.LineIndex, newText));

				command.UndoOperations.Add(new InsertLinesOperation(position.LineIndex, 1));
				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex, lineText));
				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex + 1, nextText));
			}
			else
			{
				// This is a single-line manipulation, so delete the character.
				string newText = lineText.Substring(0, position.CharacterIndex) +
				                 lineText.Substring(position.CharacterIndex + 1);

				// Create the operations for both performing and undoing the command.
				command.Operations.Add(new SetTextOperation(position.LineIndex, newText));

				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex, lineText));
			}

			// Perform the command to the action context.
			command.EndPosition = position;
			actionContext.Do(command);
		}

		/// <summary>
		/// Deletes the right word from the caret.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.Delete, ModifierType.ControlMask)]
		public static void DeleteRightWord(IActionContext actionContext)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(actionContext, command))
			{
				// We have a command, so perform it and return.
				actionContext.Do(command);
				return;
			}

			// Get the position in the buffer.
			if (position.IsEndOfLine(actionContext.DisplayContext))
			{
				// We are in the beginning of the buffer, so we don't do anything.
				DeleteRight(actionContext);
				return;
			}

			// Get the index of the previous word.
			ILineLayoutBuffer lineLayoutBuffer = displayContext.LineLayoutBuffer;
			string lineText = lineLayoutBuffer.GetLineText(position.LineIndex);
			int rightBoundary = displayContext.WordSplitter.GetNextWordBoundary(
				lineText, position.CharacterIndex);

			// Delete the text segment from the string.
			string deletedText = lineText.Substring(0, position.CharacterIndex) +
			                     lineText.Substring(rightBoundary);

			// Create a operations that wraps the operations.
			command.Operations.Add(new SetTextOperation(position.LineIndex, deletedText));

			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Finish by performing the command.
			actionContext.Do(command);
		}

		#endregion
	}
}