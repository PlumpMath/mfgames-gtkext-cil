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

using MfGames.GtkExt.Extensions.MfGames.Collections;
using MfGames.Collections;

#endregion

namespace MfGames.GtkExt.Configurators
{
	/// <summary>
	/// A composite widget to display, manage, and edit configurators.
	/// </summary>
	public class ConfiguratorPanel : VBox
	{
		#region Constructors

		public ConfiguratorPanel(HierarchicalPathTreeCollection<IGtkConfigurator> configurators)
		{
			// Save the various parameters as member variables.
			if (configurators == null)
			{
				throw new ArgumentNullException("configurators");
			}

			this.configurators = configurators;

			// Set up the widget.
			InitializeWidget();
		}

		#endregion

		#region Configurators

		private HierarchicalPathTreeCollection<IGtkConfigurator> configurators;
 
		#endregion

		#region Layout

		/// <summary>
		/// Initializes the entire widget and lays out the class.
		/// </summary>
		private void InitializeWidget()
		{
			// Start by doing the VBox stuff to arrange the top levels, a
			// separator bar, and the button bar below.
			var verticalLayout = new VBox();

			// Set up the configurator area.
			Widget configuratorArea = CreateWorkingArea();

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

		/// <summary>
		/// Creates the area on the screen that contains both the configurator
		/// selector and the configurator working area.
		/// </summary>
		/// <returns></returns>
		protected Widget CreateWorkingArea()
		{
			// The configuration area is split horizontally by a resizable pane.
			var pane = new HPaned();
			
			// On the left is the selector area.
			pane.Add1(CreateSelectorArea());
			
			// On the right is the configurator area.
			pane.Add2(CreateConfiguratorArea());

			// Set the default position of the pane to 200 px for the selector.
			pane.Position = 200;

			// Return the pane.
			return pane;
		}

		private Widget CreateConfiguratorArea()
		{
			return new Label("Bob");
		}

		/// <summary>
		/// Create the selector area which typically has the tree view of
		/// the selectors.
		/// </summary>
		/// <returns></returns>
		protected Widget CreateSelectorArea()
		{
			// The selector can be fairly big, so create a scrolled window.
			var scroll = new ScrolledWindow();

			// Create the tree model from the configurator list.
			TreeStore treeStore = configurators.ToTreeStore();

			// Create the tree view for inside the scroll.
			var treeView = new TreeView(treeStore);

			treeView.AppendColumn("Configurator", new CellRendererText(), "text", 0);

			// Return the scrolled window.
			scroll.Add(treeView);

			return scroll;
		}

		/// <summary>
		/// Creates the button bar on the bottom of the screen.
		/// </summary>
		protected Widget CreateButtonBar()
		{
			return new Label("Buttons");
		}

		#endregion
	}
}