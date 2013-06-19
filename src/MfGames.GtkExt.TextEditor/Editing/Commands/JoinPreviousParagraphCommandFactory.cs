// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MfGames.Commands;
using MfGames.Commands.TextEditing;
using MfGames.Commands.TextEditing.Composites;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	public class JoinPreviousParagraphCommandFactory:
		ICommandFactory<OperationContext>
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

		public void Do(
			object context,
			CommandFactoryReference commandFactoryReference,
			CommandFactoryManager<OperationContext> commandFactoryManager)
		{
			// Ensure the code contracts for this state.
			Contract.Requires<ArgumentNullException>(context != null);
			Contract.Requires<InvalidCastException>(context is EditorViewController);
			Contract.Requires<ArgumentNullException>(commandFactoryReference != null);

			// Pull out some useful variables for processing.
			var controller = (EditorViewController) context;
			IDisplayContext displayContext = controller.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;

			// Create the join previous paragraph command.
			var operationContext =
				new OperationContext(controller.DisplayContext.LineBuffer);
			var command =
				new JoinPreviousParagraphCommand<OperationContext>(
					controller.CommandController, (Position) position.LineIndex);

			controller.CommandController.Do(command, operationContext);
		}

		public string GetTitle(CommandFactoryReference commandFactoryReference)
		{
			return "Join Previous Paragraph";
		}

		#endregion

		#region Constructors

		static JoinPreviousParagraphCommandFactory()
		{
			Key = new HierarchicalPath(
				"/Text Editor/Join Previous Paragraph",
				HierarchicalPathOptions.InternStrings);
		}

		#endregion
	}
}
