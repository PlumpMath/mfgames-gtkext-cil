// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
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
			throw new NotImplementedException();
		}

		public IInsertTextCommand<LineBufferOperationResults?> CreateInsertTextCommand
			(
			TextPosition textPosition,
			string text)
		{
			throw new NotImplementedException();
		}

		public IInsertTextFromTextRangeCommand<LineBufferOperationResults?>
			CreateInsertTextFromTextRangeCommand(
			TextPosition destinationPosition,
			TextRange sourceRange)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
