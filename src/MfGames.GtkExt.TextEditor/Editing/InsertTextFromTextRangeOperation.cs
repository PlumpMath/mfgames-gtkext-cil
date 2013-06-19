// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Models.Buffers;

namespace MfGames.GtkExt.TextEditor.Editing
{
	public class InsertTextFromTextRangeOperation:
		IInsertTextFromTextRangeCommand<LineBufferOperationResults?>
	{
		#region Properties

		public bool CanUndo
		{
			get { return true; }
		}

		public bool IsTransient
		{
			get { return false; }
		}

		#endregion

		#region Methods

		public LineBufferOperationResults? Do(LineBufferOperationResults? state)
		{
			throw new NotImplementedException();
		}

		public LineBufferOperationResults? Redo(LineBufferOperationResults? state)
		{
			throw new NotImplementedException();
		}

		public LineBufferOperationResults? Undo(LineBufferOperationResults? state)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Constructors

		public InsertTextFromTextRangeOperation(
			TextPosition destinationPosition,
			TextRange sourceRange)
		{
		}

		#endregion
	}
}
