// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Xml;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.Actions.Keybindings
{
	/// <summary>
	/// Represents a single keybinding between an action and an accelerator
	/// chain. Chains are represented by 
	/// </summary>
	public class ActionKeybinding
	{
		#region Properties

		/// <summary>
		/// Contains the accelerator path for the action.
		/// </summary>
		public HierarchicalPath AcceleratorPath { get; private set; }

		/// <summary>
		/// Gets the name of the action.
		/// </summary>
		/// <value>
		/// The name of the action.
		/// </value>
		public string ActionName { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionKeybinding"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public ActionKeybinding(XmlReader reader)
		{
			ActionName = reader["name"];
			AcceleratorPath = new HierarchicalPath(reader["keybinding"]);
		}

		#endregion
	}
}
