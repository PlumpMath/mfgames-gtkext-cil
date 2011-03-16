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
using System.Text;

using C5;

using Cairo;

using Gtk;

using MfGames.GtkExt;
using MfGames.GtkExt.LineTextEditor;
using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Commands;
using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Enumerations;
using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Indicators;
using MfGames.GtkExt.LineTextEditor.Renderers;
using MfGames.GtkExt.LineTextEditor.Renderers.Cache;
using MfGames.GtkExt.LineTextEditor.Visuals;

using ActionEntry=Gtk.ActionEntry;

#endregion

namespace GtkExtDemo.LineTextEditor
{
	/// <summary>
	/// Contains the basic control for showing off the features of the line
	/// text editor.
	/// </summary>
	public class DemoLineTextEditor : DemoTab
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoLineTextEditor"/> class.
		/// </summary>
		public DemoLineTextEditor()
		{
			// Create the text editor with the resulting buffer.
			textEditor = new TextEditor();
			textEditor.SetTextRenderer(CreateRenderer());
			textEditor.Controller.PopulateContextMenu += OnPopulateContextMenu;

			// Update the theme with some additional colors.
			Theme theme = textEditor.Theme;

			theme.IndicatorStyles["Error"] = new IndicatorStyle(
				"Error", 100, new Color(1, 0, 0));
			theme.IndicatorStyles["Warning"] = new IndicatorStyle(
				"Warning", 10, new Color(1, 165 / 255.0, 0));
			theme.IndicatorRenderStyle = IndicatorRenderStyle.Ratio;
			theme.IndicatorPixelHeight = 2;
			theme.IndicatorRatioPixelGap = 1;

			// Wrap the text editor in a scrollbar.
			var scrolledWindow = new ScrolledWindow();
			scrolledWindow.VscrollbarPolicy = PolicyType.Always;
			scrolledWindow.Add(textEditor);

			// Create the indicator bar that is 10 px wide.
			indicatorBar = new TextIndicatorBar(textEditor);
			indicatorBar.SetSizeRequest(20, 1);

            // Create the drop down list with the enumerations.
            var lineStyleCombo = new EnumComboBox(typeof(LineStyleType));

			// Add the editor and bar to the current tab.
			var editorBand = new HBox(false, 0);
			editorBand.PackStart(scrolledWindow, true, true, 0);
			editorBand.PackStart(indicatorBar, false, false, 4);

            // Controls band
            var controlsBand = new HBox(false, 0);
            controlsBand.PackStart(lineStyleCombo, false, false, 0);
            controlsBand.PackStart(new Label(), true, true, 0);

            // Create a vbox and use it to add the combo boxes.
            var verticalLayout = new VBox(false, 4);
            verticalLayout.BorderWidth = 4;
            verticalLayout.PackStart(controlsBand, false, false, 0);
            verticalLayout.PackStart(editorBand, true, true, 4);

			// Add the editor and the controls into a vertical box.
			PackStart(verticalLayout, true, true, 2);
		}

		#endregion

		#region Buffers

		/// <summary>
		/// Creates an editable buffer.
		/// </summary>
		/// <returns></returns>
		private TextRenderer CreateRenderer()
		{
			// Create a patterned line buffer and make it read-write.
			var patternLineBuffer = new PatternLineBuffer(1024, 256, 4);
			var lineBuffer = new MemoryLineBuffer(patternLineBuffer);

			// Decorate the line buffer with something that will highligh the
			// error and warning keywords.
			var keywordBuffer = new KeywordLineBuffer(lineBuffer);

			// Finally, wrap it in a cached buffer.
			return new CachedTextRenderer(textEditor, keywordBuffer);
		}

		#endregion

		#region Widgets

		private readonly TextIndicatorBar indicatorBar;
		private readonly TextEditor textEditor;

		#region Events

		/// <summary>
		/// Called when the Editable Buffer button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnSetStyledBuffer(
			object sender,
			EventArgs e)
		{
			TextRenderer textRenderer = CreateRenderer();

			textEditor.SetTextRenderer(textRenderer);
		}

		/// <summary>
		/// Called when the No Buffer button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnClearBuffer(
			object sender,
			EventArgs e)
		{
			textEditor.SetTextRenderer(null);
		}

		#endregion

		#region Setup

        /// <summary>
        /// Configures the GUI and allows a demo to add menu and widgets.
        /// </summary>
        /// <param name="demo">The demo.</param>
        /// <param name="uiManager">The UI manager.</param>
        public override void ConfigureGui(Demo demo, UIManager uiManager)
        {
            // Create the overlay menu items for this demo.
            var uiInfo = new StringBuilder();

            uiInfo.Append("<ui>");
            uiInfo.Append("<menubar name='MenuBar'>");
            uiInfo.Append("<menu action='DlteMenu'>");
            uiInfo.Append("<menuitem action='DlteSetStyledBuffer'/>");
            uiInfo.Append("<menuitem action='DlteSetLargeBuffer'/>");
            uiInfo.Append("<menuitem action='DlteClearBuffer'/>");
            uiInfo.Append("</menu>");
            uiInfo.Append("</menubar>");
            uiInfo.Append("</ui>");

            // Set up the action items for the menu.
            var entries = new[]
                          {
                              // "File" Menu
                              new ActionEntry(
                                  "DlteMenu",
                                  null,
                                  "_Text Editor",
                                  null,
                                  null,
                                  null),
                              new ActionEntry(
                                  "DlteSetStyledBuffer",
                                  null,
                                  "Set _Styled Buffer",
                                  null,
                                  null,
                                  OnSetStyledBuffer), 
                              new ActionEntry(
                                  "DlteSetLargeBuffer",
                                  null,
                                  "Set _Large Buffer",
                                  null,
                                  null,
                                  null), 
                              new ActionEntry(
                                  "DlteClearBuffer",
                                  null,
                                  "_Clear Buffer",
                                  null,
                                  null,
                                  OnClearBuffer), 
                          };

            // Append the elements to the UI.
            // Build up the actions
            var actions = new ActionGroup("group");
            actions.Add(entries);

            uiManager.InsertActionGroup(actions, 0);
            demo.AddAccelGroup(uiManager.AccelGroup);

            // Set up the interfaces from XML
            uiManager.AddUiFromString(uiInfo.ToString());
        }

	    #endregion

		#endregion

		#region Context Menu

		/// <summary>
		/// Called when the context menu is being populated.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnPopulateContextMenu(
			object sender,
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
		private void OnReverseLine(
			object sender,
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
		private void ReverseLine(
			Command command,
			int lineIndex)
		{
			// Get the original line text.
			string lineText = textEditor.LineBuffer.GetLineText(lineIndex);

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