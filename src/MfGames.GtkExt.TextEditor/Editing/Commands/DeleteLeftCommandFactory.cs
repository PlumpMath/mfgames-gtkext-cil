// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MfGames.Commands;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;
using MfGames.GtkExt.TextEditor.Renderers;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	/// <summary>
	/// Implements the command factory for handling the "delete left" (backspace).
	/// </summary>
	public class DeleteLeftCommandFactory:
		ICommandFactory<LineBufferOperationResults?>
	{
		#region Properties

		/// <summary>
		/// Contains the key for the command provided by this factory.
		/// </summary>
		public static HierarchicalPath Key { get; private set; }

		public IEnumerable<HierarchicalPath> Keys
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

		public LineBufferOperationResults? Do(
			object context,
			CommandFactoryReference commandFactoryReference,
			CommandFactoryManager<LineBufferOperationResults?> commandFactoryManager)
		{
			// Ensure the code contracts for this state.
			Contract.Requires<ArgumentNullException>(context != null);
			Contract.Requires<InvalidCastException>(context is EditorViewController);
			Contract.Requires<ArgumentNullException>(commandFactoryReference != null);

			// Pull out some useful variables for processing.
			var controller = (EditorViewController) context;
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;

			// Figure out which command we'll be passing the operation to.
			CommandFactoryReference nextCommand;

			if (!displayContext.Caret.Selection.IsEmpty)
			{
				// If we have a selection, then we use the Delete Selection command.
				//nextCommand = new CommandFactoryReference(DeleteSelectionCommandFactory.Key);
				return null;
			}
			else if (position.IsBeginningOfBuffer(controller.DisplayContext))
			{
				// If we are the beginning of the buffer, then we can't delete anything.
				return null;
			}
			else if (position.CharacterIndex == 0)
			{
				// If we are at the beginning of the line, then we are combining paragraphs.
				//nextCommand = new CommandFactoryReference(JoinPreviousParagraphCommandFactory.Key);
				return null;
			}
			else
			{
				//nextCommand = new CommandFactoryReference(DeleteLeftCharacterCommandFactory.Key);
				return null;
			}

			// Execute the command and pass the results to calling method.
			LineBufferOperationResults? results = commandFactoryManager.Do(
				context, nextCommand);
			return results;
		}

		public string GetTitle(CommandFactoryReference commandFactoryReference)
		{
			return "Delete Left";
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
