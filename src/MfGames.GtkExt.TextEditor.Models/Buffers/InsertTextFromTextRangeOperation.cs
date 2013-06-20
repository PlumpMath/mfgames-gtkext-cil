// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Text;
using MfGames.Commands.TextEditing;

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	public class InsertTextFromTextRangeOperation: TextEditingOperation,
		IInsertTextFromTextRangeCommand<OperationContext>
	{
		#region Properties

		public TextPosition DestinationPosition { get; private set; }
		public TextRange SourceRange { get; private set; }

		#endregion

		#region Methods

		public override void Do(OperationContext state)
		{
			// Grab the text from the source line.
			string sourceLine = state.LineBuffer.GetLineText(
				SourceRange.Begin.Line, LineContexts.Unformatted);
			int sourceBegin = SourceRange.Begin.Character.Normalize(sourceLine);
			int sourceEnd = SourceRange.End.Character.Normalize(sourceLine);
			string sourceText = sourceLine.Substring(
				sourceBegin, sourceEnd - sourceBegin);

			// Insert the text from the source line into the destination.
			string destinationLine =
				state.LineBuffer.GetLineText(
					DestinationPosition.Line, LineContexts.Unformatted);
			var buffer = new StringBuilder(destinationLine);
			int characterIndex = DestinationPosition.Character.Normalize(destinationLine);

			buffer.Insert(characterIndex, sourceText);

			// Save the source text length so we can delete it.
			sourceLength = sourceText.Length;
			originalCharacterIndex = characterIndex;

			// Set the line in the buffer.
			destinationLine = buffer.ToString();
			state.LineBuffer.SetText(DestinationPosition.Line, destinationLine);
		}

		public override void Redo(OperationContext state)
		{
			Do(state);
		}

		public override void Undo(OperationContext state)
		{
			// Grab the line from the line buffer.
			string lineText = state.LineBuffer.GetLineText(
				DestinationPosition.Line, LineContexts.Unformatted);
			var buffer = new StringBuilder(lineText);

			// Normalize the character ranges.
			buffer.Remove(originalCharacterIndex, sourceLength);

			// Set the line in the buffer.
			lineText = buffer.ToString();
			state.LineBuffer.SetText(DestinationPosition.Line, lineText);

			// If we are updating the position, we need to do it here.
			if (UpdateTextPosition)
			{
				state.Results =
					new LineBufferOperationResults(
						new BufferPosition(DestinationPosition.Line, originalCharacterIndex));
			}
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

		#region Fields

		private int originalCharacterIndex;
		private int sourceLength;

		#endregion
	}
}
