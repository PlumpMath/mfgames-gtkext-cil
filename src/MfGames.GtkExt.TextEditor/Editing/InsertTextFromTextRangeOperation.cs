// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Text;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;

namespace MfGames.GtkExt.TextEditor.Editing
{
	public class InsertTextFromTextRangeOperation:
		IInsertTextFromTextRangeCommand<OperationContext>
	{
		public TextPosition DestinationPosition { get; private set; }
		public TextRange SourceRange { get; private set; }

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
			// Grab the text from the source line.
			string sourceLine = state.LineBuffer.GetLineText(
				SourceRange.Begin.Line,LineContexts.Unformatted);
			int sourceBegin = SourceRange.Begin.Character.Normalize(sourceLine);
			int sourceEnd = SourceRange.End.Character.Normalize(sourceLine);
			string sourceText = sourceLine.Substring(
				sourceBegin, sourceEnd - sourceBegin);

			// Insert the text from the source line into the destination.
			string destinationLine = state.LineBuffer.GetLineText(
				DestinationPosition.Line,
				LineContexts.Unformatted);
			var buffer = new StringBuilder(destinationLine);
			int characterIndex = DestinationPosition.Character.Normalize(destinationLine);

			buffer.Insert(characterIndex,sourceText);

			// Set the line in the buffer.
			destinationLine = buffer.ToString();
			state.LineBuffer.SetText(DestinationPosition.Line, destinationLine);
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
			DestinationPosition = destinationPosition;
			SourceRange = sourceRange;
		}

		#endregion
	}
}
