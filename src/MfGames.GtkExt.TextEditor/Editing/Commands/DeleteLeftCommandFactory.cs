// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MfGames.Commands;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.TextEditor.Editing.Commands
{
	/// <summary>
	/// Implements the command factory for handling the "delete left" (backspace).
	/// </summary>
	public class DeleteLeftCommandFactory: ICommandFactory
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
			CommandReference commandReference)
		{
			// Ensure the code contracts for this state.
			Contract.Requires<ArgumentNullException>(context != null);
			Contract.Requires<InvalidCastException>(context is EditorView);
			Contract.Requires<ArgumentNullException>(commandReference != null);
		}

		public string GetDisplayName(CommandReference commandReference)
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
