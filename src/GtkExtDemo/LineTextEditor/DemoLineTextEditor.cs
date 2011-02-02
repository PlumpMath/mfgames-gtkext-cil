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

using C5;

using Gtk;

using MfGames.GtkExt.LineTextEditor;
using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Commands;
using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace GtkExtDemo.LineTextEditor
{
	/// <summary>
	/// Contains the basic control for showing off the features of the line
	/// text editor.
	/// </summary>
	public class DemoLineTextEditor : VBox
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoLineTextEditor"/> class.
		/// </summary>
		public DemoLineTextEditor()
		{
			// Create a memory line buffer with random text so the user can
			// edit the buffer.
			ILineBuffer patternLineBuffer = new PatternLineBuffer(1024, 256, 4);
			ILineBuffer lineBuffer = new MemoryLineBuffer(patternLineBuffer);

			// Create an unformatted markup buffer and simple layout with a cache.
			ILineMarkupBuffer lineMarkupBuffer =
				new KeywordLineMarkupBuffer(lineBuffer);
			ILineMarkupBuffer selectionMarkupBuffer =
				new SimpleSelectionLineMarkupBuffer(lineMarkupBuffer);
			ILineLayoutBuffer lineLayoutBuffer =
				new SimpleLineLayoutBuffer(selectionMarkupBuffer);
			ILineLayoutBuffer cachedLayoutBuffer =
				new CachedLineLayoutBuffer(lineLayoutBuffer);

			// Create the text editor with the resulting buffer.
			textEditor = new TextEditor(cachedLayoutBuffer);
			textEditor.Controller.PopulateContextMenu += OnPopulateContextMenu;

			// Wrap the text editor in a scrollbar.
			var scrolledWindow = new ScrolledWindow();
			scrolledWindow.VscrollbarPolicy = PolicyType.Always;
			scrolledWindow.Add(textEditor);

			// Add the editor to the current tab.
			PackStart(scrolledWindow, true, true, 0);
		}

		#endregion

		#region Widgets

		private readonly TextEditor textEditor;

		#endregion

		#region Context Menu

		/// <summary>
		/// Called when the context menu is being populated.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnPopulateContextMenu(object sender,
		                                   PopulateContextMenuArgs args)
		{
			// Add a separator and our custom "function".
			var menuItem = new MenuItem("Reverse Line");
			menuItem.Activated += OnReverseLine;

			args.Menu.Add(new SeparatorMenuItem());
			args.Menu.Add(menuItem);
		}

		/// <summary>
		/// Called when the line is requested to be reversed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnReverseLine(object sender,
								   EventArgs e)
		{
			// Go through all the lines in the selection or if there is no
			// selection, then just the current line.
			Caret caret = textEditor.Caret;
			var command = new Command(caret.Position);

			if (caret.Selection.IsEmpty || caret.Selection.IsSameLine)
			{
				ReverseLine(command, caret.Position.LineIndex);
			}
			else
			{
				for (int lineIndex = caret.Selection.StartPosition.LineIndex;
				     lineIndex <= caret.Selection.EndPosition.LineIndex;
				     lineIndex++)
				{
					ReverseLine(command, lineIndex);
				}
			}

			// Perform the command.
			textEditor.Controller.Do(command);
		}

		/// <summary>
		/// Reverses the text of a line then adds it to the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="lineIndex">Index of the line.</param>
		private void ReverseLine(Command command, int lineIndex)
		{
			// Get the original line text.
			string lineText = textEditor.LineLayoutBuffer.GetLineText(lineIndex);

			// Create a reverse of the text.
			var characters = new ArrayList<char>();
			
			characters.AddAll(lineText);
			characters.Reverse();
			
			var reverseText = new string(characters.ToArray());

			// Add the operations to the command.
			command.Operations.Add(new SetTextOperation(lineIndex, reverseText));

			command.UndoOperations.Add(new SetTextOperation(lineIndex, lineText));
		}

		#endregion
	}
}