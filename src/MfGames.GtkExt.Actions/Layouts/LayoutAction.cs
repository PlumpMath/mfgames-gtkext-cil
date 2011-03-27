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

using System.Xml;

using Gtk;

using MfGames.GtkExt.Actions.Widgets;

#endregion

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// Represents a single item in the layout.
	/// </summary>
	public class LayoutAction : ILayoutListItem
	{
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

		#region Properties

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string ActionName { get; set; }

		#endregion

		#region Population

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

			// Create a menu item from this action.
			Widget widget = new ActionMenuItem(manager, action);
			
			shell.Add(widget);
		}

		#endregion
	}
}