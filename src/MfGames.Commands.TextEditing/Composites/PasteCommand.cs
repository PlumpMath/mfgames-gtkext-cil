// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands.TextEditing.Composites
{
	/// <summary>
	/// A command that pastes the given text into the buffer.
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class PasteCommand<TContext>: CompositeCommand<TContext>
	{
		#region Constructors

		public PasteCommand(
			ITextEditingCommandController<TContext> controller,
			TextPosition position,
			string text)
			: base(true, false)
		{
			// Split the clipboard text into different lines.
			string[] lines = text.Split('\n');

			// If we have only one line, then we just insert the command.
			if (lines.Length == 1)
			{
				IInsertTextCommand<TContext> singleCommand =
					controller.CreateInsertTextCommand(position, text);

				Commands.Add(singleCommand);

				return;
			}

			// Start by splitting the first paragraph at that position.
			var splitCommand = new SplitParagraphCommand<TContext>(controller, position);

			Commands.Add(splitCommand);

			// The first line is inserted into at the position.
			IInsertTextCommand<TContext> firstCommand =
				controller.CreateInsertTextCommand(position, lines[0]);
			firstCommand.UpdateTextPosition = true;

			Commands.Add(firstCommand);

			// Loop through and add all the blank lines we'll need for the paste
			// operation.
			for (int i = 2;
				i < lines.Length;
				i++)
			{
				IInsertLineCommand<TContext> lineCommand =
					controller.CreateInsertLineCommand((Position) (position.Line + 1));

				Commands.Add(lineCommand);
			}

			// For every other line, we add the line and paste it.
			for (int i = 1;
				i < lines.Length;
				i++)
			{
				IInsertTextCommand<TContext> textCommand =
					controller.CreateInsertTextCommand(
						new TextPosition((Position) (position.Line + i), Position.Begin), lines[i]);

				Commands.Add(textCommand);
			}
		}

		#endregion
	}
}
