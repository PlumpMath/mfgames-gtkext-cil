// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using Gtk;
using MfGames.GtkExt.Actions;
using MfGames.GtkExt.Configurators;
using MfGames.GtkExt.Extensions.System.Collections.Generic;

namespace GtkExtDemo.Configurators
{
	/// <summary>
	/// Encapsulates the demostration of a configurator tab.
	/// </summary>
	public class DemoConfiguratorsTab: DemoTab,
		IActionFactory
	{
		#region Methods

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
					"Preferences", "_Preferences", window, CreateTreeStore()));

			return actions;
		}

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
			PackStart(panel, true, true, 2);
		}

		#endregion

		#region Fields

		private readonly TreeStore treeStore;
		private readonly Window window;

		#endregion
	}
}
