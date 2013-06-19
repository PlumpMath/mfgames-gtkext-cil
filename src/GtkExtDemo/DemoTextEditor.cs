// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using Cairo;
using Gtk;
using GtkExtDemo.TextEditor;
using MfGames.GtkExt;
using MfGames.GtkExt.Actions;
using MfGames.GtkExt.TextEditor;
using MfGames.GtkExt.TextEditor.Editing;
using MfGames.GtkExt.TextEditor.Events;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;
using MfGames.GtkExt.TextEditor.Models.Styles;
using Action = Gtk.Action;
using Alignment = Pango.Alignment;

namespace GtkExtDemo
{
	/// <summary>
	/// Contains the basic control for showing off the features of the line
	/// text editor.
	/// </summary>
	public class DemoTextEditor: DemoTab,
		IActionFactory
	{
		#region Methods

		/// <summary>
		/// Creates all the <see cref="Gtk.Action"/> objects associated with the extending
		/// class.
		/// </summary>
		/// <returns></returns>
		public ICollection<Action> CreateActions()
		{
			var actions = new List<Action>();

			actions.Add(
				new ChangeEditorBufferAction(
					"EditableBuffer",
					"_Editable Buffer",
					editorView,
					1,
					CreateEditableLineBuffer));
			actions.Add(
				new ChangeEditorBufferAction(
					"ReadOnlyBuffer",
					"_Read-Only Buffer",
					editorView,
					2,
					CreateReadOnlyLineBuffer));
			actions.Add(
				new ChangeEditorBufferAction(
					"ClearBuffer", "_Clear Buffer", editorView, 3, null));

			return actions;
		}

		/// <summary>
		/// Creates the editable line buffer.
		/// </summary>
		/// <returns></returns>
		private static LineBuffer CreateEditableLineBuffer()
		{
			// Create a patterned line buffer and make it read-write.
			var lineBuffer = new DemoEditableLineBuffer();

			// Decorate the line buffer with something that will highlight the
			// error and warning keywords.
			var keywordBuffer = new KeywordLineBuffer(lineBuffer);

			// Return the resulting buffer.
			return keywordBuffer;
		}

		/// <summary>
		/// Creates the read-only line buffer.
		/// </summary>
		/// <returns></returns>
		private static LineBuffer CreateReadOnlyLineBuffer()
		{
			return new DemoReadOnlyLineBuffer();
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
			// Clear the buffer.
			editorView.SetRenderer(null);

			// Set the menu item toggle states.
			SetBufferMenuStates(false, false, true);
		}

		/// <summary>
		/// Called when the Editable Buffer button is clicked.
		/// </summary>
		private void OnEditableBufferActivated()
		{
			// Create the buffer and set it.
			editorView.SetLineBuffer(CreateEditableLineBuffer());

			// Set the menu item toggle states.
			SetBufferMenuStates(true, false, false);
		}

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
		/// Called when the Read-Only Buffer button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnReadOnlyBufferActivated(
			object sender,
			EventArgs e)
		{
			// Create the buffer and set it.
			editorView.SetLineBuffer(CreateReadOnlyLineBuffer());

			// Set the menu item toggle states.
			SetBufferMenuStates(false, true, false);
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
			Caret caret = editorView.Caret;
			var command = new Command(caret.Position);

			if (caret.Selection.IsEmpty
				|| caret.Selection.IsSameLine)
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
			editorView.Controller.Do(command);
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
			string lineText = editorView.LineBuffer.GetLineText(
				lineIndex, LineContexts.None);

			// Create a reverse of the text.
			//var characters = new ArrayList<char>();

			//characters.AddAll(lineText);
			//characters.Reverse();

			//var reverseText = new string(characters.ToArray());

			//// Add the operations to the command.
			//command.Operations.Add(new SetTextOperation(lineIndex, reverseText));

			//command.UndoOperations.Add(new SetTextOperation(lineIndex, lineText));
		}

		private void SetBufferMenuStates(
			bool checkEditable,
			bool checkReadOnly,
			bool checkClear)
		{
			//// Remove the events from the items to avoid callbacks on the items.
			//editableBufferMenuItem.Activated -= OnEditableBufferActivated;
			//readOnlyBufferMenuItem.Activated -= OnReadOnlyBufferActivated;
			//clearBufferMenuItem.Activated -= OnClearBuffer;

			//// Check the boxes.
			//editableBufferMenuItem.Active = checkEditable;
			//readOnlyBufferMenuItem.Active = checkReadOnly;
			//clearBufferMenuItem.Active = checkClear;

			//// Add the events back in.
			//editableBufferMenuItem.Activated += OnEditableBufferActivated;
			//readOnlyBufferMenuItem.Activated += OnReadOnlyBufferActivated;
			//clearBufferMenuItem.Activated += OnClearBuffer;
		}

		/// <summary>
		/// Configures the theme for all the elements used in the demo.
		/// </summary>
		private void SetupTheme()
		{
			// Grab the theme.
			Theme theme = editorView.Theme;

			// Set up the indicator styles.
			theme.IndicatorStyles["Error"] = new IndicatorStyle(
				"Error", 100, new Color(1, 0, 0));
			theme.IndicatorStyles["Warning"] = new IndicatorStyle(
				"Warning", 10, new Color(1, 165 / 255.0, 0));
			theme.IndicatorRenderStyle = IndicatorRenderStyle.Ratio;
			theme.IndicatorPixelHeight = 2;
			theme.IndicatorRatioPixelGap = 1;

			var indicatorBackgroundStyle = new RegionBlockStyle();
			indicatorBackgroundStyle.BackgroundColor = new Color(1, 0.9, 1);
			//indicatorBackgroundStyle.Borders.SetBorder(new Border(1, new Color(0.5, 0, 0)));
			theme.RegionStyles[IndicatorView.BackgroundRegionName] =
				indicatorBackgroundStyle;

			var indicatorVisibleStyle = new RegionBlockStyle();
			indicatorVisibleStyle.BackgroundColor = new Color(1, 1, 0.9);
			indicatorVisibleStyle.Borders.SetBorder(new Border(1, new Color(0, 0.5, 0)));
			theme.RegionStyles[IndicatorView.VisibleRegionName] = indicatorVisibleStyle;

			// Set up the editable text styles.
			for (var type = DemoLineStyleType.Default;
				type <= DemoLineStyleType.Break;
				type++)
			{
				// Create a line style for this type.
				var lineStyle = new LineBlockStyle(theme.TextLineStyle);

				theme.LineStyles[type.ToString()] = lineStyle;

				// Custom the style based on type.
				switch (type)
				{
					case DemoLineStyleType.Chapter:
						lineStyle.FontDescription =
							FontDescriptionCache.GetFontDescription("Serif Bold 24");
						lineStyle.Borders.Bottom = new Border(2, new Color(0, 0, 0));
						lineStyle.Margins.Bottom = 5;
						break;

					case DemoLineStyleType.Heading:
						lineStyle.FontDescription =
							FontDescriptionCache.GetFontDescription("Sans Bold 18");
						lineStyle.Padding.Left = 25;
						break;

					case DemoLineStyleType.Borders:
						lineStyle.Padding.Left = 15;
						lineStyle.Padding.Right = 15;
						lineStyle.Margins.Left = 10;
						lineStyle.Margins.Right = 10;
						lineStyle.Margins.Top = 10;
						lineStyle.Margins.Bottom = 10;
						lineStyle.Borders.Bottom = new Border(5, new Color(1, 0, 0));
						lineStyle.Borders.Top = new Border(5, new Color(0, 1, 0));
						lineStyle.Borders.Right = new Border(5, new Color(0, 0, 1));
						lineStyle.Borders.Left = new Border(5, new Color(1, 0, 1));
						break;

					case DemoLineStyleType.Default:
						lineStyle.Padding.Left = 50;
						break;

					case DemoLineStyleType.Break:
						lineStyle.Padding.Left = 50;
						lineStyle.Alignment = Alignment.Center;
						break;
				}
			}

			// Create the inactive header style.
			var inactiveHeadingStyle = new LineBlockStyle(theme.LineStyles["Heading"]);

			inactiveHeadingStyle.ForegroundColor = new Color(0.8, 0.8, 0.8);

			theme.LineStyles["Inactive Heading"] = inactiveHeadingStyle;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoTextEditor"/> class.
		/// </summary>
		public DemoTextEditor()
		{
			// Create the text editor with the resulting buffer.
			editorView = new EditorView();
			editorView.Controller.PopulateContextMenu += OnPopulateContextMenu;

			// Update the theme with some additional colors.
			SetupTheme();

			// Wrap the text editor in a scrollbar.
			var scrolledWindow = new ScrolledWindow();
			scrolledWindow.VscrollbarPolicy = PolicyType.Always;
			scrolledWindow.Add(editorView);

			// Create the indicator bar that is 10 px wide.
			indicatorView = new IndicatorView(editorView);
			indicatorView.SetSizeRequest(20, 1);

			// Create the drop down list with the enumerations.
			var lineStyleCombo = new EnumComboBox(typeof (DemoLineStyleType));
			lineStyleCombo.Sensitive = false;

			// Add the editor and bar to the current tab.
			var editorBand = new HBox(false, 0);
			editorBand.PackStart(scrolledWindow, true, true, 0);
			editorBand.PackStart(indicatorView, false, false, 4);

			// Controls band
			var controlsBand = new HBox(false, 0);
			controlsBand.PackStart(lineStyleCombo, false, false, 0);
			controlsBand.PackStart(new Label(), true, true, 0);

			// Create a vbox and use it to add the combo boxes.
			var verticalLayout = new VBox(false, 4);
			verticalLayout.BorderWidth = 4;
			verticalLayout.PackStart(editorBand, true, true, 0);
			verticalLayout.PackStart(controlsBand, false, false, 4);

			// Add the editor and the controls into a vertical box.
			PackStart(verticalLayout, true, true, 2);

			// Create the first buffer.
			editorView.SetLineBuffer(CreateEditableLineBuffer());
			SetBufferMenuStates(true, false, false);
		}

		#endregion

		#region Fields

		private readonly EditorView editorView;
		private readonly IndicatorView indicatorView;

		#endregion
	}
}
