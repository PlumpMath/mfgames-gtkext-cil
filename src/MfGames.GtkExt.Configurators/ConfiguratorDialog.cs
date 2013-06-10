// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Gdk;
using Gtk;
using MfGames.GtkExt.Configurators.Widgets;
using Window = Gtk.Window;

namespace MfGames.GtkExt.Configurators
{
	/// <summary>
	/// Implements a configuration screen as a dialog box.
	/// </summary>
	public class ConfiguratorDialog: Dialog
	{
		#region Methods

		/// <summary>
		/// Initializes the entire widget and lays out the class.
		/// </summary>
		/// <param name="selectorTreeStore">The selector tree store.</param>
		private void InitializeWidget(TreeStore selectorTreeStore)
		{
			// Set up the configurator area.
			configuratorArea = new ConfiguratorsSelectorEditorComposite(
				selectorTreeStore);

			VBox.PackStart(configuratorArea, true, true, 0);

			// Depending on the mood determines the buttons we'll have.
			Response += OnResponse;

			if (configuratorArea.ApplyMode == ApplyMode.Explicit)
			{
				// For explict apply, add in the apply, cancel, and ok buttons.
				AddButton("Apply", ResponseType.Apply);
				AddButton("Cancel", ResponseType.Cancel);
				AddButton("Ok", ResponseType.Ok);
			}
			else
			{
				// For instant apply, just have a close button.
				AddButton("Close", ResponseType.Close);
			}

			// Set up spacing.
			BorderWidth = 6;
			VBox.Spacing = 12;
			ActionArea.Spacing = 12;
			ActionArea.BorderWidth = 6;
		}

		/// <summary>
		/// Called when a dialog button is clicked.
		/// </summary>
		/// <param name="sender">The sender of the vent.</param>
		/// <param name="e">The event arguments.</param>
		private void OnResponse(
			object sender,
			ResponseArgs e)
		{
			switch (e.ResponseId)
			{
				case ResponseType.Apply:
					configuratorArea.ApplyConfigurators();
					break;
				case ResponseType.Cancel:
					configuratorArea.CancelConfigurators();
					break;
				case ResponseType.Ok:
					configuratorArea.ApplyConfigurators();
					Destroy();
					break;
				case ResponseType.Close:
					Destroy();
					break;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorDialog"/> class.
		/// </summary>
		/// <param name="treeStore">The tree store.</param>
		/// <param name="dialogTitle">The dialog title.</param>
		/// <param name="parentWindow">The parent window.</param>
		/// <param name="flags">The flags.</param>
		/// <param name="buttonData">The button data.</param>
		public ConfiguratorDialog(
			TreeStore treeStore,
			string dialogTitle = "Configuration",
			Window parentWindow = null,
			DialogFlags flags = DialogFlags.DestroyWithParent,
			params object[] buttonData)
			: base(dialogTitle, parentWindow, flags, buttonData)
		{
			// Save the various parameters as member variables.
			if (treeStore == null)
			{
				throw new ArgumentNullException("treeStore");
			}

			// Set up the widget.
			InitializeWidget(treeStore);

			// Set up the default
			DefaultSize = new Size(400, 250);
		}

		#endregion

		#region Fields

		private ConfiguratorsSelectorEditorComposite configuratorArea;

		#endregion
	}
}
