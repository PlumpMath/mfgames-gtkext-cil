// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using MfGames.Commands;
using MfGames.Commands.TextEditing;
using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	public class InsertKeyCommandFactory:TextEditingCommandFactory
	{
		#region Properties

		public override HierarchicalPath FactoryKey
		{
			get { return Key; }
		}

		/// <summary>
		/// Contains the key for the command provided by this factory.
		/// </summary>
		public static HierarchicalPath Key { get; private set; }

		#endregion

		#region Methods

		public override string GetTitle(
			CommandFactoryReference commandFactoryReference)
		{
			return "Insert Key";
		}

		protected override void Do(
			object context,
			CommandFactoryManager<OperationContext> commandFactory,
			object commandData,
			OperationContext operationContext,
			EditorViewController controller,
			IDisplayContext displayContext,
			BufferPosition position)
		{
			// Based on the text, figure out what to do.
			var key = (char) commandData;
			CommandFactoryReference command;

			switch (key)
			{
				case '\n':
				case '\r':
					return;

				default:
					command = new CommandFactoryReference(
						InsertCharacterCommandFactory.Key, key);
					break;
			}

			// Execute the command requested.
			commandFactory.Do(context, command);
		}

		#endregion

		#region Constructors

		static InsertKeyCommandFactory()
		{
			Key = new HierarchicalPath(
				"/Text Editor/Insert Key",HierarchicalPathOptions.InternStrings);
		}

		#endregion
	}
}
