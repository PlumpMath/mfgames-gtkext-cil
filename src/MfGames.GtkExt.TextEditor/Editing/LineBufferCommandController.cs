// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using MfGames.Commands;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Models.Buffers;

namespace MfGames.GtkExt.TextEditor.Editing
{
	public class LineBufferCommandController:
		UndoRedoCommandController<LineBufferOperationResults?>,
		ITextEditingCommandController<LineBufferOperationResults?>
	{
		#region Methods

		public IDeleteLineCommand<LineBufferOperationResults?> CreateDeleteLineCommand
			(Position line)
		{
			var operation = new DeleteLinesOperation(line, 1);
			return operation;
		}

		public IInsertTextCommand<LineBufferOperationResults?> CreateInsertTextCommand
			(
			TextPosition textPosition,
			string text)
		{
			var operation = new InsertTextOperation(
				textPosition.Line, textPosition.Character, text);
			return operation;
		}

		public IInsertTextFromTextRangeCommand<LineBufferOperationResults?>
			CreateInsertTextFromTextRangeCommand(
			TextPosition destinationPosition,
			TextRange sourceRange)
		{
			var operation = new InsertTextFromTextRangeOperation(
				destinationPosition, sourceRange);
			return operation;
		}

		#endregion
	}
}
