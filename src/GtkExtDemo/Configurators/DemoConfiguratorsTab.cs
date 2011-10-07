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

using System.Collections.Generic;

using Gtk;

using MfGames.GtkExt.Actions;
using MfGames.GtkExt.Configurators;
using MfGames.GtkExt.Extensions.System.Collections.Generic;

#endregion

namespace GtkExtDemo.Configurators
{
	/// <summary>
	/// Encapsulates the demostration of a configurator tab.
	/// </summary>
	public class DemoConfiguratorsTab
		: DemoTab,
		  IActionFactory
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoConfiguratorsTab"/> class.
		/// </summary>
		public DemoConfiguratorsTab(Window window)
		{
			// Create the configurators in a tree store.
			treeStore = CreateTreeStore();
			this.window = window;

			// Create the configuration view.
			var panel = new ConfiguratorsPanel(treeStore);

			// Add the panel to ourselves.
			PackStart(
				panel,
				true,
				true,
				2);
		}

		#endregion

		#region Configurators

		/// <summary>
		/// Creates the tree store which contains the configurators.
		/// </summary>
		/// <returns></returns>
		private TreeStore CreateTreeStore()
		{
			// Set up the configuration tab with a default configurators.
			var configurators = new List<IGtkConfigurator>();
			configurators.Add(new TextEditorStylesConfigurator());
			configurators.Add(new TextEditorDisplayConfigurator());

			// Create the tree store from the list.
			TreeStore store = configurators.ToTreeStore();
			return store;
		}

		#endregion

		#region IActionFactory

		private readonly TreeStore treeStore;
		private readonly Window window;

		/// <summary>
		/// Creates all the <see cref="Action"/> objects associated with the extending
		/// class.
		/// </summary>
		/// <returns></returns>
		public ICollection<Action> CreateActions()
		{
			var actions = new List<Action>();

			actions.Add(
				new ShowPreferencesAction(
					"Preferences",
					"_Preferences",
					window,
					CreateTreeStore()));

			return actions;
		}

		#endregion
	}
}