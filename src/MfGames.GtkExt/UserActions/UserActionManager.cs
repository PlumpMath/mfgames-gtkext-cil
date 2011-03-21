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
using System.Reflection;

using Gtk;

#endregion

namespace MfGames.GtkExt.UserActions
{
	/// <summary>
	/// Handles gathering and managing user actions. It also is used to populate
	/// various widget containers, such as menus and toolbars.
	/// </summary>
	public class UserActionManager
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionManager"/> class.
		/// </summary>
		public UserActionManager()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionManager"/> class
		/// and populates it with the actions given.
		/// </summary>
		/// <param name="userActions">The user actions.</param>
		public UserActionManager(IEnumerable<UserAction> userActions)
		{
		}

		#endregion

		#region Scanning

		/// <summary>
		/// Scans through the given assembly and looks for anything that 
		/// is marked with the <see cref="UserActionAttribute"/>.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		public void Add(Assembly assembly)
		{
		}

		/// <summary>
		/// Adds the specified user action to the manager. If the action's
		/// ConfigurationPath already exists, this will throw an exception.
		/// </summary>
		/// <param name="userAction">The user action.</param>
		public void Add(UserAction userAction)
		{
		}

		/// <summary>
		/// Adds a collection of <see cref="UserAction"/> to the manager.
		/// </summary>
		/// <param name="userActions">The user actions.</param>
		public void Add(IEnumerable<UserAction> userActions)
		{
		}

		/// <summary>
		/// Removes the specified configuration path from the manager.
		/// </summary>
		/// <param name="configurationPath">The configuration path.</param>
		public void Remove(HierarchicalPath configurationPath)
		{
		}

		#endregion

		#region Widgets

		/// <summary>
		/// Initializes the specified menu bar. If the menu bar already has
		/// items, they are removed before new elements are included.
		/// </summary>
		/// <param name="rootPath">The root path for the widget.</param>
		/// <param name="menuBar">The menu bar to populate.</param>
		public void Initialize(
			HierarchicalPath rootPath,
			MenuBar menuBar)
		{
		}

		#endregion
	}
}