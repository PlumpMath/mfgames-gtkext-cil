// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Diagnostics.Contracts;

namespace MfGames.Commands.TextEditing.Composites
{
	/// <summary>
	/// Represents a command that joins the current line with the previous one.
	/// It sets the cursor/caret position to the end of the previous line and adds
	/// a space between the two lines.
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public class JoinPreviousParagraphCommand<TState>: CompositeCommand<TState>
	{
		#region Constructors

		public JoinPreviousParagraphCommand(
			ITextEditingCommandController<TState> controller,
			Position line)
			: base(true, false)
		{
			// Establish our code contracts.
			Contract.Requires<InvalidOperationException>(line.Index > 0);

			// Joining a paragraph consists of inserting the text of the current
			// paragraph into the previous one with a space and then moving the
			// cursor to the end of the original first paragraph (and space).

			// We start by appending the whitespace to the end of the first line.
			var joinedLine = new Position(line - 1);

			IInsertTextCommand<TState> whitespaceCommand =
				controller.CreateInsertTextCommand(
					new TextPosition(joinedLine, Position.End), " ");

			// Insert the text from the line into the prvious line.
			IInsertTextFromTextRangeCommand<TState> insertCommand =
				controller.CreateInsertTextFromTextRangeCommand(
					new TextPosition(joinedLine, Position.End),
					new TextRange(
						new TextPosition(line, Position.Begin),
						new TextPosition(line, Position.End)));

			// Finally, delete the current line since we merged it.
			IDeleteLineCommand<TState> deleteCommand =
				controller.CreateDeleteLineCommand(line);

			// Add the commands into the composite and indicate that the whitespace
			// command controls where the text position will end up.
			Commands.Add(whitespaceCommand);
			Commands.Add(insertCommand);
			Commands.Add(deleteCommand);

			StateCommand = whitespaceCommand;
		}

		#endregion
	}
}
