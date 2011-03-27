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

using Gtk;

using MfGames.GtkExt.Actions;

#endregion

namespace GtkExtDemo
{
	/// <summary>
	/// Contains the various functionality for showing off the actions and
	/// keybindings.
	/// </summary>
	public class DemoActions : DemoTab
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoActions"/> class.
		/// </summary>
		public DemoActions(ActionManager actionManager)
		{
			this.actionManager = actionManager;

			// Create a frame with the enable elements.
			var enableFrame = new Frame("Action Group Sensitivity");
			var enableBox = new VBox();

			enableBox.BorderWidth = 5;

			enableFrame.BorderWidth = 5;
			enableFrame.Add(enableBox);

			// Add the check buttons.
			var checkButton = new CheckButton("Enable _Global entries");
			checkButton.Name = "Global";
			checkButton.Active = true;
			checkButton.Toggled += OnGroupSensitivityChanged;
			enableBox.PackStart(checkButton);

			checkButton = new CheckButton("Enable _View entries");
			checkButton.Name = "View";
			checkButton.Active = true;
			checkButton.Toggled += OnGroupSensitivityChanged;
			enableBox.PackStart(checkButton);

			checkButton = new CheckButton("Enable _Editor entries");
			checkButton.Name = "Editor";
			checkButton.Active = true;
			checkButton.Toggled += OnGroupSensitivityChanged;
			enableBox.PackStart(checkButton);

			// Return it
			PackStart(enableFrame, false, false, 0);
			PackStart(new Label(""), true, true, 0);
		}

		#endregion

		#region Properties

		private readonly ActionManager actionManager;

		#endregion

		#region Events

		/// <summary>
		/// Called when the group sensitivity check button is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnGroupSensitivityChanged(
			object sender,
			EventArgs e)
		{
			// Get the name of the group we are enabling or disabling.
			var button = (CheckButton) sender;
			string groupName = button.Name;
			bool enable = button.Active;

			// Change the enabled state.
			var group = actionManager.GetGroup(groupName);
			group.Sensitive = enable;
		}

		#endregion
	}
}