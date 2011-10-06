#region Copyright and License

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
	/// A composite widget to display, manage, and edit configurators.
	/// </summary>
	public class ConfiguratorsPanel : VBox
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorsPanel"/> class.
		/// </summary>
		/// <param name="selectorTreeStore">The selector tree store.</param>
		/// <param name="showCloseButton">if set to <c>true</c> [show close button].</param>
		public ConfiguratorsPanel(
			TreeStore selectorTreeStore,
			bool showCloseButton = true)
		{
			// Save the various parameters as member variables.
			if (selectorTreeStore == null)
			{
				throw new ArgumentNullException("selectorTreeStore");
			}

			// Set up the widget.
			InitializeWidget(
				selectorTreeStore,
				showCloseButton);
		}

		#endregion

		#region Layout

		private ConfiguratorsSelectorEditorComposite configuratorArea;

		/// <summary>
		/// Creates the button bar on the bottom of the screen.
		/// </summary>
		/// <param name="showCloseButton"></param>
		protected Widget CreateButtonBar(bool showCloseButton)
		{
			// If we don't show the close button and we are instant apply, we
			// don't have any buttons to show.
			if (!showCloseButton && configuratorArea.ApplyMode == ApplyMode.Instant)
			{
				// Nothing to see here, return a null so it isn't added.
				return null;
			}

			// Create a horizontal button bar.
			var buttons = new HBox();

			// Add in the spacer for the left.
			buttons.PackStart(
				new Label(),
				true,
				true,
				0);

			// For explict apply, add in the apply, cancel, and ok buttons.
			if (configuratorArea.ApplyMode == ApplyMode.Explicit)
			{
				var applyButton = new Button(Stock.Apply);
				applyButton.Clicked += OnApplyClicked;

				buttons.PackStart(
					applyButton,
					false,
					false,
					0);

				var cancelButton = new Button(Stock.Cancel);
				cancelButton.Clicked += OnCancelClicked;

				buttons.PackStart(
					cancelButton,
					false,
					false,
					6);

				if (showCloseButton)
				{
					var okButton = new Button(Stock.Ok);
					okButton.Clicked += OnCloseClicked;

					buttons.PackStart(
						okButton,
						false,
						false,
						6);
				}
			}
			else
			{
				// We don't have to check for showCloseButton again since we
				// had a check at the beginning of this method.
				var closeButton = new Button(Stock.Close);
				closeButton.Clicked += OnCloseClicked;

				buttons.PackStart(
					closeButton,
					false,
					false,
					0);
			}

			// Return the resulting buttons.
			return buttons;
		}

		/// <summary>
		/// Initializes the entire widget and lays out the class.
		/// </summary>
		/// <param name="selectorTreeStore">The selector tree store.</param>
		/// <param name="showCloseButton">if set to <c>true</c> [show close button].</param>
		private void InitializeWidget(
			TreeStore selectorTreeStore,
			bool showCloseButton)
		{
			// Start by doing the VBox stuff to arrange the top levels, a
			// separator bar, and the button bar below.
			var verticalLayout = new VBox();

			// Set up the configurator area.
			configuratorArea = new ConfiguratorsSelectorEditorComposite(
				selectorTreeStore);

			verticalLayout.PackStart(
				configuratorArea,
				true,
				true,
				0);

			// Create the button bar on the bottom.
			Widget buttonBar = CreateButtonBar(showCloseButton);

			if (buttonBar != null)
			{
				// Create the vertical separator. The 12 pixel spacing comes from
				// the Gnome HIG.
				verticalLayout.PackStart(
					new HSeparator(),
					false,
					false,
					12);

				// Add the buttons to the list.
				verticalLayout.PackStart(
					buttonBar,
					false,
					false,
					0);
			}

			// Add the vertical layout to the bin.
			PackStart(verticalLayout);
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