// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using MfGames.Commands;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	public class DeleteSelectionCommandFactory: TextEditingCommandFactory
	{
		#region Properties

		/// <summary>
		/// Contains the key for the command provided by this factory.
		/// </summary>
		public static HierarchicalPath Key { get; private set; }

		public override IEnumerable<HierarchicalPath> Keys
		{
			get
			{
				return new[]
				{
					Key
				};
			}
		}

		#endregion

		#region Methods

		public override string GetTitle(
			CommandFactoryReference commandFactoryReference)
		{
			return "Join Previous Paragraph";
		}

		protected override void Do(
			object context,
			CommandFactoryManager<OperationContext> commandFactory,
			OperationContext operationContext,
			EditorViewController controller,
			IDisplayContext displayContext,
			BufferPosition position)
		{
			BufferSegment selection = displayContext.Caret.Selection;
			IDeleteTextCommand<OperationContext> deleteCommand =
				controller.CommandController.CreateDeleteTextCommand(
					new TextRange(
						new TextPosition(
							(Position) selection.StartPosition.LineIndex,
							(Position) selection.StartPosition.CharacterIndex),
						new TextPosition(
							(Position) selection.EndPosition.LineIndex,
							(Position) selection.EndPosition.CharacterIndex)));
			deleteCommand.UpdateTextPosition = true;

			// Execute the command.
			controller.CommandController.Do(deleteCommand, operationContext);

			// If we have a text position, we need to set it.
			if (operationContext.Results.HasValue)
			{
				displayContext.Caret.Position =
					operationContext.Results.Value.BufferPosition;
			}
		}

		#endregion

		#region Constructors

		static DeleteSelectionCommandFactory()
		{
			Key = new HierarchicalPath(
				"/Text Editor/Delete Selection", HierarchicalPathOptions.InternStrings);
		}

		#endregion
	}
}
