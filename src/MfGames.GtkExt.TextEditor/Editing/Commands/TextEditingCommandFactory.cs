// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Diagnostics.Contracts;
using MfGames.Commands;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	public abstract class TextEditingCommandFactory:
		ICommandFactory<OperationContext>
	{
		#region Properties

		public abstract HierarchicalPath FactoryKey { get; }

		#endregion

		#region Methods

		public void Do(
			object context,
			CommandFactoryReference commandFactoryReference,
			CommandFactoryManager<OperationContext> controller)
		{
			// Ensure the code contracts for this state.
			Contract.Requires<ArgumentNullException>(context != null);
			Contract.Requires<InvalidCastException>(context is EditorViewController);
			Contract.Requires<ArgumentNullException>(commandFactoryReference != null);

			// Pull out some useful variables for processing.
			var editViewController = (EditorViewController) context;
			IDisplayContext displayContext = editViewController.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;

			// Set up the operation context for this request.
			var operationContext = new OperationContext(
				displayContext.LineBuffer,
				new TextPosition(
					(Position) displayContext.Caret.Position.LineIndex,
					(Position) displayContext.Caret.Position.CharacterIndex));

			// Create the commands and execute them.
			Do(
				context,
				controller,
				operationContext,
				editViewController,
				displayContext,
				position);
		}

		public abstract string GetTitle(
			CommandFactoryReference commandFactoryReference);

		protected abstract void Do(
			object context,
			CommandFactoryManager<OperationContext> commandFactory,
			OperationContext operationContext,
			EditorViewController editViewController,
			IDisplayContext displayContext,
			BufferPosition position);

		#endregion
	}
}
