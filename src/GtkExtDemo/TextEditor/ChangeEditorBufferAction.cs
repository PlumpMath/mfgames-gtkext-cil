// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using MfGames.GtkExt.Actions;
using MfGames.GtkExt.TextEditor;
using MfGames.GtkExt.TextEditor.Models;
using Action = Gtk.Action;

namespace GtkExtDemo.TextEditor
{
	/// <summary>
	/// Implements an action that changes the buffer.
	/// </summary>
	public class ChangeEditorBufferAction: Action,
		IConfigurableAction
	{
		#region Properties

		/// <summary>
		/// Gets the name of the action group to associate with this action.
		/// </summary>
		/// <value>
		/// The name of the group.
		/// </value>
		public string GroupName
		{
			get { return "Editor"; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Called when the menu item is activated.
		/// </summary>
		protected override void OnActivated()
		{
			// Check to see if we don't have an action.
			if (action == null)
			{
				// Clear out the line buffer.
				editorView.ClearLineBuffer();
			}
			else
			{
				// Create the buffer and set it.
				editorView.SetLineBuffer(action());
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangeEditorBufferAction"/>
		/// class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="editorView">The editor view.</param>
		/// <param name="index">The index.</param>
		/// <param name="action">The action to perform while loading.</param>
		public ChangeEditorBufferAction(
			string name,
			string label,
			EditorView editorView,
			int index,
			Func<LineBuffer> action)
			: base(name, label)
		{
			if (editorView == null)
			{
				throw new ArgumentNullException("editorView");
			}

			this.editorView = editorView;
			this.action = action;
		}

		#endregion

		#region Fields

		private readonly Func<LineBuffer> action;
		private readonly EditorView editorView;

		#endregion
	}
}
