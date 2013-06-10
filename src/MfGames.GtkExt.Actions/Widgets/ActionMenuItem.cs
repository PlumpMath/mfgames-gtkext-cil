// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Gdk;
using Gtk;
using MfGames.HierarchicalPaths;
using Action = Gtk.Action;
using Image = Gtk.Image;

namespace MfGames.GtkExt.Actions.Widgets
{
	/// <summary>
	/// Implements an menu item that wraps around an <see cref="Gtk.Action"/> and
	/// handles the custom keybinding from the manager.
	/// </summary>
	public class ActionMenuItem: ImageMenuItem
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionMenuItem"/> class.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="action">The action.</param>
		public ActionMenuItem(
			ActionManager manager,
			Action action)
		{
			// Saves the properties.
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}

			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			// Attach to the action.
			this.action = action;

			action.ConnectProxy(this);

			// The default menu can't handle the chained accelerators, so we 
			// remove the contents of the menu item and recreate it using a 
			// HBox with a custom label for both.			
			var newChild = new HBox(false, 0);

			displayLabel = new Label(action.Label);
			displayLabel.UseUnderline = true;
			displayLabel.Xalign = 0.0f;
			displayLabel.Show();

			newChild.PackStart(displayLabel);

			// Get the keybinding label.
			HierarchicalPath acceleratorPath = manager.GetPrimaryAcceleratorPath(action);

			if (acceleratorPath != null)
			{
				// Get a formatted version of the accelerator path.
				string display = ActionKeybindings.FormatAcceleratorPath(acceleratorPath);
				keybindingLabel = new Label(display);
				keybindingLabel.UseUnderline = false;
				keybindingLabel.Xalign = 1.0f;
				keybindingLabel.Show();

				newChild.PackStart(keybindingLabel);
			}

			// Lay out the contents in the HBox.
			newChild.Show();

			// Remove the old child item and add this.
			Remove(Child);
			Add(newChild);

			// Figure out if we have an icon.
			if (!String.IsNullOrEmpty(action.StockId))
			{
				IconSet iconSet = Style.LookupIconSet(action.StockId);
				Pixbuf image = iconSet.RenderIcon(
					Style, DefaultDirection, State, IconSize.Menu, this, null);
				var iconImage = new Image(image);

				Image = iconImage;
			}
		}

		#endregion

		#region Fields

		private readonly Action action;

		private readonly Label displayLabel;
		private readonly Label keybindingLabel;

		#endregion

		///// <summary>
		///// Called when the menu item is activated.
		///// </summary>
		//protected override void OnActivated()
		//{
		//    action.Activate();
		//}
	}
}
