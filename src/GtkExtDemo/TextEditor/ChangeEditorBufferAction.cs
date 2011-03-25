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

using MfGames.GtkExt.TextEditor;
using MfGames.GtkExt.TextEditor.Models;

using Action=Gtk.Action;

#endregion

namespace GtkExtDemo.TextEditor
{
	/// <summary>
	/// Implements an action that changes the buffer.
	/// </summary>
	public class ChangeEditorBufferAction : Action
	{
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
			this.index = index;
			this.action = action;
		}

		#endregion

		#region Action

		private readonly Func<LineBuffer> action;
		private readonly EditorView editorView;
		private readonly int index;

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
	}
}