// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;
using MfGames.GtkExt;
using MfGames.GtkExt.Configurators;

namespace GtkExtDemo.Configurators
{
	/// <summary>
	/// An action to show the user preferences as a non-modal dialog box.
	/// </summary>
	public class ShowPreferencesAction: Action
	{
		#region Methods

		/// <summary>
		/// Called when the action is choosen or activated.
		/// </summary>
		protected override void OnActivated()
		{
			// Create a new configurator dialog.
			var dialog = new ConfiguratorDialog(treeStore, "Preferences", parentWindow);

			// Use the saved settings, if there is one.
			WindowStateSettings.Instance.RestoreState(dialog, "Preferences", true);

			// Show the dialog to the user.
			dialog.ShowAll();
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ShowPreferencesAction"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="parentWindow">The parent window.</param>
		/// <param name="treeStore"></param>
		public ShowPreferencesAction(
			string name,
			string label,
			Window parentWindow,
			TreeStore treeStore)
			: base(name, label)
		{
			this.parentWindow = parentWindow;
			this.treeStore = treeStore;
		}

		#endregion

		#region Fields

		private readonly Window parentWindow;
		private readonly TreeStore treeStore;

		#endregion
	}
}
