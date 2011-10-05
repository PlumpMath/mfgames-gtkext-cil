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
using System.Diagnostics;

using Gtk;

#endregion

namespace MfGames.GtkExt.Configurators.Widgets
{
	/// <summary>
	/// A composite widget which encapsulates both the configurator selector
	/// and the editor area. This is intended to be included in a larger 
	/// widget, such as ConfiguratorsWindow or ConfiguratorsPanel.
	/// </summary>
	public class ConfiguratorsSelectorEditorComposite : HBox
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorsSelectorEditorComposite"/> class.
		/// </summary>
		/// <param name="selectorTreeStore">The tree store which contains three columns (name, HierarchicalPath, and IGtkConfigurator).</param>
		public ConfiguratorsSelectorEditorComposite(TreeStore selectorTreeStore)
		{
			// Save the various parameters as member variables.
			if (selectorTreeStore == null)
			{
				throw new ArgumentNullException("selectorTreeStore");
			}

			this.selectorTreeStore = selectorTreeStore;

			// Set up the widget.
			InitializeWidget();
		}

		#endregion

		#region Configurators

		private readonly TreeStore selectorTreeStore;

		/// <summary>
		/// Gets the configurator selector tree store.
		/// </summary>
		public TreeStore SelectorTreeStore
		{
			[DebuggerStepThrough]
			get { return selectorTreeStore; }
		}

		#endregion

		#region Layout

		/// <summary>
		/// Creates the configurator area which is where the configurators
		/// will have their widgets placed.
		/// </summary>
		/// <returns></returns>
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
			TreeStore treeStore = selectorTreeStore;

			// Create the tree view for inside the scroll.
			var treeView = new TreeView(treeStore);

			treeView.AppendColumn("Configurator", new CellRendererText(), "text", 0);

			// Return the scrolled window.
			scroll.Add(treeView);

			return scroll;
		}

		/// <summary>
		/// Initializes the entire widget and lays out the class.
		/// </summary>
		private void InitializeWidget()
		{
			// The configuration area is split horizontally by a resizable pane.
			var pane = new HPaned();

			// On the left is the selector area.
			pane.Add1(CreateSelectorArea());

			// On the right is the configurator area.
			pane.Add2(CreateConfiguratorArea());

			// Set the default position of the pane to 200 px for the selector.
			pane.Position = 200;

			// Pack the widget into ourselves.
			PackStart(pane, true, true, 0);
		}

		#endregion
	}
}