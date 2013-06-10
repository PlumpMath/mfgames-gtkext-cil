// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using Gtk;

namespace MfGames.GtkExt.Configurators.Widgets
{
	/// <summary>
	/// A composite widget which encapsulates both the configurator selector
	/// and the editor area. This is intended to be included in a larger 
	/// widget, such as ConfiguratorsWindow or ConfiguratorsPanel.
	/// </summary>
	public class ConfiguratorsSelectorEditorComposite: HBox
	{
		#region Properties

		/// <summary>
		/// Gets the apply mode which all the configurators in the composite
		/// use.
		/// </summary>
		public ApplyMode ApplyMode { get; private set; }

		/// <summary>
		/// Gets the configurator selector tree store.
		/// </summary>
		public TreeStore SelectorTreeStore { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Tells each of the configurators to apply their changes.
		/// </summary>
		public void ApplyConfigurators()
		{
			if (ApplyMode == ApplyMode.Explicit)
			{
				configurators.ForEach(configurator => configurator.ApplyConfigurator());
			}
		}

		/// <summary>
		/// Cancels each of the configurators and restores the state.
		/// </summary>
		public void CancelConfigurators()
		{
			if (ApplyMode == ApplyMode.Explicit)
			{
				configurators.ForEach(configurator => configurator.CancelConfigurator());
			}
		}

		/// <summary>
		/// Create the selector area which typically has the tree view of
		/// the selectors.
		/// </summary>
		/// <returns></returns>
		protected virtual Widget CreateSelectorArea()
		{
			// The selector can be fairly big, so create a scrolled window.
			var scroll = new ScrolledWindow();
			scroll.ShadowType = ShadowType.EtchedIn;

			// Create the tree model from the configurator list.
			TreeStore treeStore = SelectorTreeStore;

			// Create the tree view for inside the scroll.
			treeView = new TreeView(treeStore);
			treeView.EnableTreeLines = true;
			treeView.HeadersVisible = false;
			treeView.Selection.Mode = SelectionMode.Single;
			treeView.Selection.Changed += OnSelectionChanged;
			treeView.AppendColumn("Configurator", new CellRendererText(), "text", 0);

			// Return the scrolled window.
			scroll.Add(treeView);

			return scroll;
		}

		/// <summary>
		/// Creates the configurator area which is where the configurators
		/// will have their widgets placed.
		/// </summary>
		/// <returns></returns>
		private Widget CreateConfiguratorArea()
		{
			configuratorFrame = new Frame();
			configuratorFrame.Shadow = ShadowType.None;

			return configuratorFrame;
		}

		/// <summary>
		/// Initializes the configurators in this composite.
		/// </summary>
		private void InitializeConfigurators()
		{
			configurators.ForEach(configurator => configurator.InitializeConfigurator());
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

			// Set the default position of the pane to 100 px for the selector.
			pane.Position = 100;

			// Pack the widget into ourselves.
			PackStart(pane, true, true, 0);
		}

		/// <summary>
		/// Called when a tree row is activated.
		/// </summary>
		/// <param name="sender">The o.</param>
		/// <param name="e">The args.</param>
		private void OnSelectionChanged(
			object sender,
			EventArgs e)
		{
			// Clear out the configurator frame.
			configuratorFrame.Remove(configuratorFrame.Child);

			// Figure out what the selection is.
			TreeIter treeIter;

			if (treeView.Selection.GetSelected(out treeIter))
			{
				// Get the configurator for the selected row.
				var configurator =
					(IGtkConfigurator) SelectorTreeStore.GetValue(treeIter, 2);

				// If the configurator is not null, then we get the widget and set the frame.
				if (configurator == null)
				{
					// This is a null configurator, we want to move down to the
					// the first valid configurator instead.
					TreeIter childTreeIter;

					if (SelectorTreeStore.IterChildren(out childTreeIter, treeIter))
					{
						// We have a child tree element, make sure it is visible.
						TreePath treePath = SelectorTreeStore.GetPath(treeIter);
						treeView.ExpandToPath(treePath);

						// Select this child element.
						treeView.Selection.SelectIter(childTreeIter);
					}
				}
				else
				{
					// We have a configurator, so select and show it.
					Widget configuratorWidget = configurator.CreateConfiguratorWidget();

					configuratorFrame.Add(configuratorWidget);
					configuratorFrame.ShowAll();
				}
			}
		}

		/// <summary>
		/// Sets the selector tree store.
		/// </summary>
		private void VerifyConfigurators()
		{
			// Go through the tree store and make sure the settings are
			// correct and that we don't have a conflict with the buttons. We
			// use a delegate to avoid creating a member variable.
			ApplyMode? localApplyMode = null;

			SelectorTreeStore.Foreach(
				delegate(TreeModel model,
					TreePath path,
					TreeIter iter)
				{
					// Pull out the configurator which may be null.
					var configurator = (IGtkConfigurator) model.GetValue(iter, 2);

					if (configurator == null)
					{
						// Continue parsing through the store.
						return false;
					}

					// Add the configurator to the list.
					configurators.Add(configurator);

					// Get the apply mode and look for conflicts with the
					// other existing configurators.
					ApplyMode configuratorApplyMode = configurator.ApplyMode;

					if (!localApplyMode.HasValue)
					{
						localApplyMode = configuratorApplyMode;
					}

					if (localApplyMode.Value != configuratorApplyMode)
					{
						throw new InvalidOperationException(
							string.Format(
								"Cannot add configurator \"{0}\" because of a conflicting apply mode of {1}.",
								configurator.HierarchicalPath,
								configurator.ApplyMode));
					}

					// Continue parsing through the store.
					return false;
				});

			// If we don't have an apply mode at this point, we didn't get a
			// configurator.
			if (!localApplyMode.HasValue)
			{
				throw new InvalidOperationException(
					"Cannot verify without at least one configurator in the tree store.");
			}

			// Set the apply mode, which is used with the initialization.
			ApplyMode = localApplyMode.Value;
		}

		#endregion

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

			SelectorTreeStore = selectorTreeStore;

			// Create the configurator list.
			configurators = new List<IGtkConfigurator>();

			// Verify that the configuration setup is acceptable.
			VerifyConfigurators();

			// Set up the widget and the configurators.
			InitializeWidget();
			InitializeConfigurators();
		}

		#endregion

		#region Fields

		private Frame configuratorFrame;
		private readonly List<IGtkConfigurator> configurators;
		private TreeView treeView;

		#endregion
	}
}
