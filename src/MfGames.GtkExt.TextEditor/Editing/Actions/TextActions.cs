// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Text;
using Gdk;
using Gtk;
using MfGames.Commands;
using MfGames.GtkExt.TextEditor.Editing.Commands;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;
using MfGames.GtkExt.TextEditor.Renderers;
using Key = Gdk.Key;

namespace MfGames.GtkExt.TextEditor.Editing.Actions
{
	/// <summary>
	/// Contains the various actions used for moving the caret (cursor) around
	/// the text buffer.
	/// </summary>
	[ActionFixture]
	public static class TextActions
	{
		#region Methods

		/// <summary>
		/// Copies the selection into the clipboard.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[ActionState(typeof (VerticalMovementActionState))]
		[KeyBinding(Key.C, ModifierType.ControlMask)]
		[KeyBinding(Key.Insert, ModifierType.ControlMask)]
		public static void Copy(EditorViewController controller)
		{
			// If we don't have anything selected, we don't do anything.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferSegment selection = displayContext.Caret.Selection;

			if (selection.IsEmpty)
			{
				return;
			}

			// Go through the selection and figure out if we have a single-line
			// copy.
			LineBuffer lineBuffer = displayContext.LineBuffer;

			int endLineIndex =
				lineBuffer.NormalizeLineIndex(selection.EndPosition.LineIndex);
			string firstLine = lineBuffer.GetLineText(
				selection.StartPosition.LineIndex, LineContexts.Unformatted);

			if (endLineIndex == selection.StartPosition.LineIndex)
			{
				// Single-line copy is much easier since we just need a substring.
				string singleLineText =
					firstLine.Substring(
						selection.StartPosition.CharacterIndex,
						selection.EndPosition.CharacterIndex
							- selection.StartPosition.CharacterIndex);

				// Set the clipboard's text and return.
				displayContext.Clipboard.Text = singleLineText;
				return;
			}

			// For multiple line copies, we need to copy every line from the first
			// to the last. We already have the first, so just copy that.
			var buffer = new StringBuilder();
			buffer.Append(firstLine.Substring(selection.StartPosition.CharacterIndex));
			buffer.Append("\n");

			// Loop through the second to just shy of the last line, adding
			// each one as a full line.
			for (int lineIndex = selection.StartPosition.LineIndex + 1;
				lineIndex < endLineIndex;
				lineIndex++)
			{
				buffer.Append(lineBuffer.GetLineText(lineIndex, LineContexts.Unformatted));
				buffer.Append("\n");
			}

			// Add the last line, which is a substring, but we don't add a
			// newline to the end of this one.
			buffer.Append(
				lineBuffer.GetLineText(endLineIndex, LineContexts.Unformatted)
				          .Substring(0, selection.EndPosition.CharacterIndex));

			// Set the clipboard value.
			displayContext.Clipboard.Text = buffer.ToString();
		}

		/// <summary>
		/// Copies, then deletes the selected text.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.X, ModifierType.ControlMask)]
		public static void Cut(EditorViewController controller)
		{
			// If we don't have anything selected, we don't do anything.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferSegment selection = displayContext.Caret.Selection;

			if (selection.IsEmpty)
			{
				return;
			}

			// Copy the text first.
			Copy(controller);

			// Then delete the text. Since we know we have a selection, this
			// will only delete the selected text.
			DeleteLeft(controller);
		}

		/// <summary>
		/// Deletes the character to the left.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.BackSpace)]
		public static void DeleteLeft(EditorViewController controller)
		{
			// Bridge into the new command controller subsystem.
			var commandReference = new CommandFactoryReference(DeleteLeftCommandFactory.Key);
			controller.CommandFactory.Do(controller, commandReference);

			//// If we have a selection, then we simply delete that selection.
			//IDisplayContext displayContext = controller.DisplayContext;
			//BufferPosition position = displayContext.Caret.Position;
			//var command = new Command(position);

			//if (DeleteSelection(controller, command))
			//{
			//	// We have a command, so perform it and return.
			//	controller.Do(command, command.EndPosition);
			//	return;
			//}

			//// Get the position in the buffer.
			//if (position.IsBeginningOfBuffer(controller.DisplayContext))
			//{
			//	// We are in the beginning of the buffer, so we don't do anything.
			//	return;
			//}

			//// If we are at the beginning of the line, then we are combining paragraphs.
			//if (position.CharacterIndex == 0)
			//{
			//	// Delete the paragraph.
			//	DeleteLeftParagraph(controller, command);
			//	return;
			//}

			//// This is a single character delete which doesn't combine paragraphs.
			//LineBuffer lineBuffer = displayContext.LineBuffer;
			//string lineText = lineBuffer.GetLineText(
			//	position.LineIndex, LineContexts.Unformatted);

			//// Create the operation and its undo.
			//command.Operations.Add(
			//	new DeleteTextOperation(
			//		position.LineIndex, position.CharacterIndex - 1, position.CharacterIndex));

			//command.UndoOperations.Add(
			//	new SetTextOperation(position.LineIndex, lineText));

			//// Perform the command in the context.
			//controller.Do(command);
		}

		/// <summary>
		/// Deletes the left word from the caret.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.BackSpace, ModifierType.ControlMask)]
		public static void DeleteLeftWord(EditorViewController controller)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(controller, command))
			{
				// We have a command, so perform it and return.
				controller.Do(command, command.EndPosition);
				return;
			}

			// Get the position in the buffer.
			if (position.IsBeginningOfBuffer(controller.DisplayContext))
			{
				// We are in the beginning of the buffer, so we don't do anything.
				return;
			}

			// Get the position in the buffer.
			if (position.IsBeginningOfLine(controller.DisplayContext))
			{
				// We are in the beginning of the line, so delete the paragraph.
				DeleteLeftParagraph(controller, command);
				return;
			}

			// Get the index of the previous word.
			LineBuffer lineBuffer = displayContext.LineBuffer;
			string lineText = lineBuffer.GetLineText(
				position.LineIndex, LineContexts.CurrentLine);
			int leftBoundary =
				displayContext.WordSplitter.GetPreviousWordBoundary(
					lineText, position.CharacterIndex);

			// Create the operations we need to perform the action.
			command.Operations.Add(
				new DeleteTextOperation(
					position.LineIndex, leftBoundary, position.CharacterIndex));

			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Perform the operation.
			controller.Do(command);
		}

		/// <summary>
		/// Deletes the character to the right.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.Delete)]
		public static void DeleteRight(EditorViewController controller)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(controller, command))
			{
				// We have a command, so perform it and return.
				controller.Do(command, command.EndPosition);
				return;
			}

			// Get the position in the buffer.
			if (position.IsEndOfBuffer(controller.DisplayContext))
			{
				// We are in the end of the buffer, so we don't do anything.
				return;
			}

			// If we are at the beginning of the line, then we are combining paragraphs.
			LineBuffer lineBuffer = displayContext.LineBuffer;
			string lineText = lineBuffer.GetLineText(
				position.LineIndex, LineContexts.Unformatted);
			int deleteIndex = position.CharacterIndex;

			if (deleteIndex == lineText.Length)
			{
				DeleteRightParagraph(controller, command);
				return;
			}

			// Create the operations for both performing and undoing the command.
			command.Operations.Add(
				new DeleteTextOperation(position.LineIndex, deleteIndex, deleteIndex + 1));

			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Perform the command to the action context.
			controller.Do(command);
		}

		/// <summary>
		/// Deletes the right word from the caret.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.Delete, ModifierType.ControlMask)]
		public static void DeleteRightWord(EditorViewController controller)
		{
			// If we have a selection, then we simply delete that selection.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			var command = new Command(position);

			if (DeleteSelection(controller, command))
			{
				// We have a command, so perform it and return.
				controller.Do(command, command.EndPosition);
				return;
			}

			// If we're at the end of the buffer, do nothing.
			if (position.IsEndOfBuffer(controller.DisplayContext))
			{
				// We are in the end of the buffer, so we don't do anything.
				return;
			}

			// If we are at the end of the line, delete the next paragraph.
			if (position.IsEndOfLine(controller.DisplayContext))
			{
				// We are in the beginning of the buffer, so we don't do anything.
				DeleteRightParagraph(controller, command);
				return;
			}

			// Get the index of the previous word.
			LineBuffer lineBuffer = displayContext.LineBuffer;
			string lineText = lineBuffer.GetLineText(
				position.LineIndex, LineContexts.Unformatted);
			int rightBoundary = displayContext.WordSplitter.GetNextWordBoundary(
				lineText, position.CharacterIndex);

			// Create a operations that wraps the operations.
			command.Operations.Add(
				new DeleteTextOperation(
					position.LineIndex, position.CharacterIndex, rightBoundary));

			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Finish by performing the command.
			controller.Do(command);
		}

		/// <summary>
		/// Inserts the paragraph at the current buffer position.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.Return)]
		public static void InsertParagraph(EditorViewController controller)
		{
			// Get the text of the current line.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			string lineText = displayContext.LineBuffer.GetLineText(
				position.LineIndex, LineContexts.Unformatted);

			// Split the line based on the character index.
			string before = lineText.Substring(0, position.CharacterIndex);
			string after = lineText.Substring(position.CharacterIndex);

			// Create the command and add the operations. To actually composite
			// the command, we create a new line and set the text of both lines
			// to the before and after.
			var command = new Command(position);

			command.Operations.Add(new InsertLinesOperation(position.LineIndex, 1));
			command.Operations.Add(new SetTextOperation(position.LineIndex, before));
			command.Operations.Add(new ExitLineOperation(position.LineIndex));
			command.Operations.Add(new SetTextOperation(position.LineIndex + 1, after));

			// The undo operation deletes the created line and sets the text of
			// the first line to the original contents.
			command.UndoOperations.Add(new DeleteLinesOperation(position.LineIndex, 1));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Change the buffer position.
			position.LineIndex++;
			position.CharacterIndex = 0;

			// Perform the operations in the command and set the position.
			controller.Do(command, position);
		}

		/// <summary>
		/// Moves the caret down one line.
		/// </summary>
		/// <param name="controller">The display context.</param>
		/// <param name="unicode">The Unicode character.</param>
		public static void InsertText(
			EditorViewController controller,
			char unicode)
		{
			// Because InsertText isn't a proper "action", we need to manually
			// remove all action states.
			controller.States.RemoveAllExcluding(typeof (InsertTextActionState));
			var actionState = controller.States.Get<InsertTextActionState>();

			// If we have a selection, we need to delete it and work from the
			// resulting text instead of what is currently in the buffer.
			IDisplayContext displayContext = controller.DisplayContext;
			Caret caret = controller.DisplayContext.Caret;
			BufferPosition position = caret.Position;
			var command = new Command(position);

			string lineText;

			bool deletedSelection = DeleteSelection(
				controller, command, ref position, out lineText);

			if (!deletedSelection)
			{
				// There is no selection, so get the line text from the buffer.
				lineText = displayContext.LineBuffer.GetLineText(
					caret.Position.LineIndex, LineContexts.Unformatted);
			}

			// Create the operation to change the text.
			var insertTextOperation = new InsertTextOperation(
				position, unicode.ToString());

			// Figure out if we are doing a new command or joining into the
			// previous one. If we are joining, then we just perform the operations
			// to set the line text and adding to the command.
			if (actionState != null)
			{
				// Pull out the text and see if we are entering a word boundary.
				string stateText = actionState.Text + unicode;
				int wordBoundary = displayContext.WordSplitter.GetNextWordBoundary(
					stateText, 0);

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
					actionState.Operation.Text = stateText;

					// After the insert completes, the state's end position would
					// shift to the right one more for the new character.
					position.CharacterIndex++;
					actionState.EndPosition = position;

					// Perform the operation and do the various redraws.
					// Scroll to the command's end position.
					LineBufferOperationResults results = controller.Do(insertTextOperation);

					displayContext.ScrollToCaret(results.BufferPosition);

					// We are done.
					return;
				}
			}

			// We either don't have a previous state we can append to or there
			// was no state to start with. So, create a new command with both
			// the redo and undo operations.
			command.Operations.Add(insertTextOperation);

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
			controller.Do(command);

			// Create a new action state and add it to the states list.
			if (actionState != null)
			{
				controller.States.Remove(actionState);
			}

			actionState = new InsertTextActionState(command, unicode.ToString());
			controller.States.Add(actionState);
		}

		/// <summary>
		/// Inserts the text into the buffer.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="input">The input.</param>
		public static void InsertText(
			EditorViewController controller,
			string input)
		{
			foreach (char c in input)
			{
				InsertText(controller, c);
			}
		}

		/// <summary>
		/// Pastes the contents of the clipboard into the buffer.
		/// </summary>
		/// <param name="controller">The action context.</param>
		[Action]
		[KeyBinding(Key.V, ModifierType.ControlMask)]
		public static void Paste(EditorViewController controller)
		{
			// Get the text from the clipboard.
			IDisplayContext displayContext = controller.DisplayContext;
			Clipboard clipboard = displayContext.Clipboard;

			clipboard.RequestText(null);

			string clipboardText = clipboard.WaitForText();

			if (string.IsNullOrEmpty(clipboardText))
			{
				return;
			}

			// See if the last character ends in newlines. We need that to figure out how
			// we'll be pasting the last line.
			bool lastIsEol = clipboardText[clipboardText.Length - 1] == '\n';

			// Split the clipboard text into different lines.
			string[] lines = clipboardText.Split('\n');

			// If there is a selection, then we want to delete it using the common
			// processing for selections.
			Caret caret = displayContext.Caret;
			BufferPosition position = caret.Position;
			var command = new Command();
			string lineText;

			bool deletedSelection = DeleteSelection(
				controller, command, ref position, out lineText);

			if (!deletedSelection)
			{
				// There is no selection, so get the line text from the buffer.
				lineText = displayContext.LineBuffer.GetLineText(
					caret.Position.LineIndex, LineContexts.Unformatted);
			}

			string nextLineText =
				displayContext.LineBuffer.GetLineText(
					caret.Position.LineIndex + 1, LineContexts.Unformatted);

			// The paste will happen in the line, splitting the current line in half.
			string before = lineText.Substring(0, position.CharacterIndex);
			string after = lineText.Substring(position.CharacterIndex);

			// Check to see if we only have one line and it doesn't end in a newline.
			if (lines.Length == 1
				&& !lastIsEol)
			{
				// Just before a set text operation.
				command.Operations.Add(
					new SetTextOperation(position.LineIndex, before + lines[0] + after));

				if (!deletedSelection)
				{
					command.UndoOperations.Add(
						new SetTextOperation(position.LineIndex, lineText));
				}

				// Set the end of the command position.
				position.CharacterIndex = (before + lines[0]).Length;
				command.EndPosition = position;

				// Perform the command.
				controller.Do(command);
				return;
			}

			// Insert the number of lines we'll need past the first.
			int linesNeeded = lines.Length - 1;

			command.Operations.Add(
				new InsertLinesOperation(position.LineIndex, linesNeeded));

			command.UndoOperations.Add(
				new DeleteLinesOperation(position.LineIndex + 1, linesNeeded));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex + 1, nextLineText));

			if (!deletedSelection)
			{
				command.UndoOperations.Add(
					new SetTextOperation(position.LineIndex, lineText));
			}

			// The first pasted line will combine with the before text.
			before += lines[0];

			command.Operations.Add(new SetTextOperation(position.LineIndex, before));

			// Insert the lines between the first and the last one, exclusive.
			for (int index = 1;
				index < linesNeeded;
				index++)
			{
				command.Operations.Add(
					new SetTextOperation(position.LineIndex + index, lines[index]));
			}

			// If the last does not end in an EOL, then we need to combine it.
			command.Operations.Add(
				new SetTextOperation(
					position.LineIndex + lines.Length - 1, lines[lines.Length - 1] + after));

			// Perform the command.
			controller.Do(command);
		}

		/// <summary>
		/// Deletes the paragraph to the left of the character.
		/// </summary>
		/// <param name="controller">The action context.</param>
		/// <param name="command">The command.</param>
		private static void DeleteLeftParagraph(
			EditorViewController controller,
			Command command)
		{
			// Pull out useful fields.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			LineBuffer lineBuffer = displayContext.LineBuffer;
			string lineText = lineBuffer.GetLineText(
				position.LineIndex, LineContexts.Unformatted);

			// This is the beginning of a paragraph and not the first one in
			// the buffer. This operation combines the text of the two paragraphs
			// together.
			string previousText = lineBuffer.GetLineText(
				position.LineIndex - 1, LineContexts.Unformatted);
			string newText = previousText + lineText;

			// Set up the operations in the command.
			command.Operations.Add(new DeleteLinesOperation(position.LineIndex, 1));
			command.Operations.Add(new SetTextOperation(position.LineIndex - 1, newText));

			command.UndoOperations.Add(
				new InsertLinesOperation(position.LineIndex - 1, 1));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex - 1, previousText));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));

			// Relocate the caret position to the previous line's end.
			position.LineIndex--;
			position.CharacterIndex = previousText.Length;

			// Perform the operations in the command and set the position.
			controller.Do(command, position);
		}

		/// <summary>
		/// Deletes the paragraph to the right of the caret.
		/// </summary>
		/// <param name="controller">The action context.</param>
		/// <param name="command">The command.</param>
		private static void DeleteRightParagraph(
			EditorViewController controller,
			Command command)
		{
			// Pull out useful fields.
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			LineBuffer lineBuffer = displayContext.LineBuffer;
			string lineText = lineBuffer.GetLineText(
				position.LineIndex, LineContexts.Unformatted);

			// This is the end of a paragraph and not the first one in
			// the buffer. This operation combines the text of the two paragraphs
			// together.
			string nextText = lineBuffer.GetLineText(
				position.LineIndex + 1, LineContexts.Unformatted);
			string newText = lineText + nextText;

			// Set up the operations in the command.
			command.Operations.Add(new DeleteLinesOperation(position.LineIndex + 1, 1));
			command.Operations.Add(new SetTextOperation(position.LineIndex, newText));

			command.UndoOperations.Add(new InsertLinesOperation(position.LineIndex, 1));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex, lineText));
			command.UndoOperations.Add(
				new SetTextOperation(position.LineIndex + 1, nextText));

			// Relocate the caret position to the previous line's end.
			position.CharacterIndex = lineText.Length;

			// Perform the operations in the command and set the position.
			controller.Do(command, position);
		}

		/// <summary>
		/// If there is a selection already there, then this adds the operations
		/// needed to delete the selection and the corresponding undo operations.
		/// </summary>
		/// <param name="controller">The action context.</param>
		/// <param name="command">The command.</param>
		/// <returns>True if operations were added to delete the selection.</returns>
		private static bool DeleteSelection(
			EditorViewController controller,
			Command command)
		{
			var position = new BufferPosition();
			string lineText;

			return DeleteSelection(controller, command, ref position, out lineText);
		}

		/// <summary>
		/// If there is a selection already there, then this adds the operations
		/// needed to delete the selection and the corresponding undo operations.
		/// </summary>
		/// <param name="controller">The action context.</param>
		/// <param name="command">The command.</param>
		/// <param name="position">The position.</param>
		/// <param name="lineText">The line text.</param>
		/// <returns>
		/// True if operations were added to delete the selection.
		/// </returns>
		private static bool DeleteSelection(
			EditorViewController controller,
			Command command,
			ref BufferPosition position,
			out string lineText)
		{
			// If we don't have a selection, then we don't do anything.
			IDisplayContext displayContext = controller.DisplayContext;
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
				displayContext.LineBuffer.NormalizeLineIndex(endPosition.LineIndex);

			command.EndPosition = startPosition;
			position = startPosition;

			// If we have a single-line selection, then we have a simplier path
			// for these operations.
			LineBuffer lineBuffer = displayContext.LineBuffer;
			string startLine = lineBuffer.GetLineText(
				startLineIndex, LineContexts.Unformatted);
			int deleteIndex = startPosition.CharacterIndex;

			if (selection.IsSameLine)
			{
				// Pull out the new line text.
				lineText = startLine.Substring(0, deleteIndex)
					+ startLine.Substring(endPosition.CharacterIndex);

				// Create the operations for undoing and redoing the lines.
				command.Operations.Add(
					new DeleteTextOperation(
						startLineIndex, deleteIndex, endPosition.CharacterIndex));

				command.UndoOperations.Add(new SetTextOperation(startLineIndex, startLine));

				// We have the proper commands, so return that we deleted it.
				return true;
			}

			// Multi-line deletes are more complicated. Our new text will be
			// the beginning of the first line and end of the last. We put this
			// into the first line we are editing.
			string endLine = lineBuffer.GetLineText(
				endLineIndex, LineContexts.Unformatted);
			int endCharacterIndex = Math.Min(endLine.Length, endPosition.CharacterIndex);

			lineText = startLine.Substring(0, deleteIndex)
				+ endLine.Substring(endCharacterIndex);

			command.Operations.Add(new SetTextOperation(startLineIndex, lineText));

			command.UndoOperations.Add(new SetTextOperation(startLineIndex, startLine));

			// For all the remaining lines, we delete them. We can do this in
			// a single operation that deletes all lines past the first one.
			command.Operations.Add(
				new DeleteLinesOperation(startLineIndex + 1, endLineIndex - startLineIndex));

			command.UndoOperations.Add(
				new InsertLinesOperation(startLineIndex + 1, endLineIndex - startLineIndex));

			// For undo operations, we need to recover the line text. So we
			// create a set text operation for each line.
			for (int lineIndex = startLineIndex + 1;
				lineIndex <= endLineIndex;
				lineIndex++)
			{
				// Get the line text. When we delete, we just delete one above
				// the start index because the lines will be shifting up. When
				// we are undoing it, we are moving down so we have to use the index.
				string deletedLineText = lineBuffer.GetLineText(
					lineIndex, LineContexts.Unformatted);

				command.UndoOperations.Add(new SetTextOperation(lineIndex, deletedLineText));
			}

			// We populated the command index.
			return true;
		}

		#endregion
	}
}
