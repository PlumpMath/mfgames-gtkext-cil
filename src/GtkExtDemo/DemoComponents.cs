// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Gtk;
using MfGames.GtkExt;

namespace GtkExtDemo
{
	/// <summary>
	/// Shows off the various GtkExt components.
	/// </summary>
	public class DemoComponents: DemoTab
	{
		#region Methods

		/// <summary>
		/// Fired when the test enumeration changes.
		/// </summary>
		private void OnTestEnumChanged(
			object obj,
			EventArgs args)
		{
			DemoWindow.Statusbar.Push(
				0,
				"ExampleEnumeration: " + testEnumCombo.ActiveEnum + " ("
					+ testEnumCombo.Active + ")");
		}

		#endregion

		#region Constructors

		public DemoComponents()
		{
			// Create the general frame
			uint row = 1;
			var table = new LabelWidgetTable(5, 2);
			table.BorderWidth = 5;

			// Just a text entry
			var nameEntry = new Entry("Bob");
			table.AttachExpanded(row++, "Name", nameEntry);

			// An unexpanded enumeration combo box
			var ecb = new EnumComboBox(typeof (ButtonsType));
			ecb.ActiveEnum = ButtonsType.YesNo;
			table.Attach(row++, "EnumComboBox", ecb);

			// An unexpanded described
			testEnumCombo = new EnumComboBox(typeof (ExampleEnumeration));
			testEnumCombo.Changed += OnTestEnumChanged;
			table.Attach(row++, "ExampleEnumeration", testEnumCombo);

			// Add the string entry
			var sle = new StringListEntry();
			sle.Values = new[]
			{
				"Line 1", "Line 2", "Line 3",
			};
			sle.CompletionStore.AppendValues("List 4");
			sle.CompletionStore.AppendValues("List 5");
			sle.CompletionStore.AppendValues("List 6");
			sle.CompletionStore.AppendValues("List 7");
			table.AttachExpanded(row++, "StringListEntry", sle);

			// Return it
			PackStart(table, false, false, 0);
			PackStart(new Label(""), true, true, 0);
		}

		#endregion

		#region Fields

		private readonly EnumComboBox testEnumCombo;

		#endregion
	}
}
