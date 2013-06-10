// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using GLib;
using Gtk;
using MfGames.HierarchicalPaths;
using MfGames.Reporting;
using Action = Gtk.Action;

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Wraps around the various <see cref="Gtk.Action"/> and <see cref="ActionGroup"/>
	/// functionality to manage all actions within an application.
	/// </summary>
	public class ActionManager
	{
		#region Properties

		/// <summary>
		/// Gets or sets the layout.
		/// </summary>
		/// <value>
		/// The layout.
		/// </value>
		public ActionLayout Layout
		{
			[DebuggerStepThrough] get { return layout; }
		}

		/// <summary>
		/// Gets the collection of messages produced by the action manager.
		/// </summary>
		/// <value>The messages.</value>
		public SeverityMessageCollection Messages
		{
			get { return messages; }
		}

		#endregion

		#region Methods

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
					"Cannot register " + actionName
						+ " action because it was already registered previously.");
				messages.Add(message);

				return;
			}

			actions[actionName] = newAction;

			// Figure out what action group this action is associated with.
			string groupName = "Global";

			if (newAction is IConfigurableAction)
			{
				groupName = ((IConfigurableAction) newAction).GroupName;
			}

			ActionSet group = GetOrCreateGroup(groupName);

			group.Add(newAction);
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
		/// Adds the actions from the specified factory.
		/// </summary>
		/// <param name="actionFactory">The action factory.</param>
		public void Add(IActionFactory actionFactory)
		{
			if (actionFactory == null)
			{
				throw new ArgumentNullException("actionFactory");
			}

			ICollection<Action> newActions = actionFactory.CreateActions();

			Add(newActions);
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
			var emptyTypes = new Type[]
			{
			};
			var emptyObjects = new object[]
			{
			};
			Type actionType = typeof (Action);

			foreach (Type type in assembly.GetTypes())
			{
				// Make sure the type is a concrete instance.
				if (!type.IsClass
					|| type.IsAbstract)
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
						"Cannot create an instance of " + type
							+ " because it does not have a parameterless constructor.");
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
		/// Attaches to the root window and installs the accelerator processing.
		/// </summary>
		/// <param name="window"></param>
		public void AttachToRootWindow(Window window)
		{
			// Save the parameters.
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}

			// Install the accelerator group.
			//window.AddAccelGroup(AccelGroup);

			// Attach to the window normally.
			AttachToWindow(window);
		}

		/// <summary>
		/// Connects various events to the widget for processing key strokes.
		/// </summary>
		/// <param name="window">The window.</param>
		public void AttachToWindow(Window window)
		{
			// See if we already have attached to the window.
			if (attachedWindows.Contains(window))
			{
				// Nothing new to do.
				return;
			}

			// Add it to the dictionary to keep track of it.
			attachedWindows.Add(window);

			// Attach to events for processing.
			window.KeyPressEvent += OnKeyPrePressed;
			window.KeyPressEvent += OnKeyPressed;
			window.Destroyed += OnDestroyed;
		}

		/// <summary>
		/// Gets the action from the given name.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		/// <returns>
		/// The action, if found, or otherwise <see langword="null"/>.
		/// </returns>
		public Action GetAction(string actionName)
		{
			return actions.ContainsKey(actionName)
				? actions[actionName]
				: null;
		}

		/// <summary>
		/// Gets the group from the given name.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		/// <returns></returns>
		public ActionSet GetGroup(string groupName)
		{
			// Make sure we have sane values.
			if (String.IsNullOrEmpty(groupName))
			{
				throw new ArgumentException(
					"Cannot retrieve a group with an empty or null name.", groupName);
			}

			// Return the resulting group.
			return groups[groupName];
		}

		/// <summary>
		/// Gets or creates an <see cref="ActionGroup"/> of the given name.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		/// <returns></returns>
		public ActionSet GetOrCreateGroup(string groupName)
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
				groups[groupName] = new ActionSet();
			}

			// Return the resulting group.
			return groups[groupName];
		}

		/// <summary>
		/// Gets the primary accelerator path for a given action or returns 
		/// <see langword="null"/> if one can't be found.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public HierarchicalPath GetPrimaryAcceleratorPath(Action action)
		{
			// If we don't have a current keybindings, then return null.
			if (keybindings == null)
			{
				return null;
			}

			// Pass it into the keybindings.
			return keybindings.GetPrimaryAcceleratorPath(action);
		}

		/// <summary>
		/// Sets the current keybindings.
		/// </summary>
		/// <param name="newKeybindings">The new keybindings.</param>
		public void SetKeybindings(ActionKeybindings newKeybindings)
		{
			// Clear out the old keybindings.
			if (keybindings != null)
			{
				keybindings.ActionManager = null;
			}

			// Set the current key binding.
			keybindings = newKeybindings;

			// Set up the new keybindings.
			if (keybindings != null)
			{
				keybindings.ActionManager = this;
			}
		}

		/// <summary>
		/// Sets the action layout.
		/// </summary>
		/// <param name="newLayout">The new layout.</param>
		public void SetLayout(ActionLayout newLayout)
		{
			// Clear out the old layout
			if (layout != null)
			{
				layout.ActionManager = null;
			}

			// Save the new layout.
			layout = newLayout;

			// Clear out the new layout
			if (layout != null)
			{
				layout.ActionManager = this;
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
		/// Called when an attached window is destroyed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnDestroyed(
			object sender,
			EventArgs e)
		{
			// Cast the sender as a window since it is the only source we'll get.
			var window = (Window) sender;

			// Disconnect from the events.
			window.Destroyed -= OnDestroyed;
			window.KeyPressEvent -= OnKeyPressed;
			window.KeyPressEvent -= OnKeyPrePressed;

			// Remove it from our dictionary.
			attachedWindows.Remove(window);
		}

		/// <summary>
		/// Called when when a key is pressed but before it is managed by
		/// the individual widgets.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Gtk.KeyPressEventArgs"/> instance containing the event data.</param>
		[ConnectBefore]
		private void OnKeyPrePressed(
			object sender,
			KeyPressEventArgs e)
		{
			// If we have a current key binding, then pass it on.
			if (keybindings != null)
			{
				keybindings.KeyPrePressed(e);
			}
		}

		/// <summary>
		/// Called when a key is pressed and is used to handle custom keybindings.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Gtk.KeyPressEventArgs"/> instance containing the event data.</param>
		private void OnKeyPressed(
			object sender,
			KeyPressEventArgs e)
		{
			// If we have a current key binding, then pass it on.
			if (keybindings != null)
			{
				keybindings.KeyPressed(e);
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionManager"/> class.
		/// </summary>
		public ActionManager()
		{
			// Create the collections we use.
			messages = new SeverityMessageCollection();
			actions = new Dictionary<string, Action>();
			groups = new Dictionary<string, ActionSet>();
			attachedWindows = new HashSet<Window>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionManager"/> class.
		/// </summary>
		/// <param name="attachToWindow">The widget to attach processing to.</param>
		public ActionManager(Window attachToWindow)
			: this()
		{
			// Attach to the widget.
			AttachToRootWindow(attachToWindow);
		}

		#endregion

		#region Fields

		private readonly Dictionary<string, Action> actions;
		private readonly HashSet<Window> attachedWindows;
		private readonly Dictionary<string, ActionSet> groups;

		private ActionKeybindings keybindings;

		private ActionLayout layout;
		private readonly SeverityMessageCollection messages;

		#endregion
	}
}
