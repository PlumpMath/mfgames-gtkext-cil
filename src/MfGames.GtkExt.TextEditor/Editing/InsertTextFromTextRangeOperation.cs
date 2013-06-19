// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;

namespace MfGames.GtkExt.TextEditor.Editing
{
	public class InsertTextFromTextRangeOperation:
		IInsertTextFromTextRangeCommand<OperationContext>
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

		public void Do(OperationContext state)
		{
			throw new NotImplementedException();
		}

		public void Redo(OperationContext state)
		{
			throw new NotImplementedException();
		}

		public void Undo(OperationContext state)
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
