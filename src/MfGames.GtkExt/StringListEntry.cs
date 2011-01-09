using Gtk;
using System;
using System.Collections;

namespace MfGames.GtkExt
{
	/// <summary>
	/// Encapsulates into a single widget that handles text entry,
	/// including text completion.
	/// </summary>
	public class StringListEntry
	: Table
	{
		private Button add;
		private Button remove;
		private TreeView list;
		private ListStore store;
		private Entry entry;
		private ListStore completionStore =
			new ListStore(typeof(string));

		/// <summary>
		/// Constructs a blank string list entry.
		/// </summary>
		public StringListEntry()
			: base(2, 2, false)
		{
			// Set up our internals
			ColumnSpacing = 5;

			// Create the text entry
			entry = new Entry();
			entry.Changed += OnEntryChanged;
			entry.Completion = new EntryCompletion();
			entry.Completion.Model = CompletionStore;
			entry.Completion.TextColumn = 0;
			Attach(entry, 0, 1, 0, 1,
				AttachOptions.Fill | AttachOptions.Expand,
				AttachOptions.Fill | AttachOptions.Expand,
				0, 0);

			// Create the text list
			list = new TreeView();
			store = new ListStore(typeof(string));
			list.Model = store;
			list.HeadersVisible = false;
			list.Selection.Mode = SelectionMode.Multiple;
			list.RowActivated += OnSelectionChanged;
			list.CursorChanged += OnSelectionChanged;
			list.ToggleCursorRow += OnSelectionChanged;
			list.AppendColumn("Strings", new CellRendererText (), "text", 0);

			ScrolledWindow sw = new ScrolledWindow();
			sw.Add(list);
			sw.ShadowType = ShadowType.In;
			Attach(sw, 0, 1, 1, 2,
				AttachOptions.Fill | AttachOptions.Expand,
				AttachOptions.Fill | AttachOptions.Expand,
				0, 0);

			// Create the add button
			add = new Button(Stock.Add);
			add.Sensitive = false;
			add.Clicked += OnAddClicked;
			Attach(add, 1, 2, 0, 1,
				AttachOptions.Fill,
				AttachOptions.Fill | AttachOptions.Expand,
				0, 0);

			// Create the remove button
			VBox box = new VBox();
			remove = new Button(Stock.Remove);
			remove.Sensitive = false;
			remove.Clicked += OnRemoveClicked;
			box.PackStart(remove, false, false, 0);
			box.PackStart(new Label(), true, true, 0);
			Attach(box, 1, 2, 1, 2,
				AttachOptions.Fill,
				AttachOptions.Fill | AttachOptions.Expand,
				0, 0);
		}

#region Events
		public EventHandler EntryAdded;
		public EventHandler EntryRemoved;

		/// <summary>
		/// Triggered when the add button is clicked.
		/// </summary>
		protected void OnAddClicked(object sender, EventArgs args)
		{
			// Add it
			strings.Add(entry.Text);
			Refresh();
			entry.Text = "";

			// Fire the event
			OnEntryAdded(sender, args);
		}

		/// <summary>
		/// This is the default operation when a new entry is added.
		/// </summary>
		protected void OnEntryAdded(object sender, EventArgs args)
		{
			if (EntryAdded != null)
				EntryAdded(sender, args);
		}

		/// <summary>
		/// This function is called when the entry is changed.
		/// </summary>
		protected void OnEntryChanged(object sender, EventArgs args)
		{
			add.Sensitive = entry.Text != "";
		}
	
		/// <summary>
		/// This is the default operation when an entry is removed.
		/// </summary>
		protected void OnEntryRemoved(object sender, EventArgs args)
		{
			if (EntryRemoved != null)
				EntryRemoved(sender, args);
		}

		/// <summary>
		/// Triggered when the remove button is clicked.
		/// </summary>
		protected void OnRemoveClicked(object sender, EventArgs args)
		{
			// Go through the selection
			foreach (TreePath tp in list.Selection.GetSelectedRows())
			{
				// Get the iter
				TreeIter iter;

				store.GetIter(out iter, tp);
				string str = (string) store.GetValue(iter, 0);
				strings.Remove(str);

				// Fire the event
				OnEntryRemoved(sender, args);
			}

			// Refresh the list
			remove.Sensitive = false;
			Refresh();
		}

		/// <summary>
		/// If the row is activated, enable the remove button.
		/// </summary>
		protected virtual void OnSelectionChanged(
			object sender, EventArgs args)
		{
			remove.Sensitive = list.Selection.CountSelectedRows() > 0;
		}
#endregion

#region Properties
		/// <summary>
		/// Contains a read-only store for entry completion. The first
		/// column is a typeof(string) and contains the entries.
		/// </summary>
		public ListStore CompletionStore
		{
			get { return completionStore; }
		}
#endregion

#region Strings
		private ArrayList strings = new ArrayList();

		/// <summary>
		/// Contains the array of strings used for population.
		/// </summary>
		public string [] Values
		{
			get { return (string []) strings.ToArray(typeof(string)); }
			set
			{
				strings = new ArrayList(value);
				strings.Sort();
				Refresh();
			}
		}

		/// <summary>
		/// Rebuilds the list of strings.
		/// </summary>
		public void Refresh()
		{
			// Sort the strings
			strings.Sort();

			// Clear it out and add them
			store.Clear();

			foreach (string str in strings)
				store.AppendValues(str);
		}
#endregion
	}
}
