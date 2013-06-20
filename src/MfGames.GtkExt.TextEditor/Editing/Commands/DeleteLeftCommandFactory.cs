﻿// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using MfGames.Commands;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Renderers;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	/// <summary>
	/// Implements the command factory for handling the "delete left" (backspace).
	/// </summary>
	public class DeleteLeftCommandFactory: TextEditingCommandFactory
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
			return "Delete Left";
		}

		protected override void Do(
			object context,
			CommandFactoryManager<OperationContext> commandFactory,
			OperationContext operationContext,
			EditorViewController controller,
			IDisplayContext displayContext,
			BufferPosition position)
		{
			// Figure out which command we'll be passing the operation to.
			HierarchicalPath key;

			if (!displayContext.Caret.Selection.IsEmpty)
			{
				// If we have a selection, then we use the Delete Selection command.
				key = DeleteSelectionCommandFactory.Key;
			}
			else if (position.IsBeginningOfBuffer(controller.DisplayContext))
			{
				// If we are the beginning of the buffer, then we can't delete anything.
				return;
			}
			else if (position.CharacterIndex == 0)
			{
				// If we are at the beginning of the line, then we are combining paragraphs.
				key = JoinPreviousParagraphCommandFactory.Key;
			}
			else
			{
				//key = DeleteLeftCharacterCommandFactory.Key;
				return;
			}

			// Execute the command and pass the results to calling method.
			var nextCommand = new CommandFactoryReference(key);
			commandFactory.Do(context, nextCommand);
		}

		#endregion

		#region Constructors

		static DeleteLeftCommandFactory()
		{
			Key = new HierarchicalPath(
				"/Text Editor/Delete Left", HierarchicalPathOptions.InternStrings);
		}

		#endregion
	}
}
