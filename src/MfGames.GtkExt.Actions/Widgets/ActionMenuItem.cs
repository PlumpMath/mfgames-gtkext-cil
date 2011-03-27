#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
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

using Gdk;

using Gtk;

using Action=Gtk.Action;
using Image=Gtk.Image;

#endregion

namespace MfGames.GtkExt.Actions.Widgets
{
	/// <summary>
	/// Implements an menu item that wraps around an <see cref="Gtk.Action"/> and
	/// handles the custom keybinding from the manager.
	/// </summary>
	public class ActionMenuItem : ImageMenuItem
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

		#region Widgets

		private readonly Label displayLabel;
		private readonly Label keybindingLabel;

		#endregion

		#region Actions

		private readonly Action action;

		///// <summary>
		///// Called when the menu item is activated.
		///// </summary>
		//protected override void OnActivated()
		//{
		//    action.Activate();
		//}

		#endregion
	}
}