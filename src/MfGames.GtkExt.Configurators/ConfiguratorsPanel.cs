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
		public ConfiguratorsPanel(TreeStore selectorTreeStore)
		{
			// Save the various parameters as member variables.
			if (selectorTreeStore == null)
			{
				throw new ArgumentNullException("selectorTreeStore");
			}

			// Set up the widget.
			InitializeWidget(selectorTreeStore);
		}

		#endregion

		#region Layout

		private ConfiguratorsSelectorEditorComposite configuratorArea;

		/// <summary>
		/// Creates the button bar on the bottom of the screen.
		/// </summary>
		protected Widget CreateButtonBar()
		{
			return new Label("Buttons");
		}

		/// <summary>
		/// Initializes the entire widget and lays out the class.
		/// </summary>
		private void InitializeWidget(TreeStore selectorTreeStore)
		{
			// Start by doing the VBox stuff to arrange the top levels, a
			// separator bar, and the button bar below.
			var verticalLayout = new VBox();

			// Set up the configurator area.
			configuratorArea = new ConfiguratorsSelectorEditorComposite(
				selectorTreeStore);

			verticalLayout.PackStart(configuratorArea, true, true, 0);

			// Create the button bar on the bottom.
			Widget buttonBar = CreateButtonBar();

			if (buttonBar != null)
			{
				// Create the vertical separator.
				verticalLayout.PackStart(new HSeparator(), false, false, 4);

				// Add the buttons to the list.
				verticalLayout.PackStart(buttonBar, false, false, 4);
			}

			// Add the vertical layout to the bin.
			PackStart(verticalLayout);
		}

		#endregion
	}
}