﻿#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using MfGames.GtkExt.Configurators.Widgets;

#endregion

namespace MfGames.GtkExt.Configurators
{
	/// <summary>
	/// Implements a configuration screen as a dialog box.
	/// </summary>
	public class ConfiguratorDialog : Dialog
	{
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
			: base(dialogTitle,
			       parentWindow,
			       flags,
			       buttonData)
		{
			// Save the various parameters as member variables.
			if (treeStore == null)
			{
				throw new ArgumentNullException("treeStore");
			}

			// Set up the widget.
			InitializeWidget(treeStore);
		}

		#endregion

		#region Layout

		private ConfiguratorsSelectorEditorComposite configuratorArea;

		/// <summary>
		/// Initializes the entire widget and lays out the class.
		/// </summary>
		/// <param name="selectorTreeStore">The selector tree store.</param>
		private void InitializeWidget(TreeStore selectorTreeStore)
		{
			// Set up the configurator area.
			configuratorArea = new ConfiguratorsSelectorEditorComposite(
				selectorTreeStore);

			VBox.PackStart(
				configuratorArea,
				true,
				true,
				0);

			// Add in the spacer for the left.
			ActionArea.PackStart(
				new Label(),
				true,
				true,
				0);

			// For explict apply, add in the apply, cancel, and ok buttons.
			if (configuratorArea.ApplyMode == ApplyMode.Explicit)
			{
				var applyButton = new Button(Stock.Apply);
				applyButton.Clicked += OnApplyClicked;

				ActionArea.PackStart(
					applyButton,
					false,
					false,
					0);

				var cancelButton = new Button(Stock.Cancel);
				cancelButton.Clicked += OnCancelClicked;

				ActionArea.PackStart(
					cancelButton,
					false,
					false,
					6);
				
				var okButton = new Button(Stock.Ok);
				okButton.Clicked += OnCloseClicked;

				ActionArea.PackStart(
					okButton,
					false,
					false,
					6);
			}
			else
			{
				// We don't have to check for showCloseButton again since we
				// had a check at the beginning of this method.
				var closeButton = new Button(Stock.Close);
				closeButton.Clicked += OnCloseClicked;

				ActionArea.PackStart(
					closeButton,
					false,
					false,
					0);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Activated when the apply button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnApplyClicked(
			object sender,
			EventArgs e)
		{
			configuratorArea.ApplyConfigurators();
		}

		/// <summary>
		/// Activated when the cancel button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnCancelClicked(
			object sender,
			EventArgs e)
		{
		}

		/// <summary>
		/// Activated when the close or ok button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCloseClicked(
			object sender,
			EventArgs e)
		{
		}

		#endregion
	}
}