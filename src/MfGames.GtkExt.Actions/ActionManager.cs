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

using Gtk;

using MfGames.GtkExt.Actions.Layouts;
using MfGames.Reporting;

using Action=Gtk.Action;

#endregion

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Wraps around the various <see cref="Gtk.Action"/> and <see cref="ActionGroup"/>
	/// functionality to manage all actions within an application.
	/// </summary>
	public class ActionManager
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionManager"/> class.
		/// </summary>
		/// <param name="accelGroup">The accelerator group to attach to.</param>
		public ActionManager(AccelGroup accelGroup)
		{
			// Save the parameters.
			if (accelGroup == null)
			{
				throw new ArgumentNullException("accelGroup");
			}

			AccelGroup = accelGroup;

			// Create the collections we use.
			messages = new SeverityMessageCollection();
			actions = new Dictionary<string, Action>();
			groups = new Dictionary<string, ActionGroup>();
			layouts = new ActionLayoutCollection(this);
		}

		#endregion

		#region Gtk#

		private readonly SeverityMessageCollection messages;

		/// <summary>
		/// Gets the accelerator group associated with this manager.
		/// </summary>
		/// <value>The accelerator group.</value>
		public AccelGroup AccelGroup { get; private set; }

		#endregion

		#region Actions

		private readonly Dictionary<string, Action> actions;

		/// <summary>
		/// Adds a single action into the manager, automatically connecting
		/// accelerators and groups.
		/// </summary>
		/// <param name="newAction">The action to add.</param>
		public void Add(Action newAction)
		{
			// Make sure we have sane data.
			if (newAction == null)
			{
				throw new ArgumentNullException("action");
			}

			// Check to see if we are going to filter out this action.
			if (!CanAdd(newAction))
			{
				return;
			}

			// Associate the name of the action in the current dictionary. If the
			// name is already registered, then add a message and ignore the
			// request.
			string actionName = newAction.Name;

			if (actions.ContainsKey(actionName))
			{
				// Create a new error message.
				var message = new SeverityMessage(
					Severity.Error,
					"Cannot register " + actionName +
					" action because it was already registered previously.");
				messages.Add(message);

				return;
			}

			actions[actionName] = newAction;

			// Figure out what action group this action is associated with.
			const string groupName = "Global";

			ActionGroup group = GetOrCreateGroup(groupName);

			newAction.ActionGroup = group;

			// Assign the accelerator of the entire manager.
			newAction.AccelGroup = AccelGroup;
		}

		/// <summary>
		/// Adds a collection of Action objects into the manager.
		/// </summary>
		/// <param name="newActions">The actions to add.</param>
		public void Add(IEnumerable<Action> newActions)
		{
			foreach (Action action in newActions)
			{
				Add(action);
			}
		}

		/// <summary>
		/// Scans the specified assembly and looks for all constructible actions
		/// with zero parameter constructors. For every one, it creates a new
		/// instance and adds it to the manager.
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
			Type actionType = typeof(Action);

			foreach (Type type in assembly.GetTypes())
			{
				// Make sure the type is a concrete instance.
				if (!type.IsClass || type.IsAbstract)
				{
					// We can't create this type.
					continue;
				}

				// If we aren't an Action type, then just continue.
				if (!actionType.IsAssignableFrom(type))
				{
					continue;
				}

				// Make sure we can add it.
				if (!CanAdd(type))
				{
					continue;
				}

				// Determine if we have a parameterless constructor.
				ConstructorInfo constructor = type.GetConstructor(emptyTypes);

				if (constructor == null)
				{
					// Add a message to report this.
					var message = new SeverityMessage(
						Severity.Alert,
						"Cannot create an instance of " + type +
						" because it does not have a parameterless constructor.");
					messages.Add(message);

					// We are done with this type.
					continue;
				}

				// Create the item and add it to the manager.
				var action = (Action) constructor.Invoke(emptyObjects);

				Add(action);
			}
		}

		/// <summary>
		/// Determines whether this manager will allow the specified action to be
		/// added. This is checked before everything except the <see langword="null"/>
		/// check.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can add the specified action; otherwise, 
		/// 	<c>false</c>.
		/// </returns>
		protected virtual bool CanAdd(Action action)
		{
			return true;
		}

		/// <summary>
		/// Determines whether this manager will allow the specified type to be added
		/// to the manager either as an <see cref="Action"/> or 
		/// <see cref="IActionFactory"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can add the specified type; otherwise, 
		/// 	<c>false</c>.
		/// </returns>
		protected virtual bool CanAdd(Type type)
		{
			return true;
		}

		/// <summary>
		/// Gets the action from the given name.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		/// <returns></returns>
		public Action GetAction(string actionName)
		{
			return actions[actionName];
		}

		#endregion

		#region Action Groups

		private readonly Dictionary<string, ActionGroup> groups;

		/// <summary>
		/// Gets or creates an <see cref="ActionGroup"/> of the given name.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		/// <returns></returns>
		private ActionGroup GetOrCreateGroup(string groupName)
		{
			// Make sure we have sane values.
			if (String.IsNullOrEmpty(groupName))
			{
				throw new ArgumentException(
					"Cannot create an ActionGroup with an empty or null name.", groupName);
			}

			// Look to see if the group doesn't exist.
			if (!groups.ContainsKey(groupName))
			{
				groups[groupName] = new ActionGroup(groupName);
			}

			// Return the resulting group.
			return groups[groupName];
		}

		#endregion

		#region Action Layouts

		private readonly ActionLayoutCollection layouts;

		/// <summary>
		/// Adds the specified layout to the manager.
		/// </summary>
		/// <param name="layout">The layout.</param>
		public void Add(ActionLayout layout)
		{
			if (layout == null)
			{
				throw new ArgumentNullException("layout");
			}

			layouts.Add(layout);
		}

		#endregion
	}
}