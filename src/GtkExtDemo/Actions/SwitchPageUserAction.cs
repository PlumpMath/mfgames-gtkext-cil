using System;

using Gtk;

using MfGames;
using MfGames.GtkExt.Actions;

namespace GtkExtDemo.Actions
{
	/// <summary>
	/// Defines a user action that switches notebook pages.
	/// </summary>
	public class SwitchPageUserAction : IUserAction
	{
		private readonly Notebook notebook;
		private readonly int page;
		private readonly string name;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchPageUserAction"/> class.
		/// </summary>
		/// <param name="notebook">The notebook.</param>
		/// <param name="page">The page.</param>
		/// <param name="label">The label.</param>
		public SwitchPageUserAction(Notebook notebook, int page, string label)
		{
			this.notebook = notebook;
			this.page = page;
			this.name = label;
		}

		#endregion

		#region User Actions

		/// <summary>
		/// Gets the name of the group associated with this action. This cannot
		/// be <see langword="null"/> or empty.
		/// </summary>
		/// <value>The name of the group.</value>
		public string GroupName
		{
			get { return "Demo"; }
		}

		/// <summary>
		/// Gets the path that this action shows up in the configuration screen.
		/// This must be unique across the entire system, so it is desired
		/// that external implementors have the top item be their company or
		/// product name (e.g., "/MfGames/TextEditor/ClearAll").
		/// 
		/// This is also the key used for serialization.
		/// </summary>
		public HierarchicalPath ConfigurationPath
		{
			get { return new HierarchicalPath("/View/" + name); }
		}

		/// <summary>
		/// Gets the Gtk-formatted label, including mnemonics.
		/// </summary>
		/// <value>The label.</value>
		public string Label
		{
			get { return "_" + name; }
		}

		/// <summary>
		/// Gets the name suitable for showing to the user.
		/// </summary>
		/// <value>The display name.</value>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the icon ID.
		/// </summary>
		/// <value>The icon id.</value>
		public string IconId
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the stock id, or <see langword="null"/> if this is not a stock item.
		/// </summary>
		/// <value>The stock id.</value>
		public string StockId
		{
			get { return null; }
		}

		/// <summary>
		/// Gets a value indicating whether this action can be undone.
		/// </summary>
		/// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
		public bool CanUndo
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance can be toggled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can be toggled; otherwise, <c>false</c>.
		/// </value>
		public bool IsTogglable
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IUserAction"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Sensitive
		{
			get { return true; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IUserAction"/> is toggled.
		/// </summary>
		/// <value><c>true</c> if toggled; otherwise, <c>false</c>.</value>
		public bool Toggled
		{
			get { throw new InvalidOperationException(); }
			set { throw new InvalidOperationException(); }
		}

		/// <summary>
		/// Performs the action represented by the command.
		/// </summary>
		public void Do()
		{
			notebook.Page = page;
		}

		/// <summary>
		/// Undoes the action performed by this command. If <see cref="CanUndo"/> is 
		/// <see langword="false"/>, then this throws an 
		/// <see cref="InvalidOperationException"/>.
		/// </summary>
		public void Undo()
		{
			throw new InvalidOperationException();
		}

		#endregion
	}
}