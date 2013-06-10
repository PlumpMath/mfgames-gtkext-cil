// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Gtk;
using MfGames.GtkExt.Actions;

namespace GtkExtDemo
{
	/// <summary>
	/// Contains the various functionality for showing off the actions and
	/// keybindings.
	/// </summary>
	public class DemoActions: DemoTab
	{
		#region Methods

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
			ActionSet group = actionManager.GetGroup(groupName);
			group.Sensitive = enable;
		}

		#endregion

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

		#region Fields

		private readonly ActionManager actionManager;

		#endregion
	}
}
