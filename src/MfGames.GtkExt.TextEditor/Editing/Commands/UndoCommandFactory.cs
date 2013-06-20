// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using MfGames.Commands;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	public class UndoCommandFactory: TextEditingCommandFactory
	{
		#region Properties

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
			return "Undo";
		}

		protected override void Do(
			object context,
			CommandFactoryManager<OperationContext> commandFactory,
			OperationContext operationContext,
			EditorViewController controller,
			IDisplayContext displayContext,
			BufferPosition position)
		{
			if (controller.CommandController.CanUndo)
			{
				controller.CommandController.Undo(operationContext);
			}
		}

		#endregion

		#region Constructors

		static UndoCommandFactory()
		{
			Key = new HierarchicalPath("/Undo", HierarchicalPathOptions.InternStrings);
		}

		#endregion
	}
}
