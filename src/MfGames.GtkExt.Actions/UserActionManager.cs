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
using System.IO;
using System.Reflection;
using System.Xml;

using Gtk;

using MfGames.GtkExt.Actions.Layouts;

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
			entries = new Dictionary<HierarchicalPath, UserActionEntry>();
			layouts = new Dictionary<string, LayoutList>();
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

		private readonly Dictionary<HierarchicalPath, UserActionEntry> entries;

		/// <summary>
		/// Scans through the given assembly and looks for anything that  is
		/// implements the <see cref="IUserAction"/> or 
		/// <see cref="IUserActionFixture"/> interfaces.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		public void Add(Assembly assembly)
		{
			// Make sure we don't have a null since we can't handle that.
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			// Scan through the types of the assembly.
			var emptyTypes = new Type[] { };
			var emptyObjects = new object[] { };
			Type userActionType = typeof(IUserAction);
			Type userActionFixtureType = typeof(IUserActionFixture);

			foreach (Type type in assembly.GetTypes())
			{
				// Make sure the type is a concrete instance.
				if (!type.IsClass || type.IsAbstract)
				{
					// We can't create this type.
					continue;
				}

				// Determine if we have a parameterless constructor.
				ConstructorInfo constructor = type.GetConstructor(emptyTypes);

				// If we implement the IUserAction, add it directly.
				object results = null;

				if (userActionType.IsAssignableFrom(type))
				{
					// Check the constructor.
					if (constructor == null)
					{
						// We don't add it because there might be another reason
						// this isn't included.
						continue;
					}

					// Create the item and add it.
					var userAction = (IUserAction) constructor.Invoke(emptyObjects);

					results = userAction;
					Add(userAction);
				}

				// If we implement the IUserActionFixture, add it directly.
				if (userActionFixtureType.IsAssignableFrom(type))
				{
					// Check the constructor.
					if (constructor == null)
					{
						throw new Exception(
							"Cannot create IUserActionFixture type " + type +
							" because it doesn't have a parameterless constructor.");
					}

					// Create the item and add it. If we already did (it could
					// happen), then just reuse the same object.
					IUserActionFixture userActionFixture;

					if (results == null)
					{
						userActionFixture = (IUserActionFixture) constructor.Invoke(emptyObjects);
					}
					else
					{
						userActionFixture = (IUserActionFixture) results;
					}

					Add(userActionFixture);

					continue;
				}
			}
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
			entries[path] = new UserActionEntry(userAction);
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

		private readonly Dictionary<string, LayoutList> layouts;

		/// <summary>
		/// Loads a layout file into memory, replacing any elements that
		/// exist already.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void LoadFileLayout(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}

			LoadFileLayout(new FileInfo(filename));
		}

		/// <summary>
		/// Loads a layout file into memory, replacing any elements that
		/// exist already.
		/// </summary>
		/// <param name="file">The file.</param>
		public void LoadFileLayout(FileInfo file)
		{
			// Make sure the file is usable.
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}

			if (!file.Exists)
			{
				throw new FileNotFoundException("Cannot find layout file", file.FullName);
			}

			// Get an XML reader and use that.
			using (
				FileStream stream = file.Open(
					FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				LoadLayout(stream);
			}
		}

		/// <summary>
		/// Loads a layout from the stream, replacing any keys found.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void LoadLayout(Stream stream)
		{
			using (XmlReader reader = XmlReader.Create(stream))
			{
				LoadLayout(reader);
			}
		}

		/// <summary>
		/// Loads a layout from the XML reader, replacing any keys found.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public void LoadLayout(XmlReader reader)
		{
			// Keep track of a stack of list items.
			var lists = new LinkedList<LayoutList>();
			LayoutList currentList = null;

			// Loop through the layout and add the items.
			while (reader.Read())
			{
				// Check for opening elements.
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.LocalName)
					{
						case "List":
							if (reader.IsEmptyElement)
							{
								break;
							}
							
							// Create a new list and add it.
							var newList = new LayoutList(reader);
							lists.AddLast(newList);

							// If we are a top-level list, we need to keep it
							// so calling programs can refer to it.
							if (currentList == null)
							{
								layouts[newList.Id] = newList;
							}
							else
							{
								currentList.Add(newList);
							}

							currentList = newList;

							break;

						case "Action":
							if (currentList == null)
							{
								throw new Exception("Could not find <List> element in XML input.");
							}

							var action = new LayoutAction(reader);
							currentList.Add(action);
							break;
					}
				}

				// Check for closing elements.
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName == "List")
					{
						// Pop the last list item off.
						lists.RemoveLast();

						// Change the list to the new current one.
						currentList = lists.Count == 0 ? null : lists.Last.Value;
					}
				}
			}
		}

		#endregion

		#region Localization

		/// <summary>
		/// Gets the localized label.
		/// </summary>
		/// <param name="label">The label.</param>
		/// <returns></returns>
		protected virtual string GetLocalizedLabel(string label)
		{
			return label;
		}

		/// <summary>
		/// Gets the localized text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		protected virtual string GetLocalizedText(string text)
		{
			return text;
		}

		#endregion

		#region Widgets

		/// <summary>
		/// Populates the specified menu bar with a given layout.
		/// </summary>
		/// <param name="menubar">The menu bar.</param>
		/// <param name="layoutId">The layout ID.</param>
		public void Populate(MenuBar menubar, string layoutId)
		{
			// Clear the menubar of its current elements.

			// Get the layout with the given name.
			LayoutList layout = layouts[layoutId];

			// Add the items in the list.
			Populate(menubar, layout);
		}

		private void Populate(MenuBar menubar, LayoutList layout)
		{
			// Go through the list and pull out the layouts.
			foreach (LayoutList menuLayout in layout)
			{
				// Create a new menu for this item.
				var menu = new Menu();

				var menuItem = new MenuItem(menuLayout.Label);
				menuItem.Submenu = menu;

				menubar.Append(menuItem);

				// Go through and populate the menu itself.
				Populate(menu, menuLayout);
			}
		}

		private void Populate(Menu menu, LayoutList layout)
		{
			// Go through all the items in the menu.
			foreach (ILayoutItem item in layout)
			{
				// Check the type of item.
				if (item is LayoutAction)
				{
					// Pull out the action we are processing.
					var action = (LayoutAction) item;
					UserActionEntry entry = entries[action.ConfigurationPath];
					IUserAction userAction = entry.UserAction;
					
					// If the menu has an icon or is stock, then we use that.
					MenuItem menuItem;

					if (String.IsNullOrEmpty(userAction.StockId))
					{
						menuItem = new ImageMenuItem(userAction.Label);
					}
					else
					{
						menuItem = new ImageMenuItem(userAction.StockId, new AccelGroup());
					}

					// Add the menu item to the list.
					menu.Append(menuItem);
				}
			}
		}

		#endregion
	}
}