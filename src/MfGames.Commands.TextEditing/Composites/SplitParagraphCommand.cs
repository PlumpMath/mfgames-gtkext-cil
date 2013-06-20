// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands.TextEditing.Composites
{
	/// <summary>
	/// Represents a command that joins the current line with the previous one.
	/// It sets the cursor/caret position to the end of the previous line and adds
	/// a space between the two lines.
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class SplitParagraphCommand<TContext>: CompositeCommand<TContext>
	{
		#region Constructors

		public SplitParagraphCommand(
			ITextEditingCommandController<TContext> controller,
			TextPosition position)
			: base(true, false)
		{
			// The split paragraph consists of inserting a new line, pasting the
			// text to the right of the position into that one, and then removing
			// the text from the current line.
			int line = position.Line;

			// Start by inserting the new line.
			IInsertLineCommand<TContext> insertLineCommand =
				controller.CreateInsertLineCommand((Position) (position.Line + 1));
			insertLineCommand.UpdateTextPosition = true;

			// Insert the text from the line into the nmew line.
			IInsertTextFromTextRangeCommand<TContext> insertTextCommand =
				controller.CreateInsertTextFromTextRangeCommand(
					new TextPosition((Position) (line + 1), Position.Begin),
					new SingleLineTextRange(position.Line, position.Character, Position.End));

			// Delete the text from the current line.
			IDeleteTextCommand<TContext> deleteTextCommand =
				controller.CreateDeleteTextCommand(
					new SingleLineTextRange((Position) line, position.Character, Position.End));

			// Add the commands into the composite and indicate that the whitespace
			// command controls where the text position will end up.
			Commands.Add(insertLineCommand);
			Commands.Add(insertTextCommand);
			Commands.Add(deleteTextCommand);
		}

		#endregion
	}
}
