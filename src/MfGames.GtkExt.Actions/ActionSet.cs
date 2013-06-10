// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using System.Diagnostics;
using Gtk;

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Implements the same functionality of <see cref="ActionGroup"/>. This was
	/// created because the sensitivity wasn't changing for actions.
	/// </summary>
	public class ActionSet
	{
		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ActionSet"/> is sensitive.
		/// </summary>
		/// <value>
		///   <c>true</c> if sensitive; otherwise, <c>false</c>.
		/// </value>
		public bool Sensitive
		{
			[DebuggerStepThrough] get { return sensitive; }
			set
			{
				// Set the new value for the sensitive.
				sensitive = value;

				// Go through all the items and set their sensitivity.
				foreach (Action action in actions)
				{
					action.Sensitive = value;
				}

				foreach (Widget widget in widgets)
				{
					widget.Sensitive = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ActionSet"/> is visible.
		/// </summary>
		/// <value>
		///   <c>true</c> if visible; otherwise, <c>false</c>.
		/// </value>
		public bool Visible
		{
			[DebuggerStepThrough] get { return visible; }
			set
			{
				// Set the new value for the sensitive.
				visible = value;

				// Go through all the items and set their sensitivity.
				foreach (Action action in actions)
				{
					action.Visible = value;
				}

				foreach (Widget widget in widgets)
				{
					widget.Visible = value;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the specified action to the set.
		/// </summary>
		/// <param name="action">The action.</param>
		public void Add(Action action)
		{
			action.Sensitive = sensitive;
			action.Visible = visible;

			actions.Add(action);
		}

		/// <summary>
		/// Adds the specified widget to the set.
		/// </summary>
		/// <param name="widget">The widget.</param>
		public void Add(Widget widget)
		{
			widget.Sensitive = sensitive;
			widget.Visible = visible;

			widgets.Add(widget);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionSet"/> class.
		/// </summary>
		public ActionSet()
		{
			sensitive = true;
			visible = true;

			actions = new List<Action>();
			widgets = new List<Widget>();
		}

		#endregion

		#region Fields

		private readonly List<Action> actions;

		private bool sensitive;
		private bool visible;
		private readonly List<Widget> widgets;

		#endregion
	}
}
