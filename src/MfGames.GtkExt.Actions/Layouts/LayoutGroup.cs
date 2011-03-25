#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

#region Namespaces

using System.Collections.Generic;
using System.Xml;

using Gtk;

#endregion

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Contains the top-level element in a <see cref="ActionLayout"/>.
	/// </summary>
	public class LayoutGroup : List<LayoutList>
	{
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

		#region Properties

		/// <summary>
		/// Contains the ID to identify the group.
		/// </summary>
		public string Id { get; private set; }

		#endregion

		#region Loading

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
				if (reader.NodeType == XmlNodeType.EndElement &&
				    reader.LocalName == "layout-group")
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

		#region Population

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
			foreach (LayoutList list in this)
			{
				list.Populate(manager, menubar);
			}
		}

		#endregion

		#region Conversion

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

		#endregion
	}
}