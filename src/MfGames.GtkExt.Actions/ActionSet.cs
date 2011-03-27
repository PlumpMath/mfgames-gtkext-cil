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
using System.Diagnostics;

using Gtk;

#endregion

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Implements the same functionality of <see cref="ActionGroup"/>. This was
	/// created because the sensitivity wasn't changing for actions.
	/// </summary>
	public class ActionSet
	{
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

		#region Properties

		private bool sensitive;
		private bool visible;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ActionSet"/> is sensitive.
		/// </summary>
		/// <value>
		///   <c>true</c> if sensitive; otherwise, <c>false</c>.
		/// </value>
		public bool Sensitive
		{
			[DebuggerStepThrough]
			get { return sensitive; }
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
			[DebuggerStepThrough]
			get { return visible; }
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

		#region Collection

		private readonly List<Action> actions;
		private readonly List<Widget> widgets;

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
	}
}