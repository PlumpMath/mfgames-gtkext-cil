// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using MfGames.GtkExt.TextEditor.Editing;
using MfGames.GtkExt.TextEditor.Editing.Commands;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.HierarchicalPaths;

namespace MfGames.Commands
{
	public class RedoCommandFactory: TextEditingCommandFactory
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
			return "Redo";
		}

		protected override void Do(
			object context,
			CommandFactoryManager<OperationContext> commandFactory,
			OperationContext operationContext,
			EditorViewController controller,
			IDisplayContext displayContext,
			BufferPosition position)
		{
			if (controller.CommandController.CanRedo)
			{
				controller.CommandController.Redo(operationContext);
			}
		}

		#endregion

		#region Constructors

		static RedoCommandFactory()
		{
			Key = new HierarchicalPath("/Redo", HierarchicalPathOptions.InternStrings);
		}

		#endregion
	}
}
