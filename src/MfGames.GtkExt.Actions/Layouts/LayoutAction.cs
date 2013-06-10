// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Xml;
using Gtk;
using MfGames.GtkExt.Actions.Widgets;
using MfGames.Reporting;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Represents a single item in the layout.
	/// </summary>
	public class LayoutAction: ILayoutListItem
	{
		#region Properties

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string ActionName { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Populates the specified shell with sub-menus.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="shell">The shell.</param>
		public void Populate(
			ActionManager manager,
			MenuShell shell)
		{
			// Get the action associated with this.
			Action action = manager.GetAction(ActionName);
			MenuItem menuItem;

			if (action == null)
			{
				// Create a placeholder menu item.
				menuItem = new MenuItem("<Unknown Action: " + ActionName + ">");
				menuItem.Sensitive = false;

				// Add it to the errors.
				manager.Messages.Add(
					new SeverityMessage(
						Severity.Error, "Could not find action " + ActionName + " to add to menu."));
			}
			else
			{
				// Create a menu item from this action.
				menuItem = new ActionMenuItem(manager, action);
			}

			// Add the resulting menu item to the list.
			shell.Add(menuItem);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LayoutAction"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public LayoutAction(XmlReader reader)
		{
			ActionName = reader["name"];
		}

		#endregion
	}
}
