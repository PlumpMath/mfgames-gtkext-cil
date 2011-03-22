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

using System;
using System.Collections.Generic;
using System.Reflection;

using Action=Gtk.Action;

#endregion

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Handles gathering and managing user actions. It also is used to populate
	/// various widget containers, such as menus and toolbars.
	/// This is organized that a class could extend one of the interfaces and
	/// automatically gather up actions using an IoC manager.
	/// </summary>
	public class UserActionManager
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionManager"/> class.
		/// </summary>
		public UserActionManager()
		{
			entries = new Dictionary<HierarchicalPath, Entry>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionManager"/> class
		/// and populates it with the actions given.
		/// </summary>
		/// <param name="userActions">The user actions.</param>
		public UserActionManager(IEnumerable<IUserAction> userActions)
			: this()
		{
			Add(userActions);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionManager"/> class
		/// and populates it with the actions from the fixtures.
		/// </summary>
		/// <param name="userActionFixtures">The user action fixtures.</param>
		public UserActionManager(IEnumerable<IUserActionFixture> userActionFixtures)
			: this()
		{
			Add(userActionFixtures);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionManager"/> class
		/// and populates it with the given user actions and action fixtures.
		/// </summary>
		/// <param name="userActions">The user actions.</param>
		/// <param name="userActionFixtures">The user action fixtures.</param>
		public UserActionManager(
			IEnumerable<IUserAction> userActions,
			IEnumerable<IUserActionFixture> userActionFixtures)
			: this()
		{
			Add(userActions);
			Add(userActionFixtures);
		}

		#endregion

		#region Entries

		private readonly Dictionary<HierarchicalPath, Entry> entries;

		/// <summary>
		/// Scans through the given assembly and looks for anything that  is
		/// implements the <see cref="IUserAction"/> or 
		/// <see cref="IUserActionFixture"/> interfaces.
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
		public void Add(IUserAction userAction)
		{
			// We don't allow null actions to be included.
			if (userAction == null)
			{
				throw new ArgumentNullException("userAction");
			}

			// Make sure the path exists and isn't already registered.
			HierarchicalPath path = userAction.ConfigurationPath;

			if (path == null)
			{
				throw new InvalidOperationException(
					"Given action has a null ConfigurationPath: " + userAction + ".");
			}

			if (entries.ContainsKey(path))
			{
				throw new InvalidOperationException(
					"There already exists an entry with the ConfigurationPath of " + path + ".");
			}

			// Add the path into the dictionary.
			entries[path] = new Entry(userAction);
		}

		/// <summary>
		/// Adds all the user action inside the fixture to the manager.
		/// </summary>
		public void Add(IUserActionFixture userActionFixture)
		{
			if (userActionFixture == null)
			{
				throw new ArgumentNullException("userActionFixture");
			}

			Add(userActionFixture.CreateUserActions());
		}

		/// <summary>
		/// Adds a collection of <see cref="IUserAction"/> to the manager.
		/// </summary>
		/// <param name="userActions">The user actions.</param>
		public void Add(IEnumerable<IUserAction> userActions)
		{
			foreach (IUserAction userAction in userActions)
			{
				Add(userAction);
			}
		}

		/// <summary>
		/// Adds a collection of <see cref="IUserActionFixture"/> to the manager.
		/// </summary>
		/// <param name="userActionFixtures">The user action fixtures.</param>
		public void Add(IEnumerable<IUserActionFixture> userActionFixtures)
		{
			foreach (IUserActionFixture userActionFixture in userActionFixtures)
			{
				Add(userActionFixture);
			}
		}

		/// <summary>
		/// Removes the specified configuration path from the manager.
		/// </summary>
		/// <param name="configurationPath">The configuration path.</param>
		public void Remove(HierarchicalPath configurationPath)
		{
			entries.Remove(configurationPath);
		}

		#endregion

		#region Key Bindings

		#endregion

		#region Layout

		#endregion

		#region Widgets

		#endregion

		#region Nested type: Entry

		/// <summary>
		/// Represents an user action and the associated data.
		/// </summary>
		private struct Entry
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="Entry"/> struct.
			/// </summary>
			/// <param name="userAction">The user action.</param>
			public Entry(IUserAction userAction)
			{
				UserAction = userAction;
				Action = null;
			}

			#endregion

			#region Properties

			public Action Action;
			public IUserAction UserAction;

			#endregion
		}

		#endregion
	}
}