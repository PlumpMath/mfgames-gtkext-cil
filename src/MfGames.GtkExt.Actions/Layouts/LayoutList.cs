// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Xml;
using Gtk;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Implements a list of layout items which can include other layout lists
	/// and actions.
	/// </summary>
	public class LayoutList: List<ILayoutListItem>,
		ILayoutListItem,
		ILayoutGroupItem
	{
		#region Properties

		/// <summary>
		/// Gets or sets the name of the action group associated with this list.
		/// </summary>
		/// <value>The name of the group.</value>
		public string GroupName { get; set; }

		/// <summary>
		/// Gets or sets the label of the list which is used in the menus.
		/// </summary>
		/// <value>The label.</value>
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the list would be right
		/// aligned in the menu bar.
		/// </summary>
		/// <value>
		///   <c>true</c> if [right aligned]; otherwise, <c>false</c>.
		/// </value>
		public bool RightAligned { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Populates the specified shell with sub-menus.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="menuBar">The menu bar.</param>
		public void Populate(
			ActionManager manager,
			MenuBar menuBar)
		{
			Populate(manager, (MenuShell) menuBar);
		}

		/// <summary>
		/// Populates the specified shell with sub-menus.
		/// </summary>
		/// <param name="shell">The shell.</param>
		public void Populate(
			ActionManager manager,
			MenuShell shell)
		{
			// Create a new submenu for ourselves.
			var menu = new Menu();

			var menuItem = new MenuItem(Label);
			menuItem.Submenu = menu;
			menuItem.RightJustified = RightAligned;

			// If we have a group name, add it to the list.
			if (!String.IsNullOrEmpty(GroupName))
			{
				ActionSet group = manager.GetOrCreateGroup(GroupName);

				group.Add(menuItem);
			}

			// Attach our menu to the shell.
			shell.Append(menuItem);

			// Go through all of our elements and add them to our menu.
			foreach (ILayoutListItem item in this)
			{
				// Go through and populate the menu itself.
				item.Populate(manager, menu);
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format(
				"LayoutList ({0}, {1}, Items {2})", Label, GroupName ?? "<No Group>", Count);
		}

		/// <summary>
		/// Loads the layout list from the given reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void Load(XmlReader reader)
		{
			// Pull out the group's information.
			Label = reader["label"];
			GroupName = reader["group"];

			if (!String.IsNullOrEmpty(reader["right-aligned"]))
			{
				RightAligned = Convert.ToBoolean(reader["right-aligned"]);
			}

			// Loop through until we find the end of the group.
			while (reader.Read())
			{
				// Ignore everything outside of our namespace.
				if (reader.NamespaceURI != ActionLayout.ActionLayoutNamespace)
				{
					continue;
				}

				// Check for the end of the layout tag.
				if (reader.NodeType == XmlNodeType.EndElement
					&& reader.LocalName == "layout-list")
				{
					// We are done reading.
					return;
				}

				// Load the list as we get them.
				switch (reader.LocalName)
				{
					case "action":
						var action = new LayoutAction(reader);
						Add(action);
						break;

					case "separator":
						var separator = new LayoutSeparator();
						Add(separator);
						break;
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LayoutList"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public LayoutList(XmlReader reader)
		{
			Load(reader);
		}

		#endregion
	}
}
