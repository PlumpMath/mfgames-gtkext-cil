// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using System.Xml;
using Gtk;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Contains the top-level element in a <see cref="ActionLayout"/>.
	/// </summary>
	public class LayoutGroup: List<ILayoutGroupItem>
	{
		#region Properties

		/// <summary>
		/// Contains the ID to identify the group.
		/// </summary>
		public string Id { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Populates the specified menu bar with components from this list.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="menubar">The menu bar.</param>
		public void Populate(
			ActionManager manager,
			MenuBar menubar)
		{
			// Go through all the lists in this group.
			foreach (ILayoutGroupItem list in this)
			{
				list.Populate(manager, menubar);
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
			return "Layout Group (" + Id + ", Lists " + Count + ")";
		}

		/// <summary>
		/// Loads the layout group from the given reader. This assumes that the
		/// reader is positioned on the group's tag.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void Load(XmlReader reader)
		{
			// Pull out the group's information.
			Id = reader["id"];

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
					&& reader.LocalName == "layout-group")
				{
					// We are done reading.
					return;
				}

				// Load the list as we get them.
				if (reader.LocalName == "layout-list")
				{
					var list = new LayoutList(reader);
					Add(list);
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LayoutGroup"/> class
		/// and populates it from the given reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public LayoutGroup(XmlReader reader)
		{
			Load(reader);
		}

		#endregion
	}
}
