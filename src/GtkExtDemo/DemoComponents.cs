using Gtk;
using MfGames.GtkExt;
using System;

public class DemoComponents
: VBox
{
	private EnumComboBox testEnumCombo;

	public DemoComponents()
	{
		// Create the general frame
		uint row = 1;
		LabelWidgetTable table = new LabelWidgetTable(5, 2);
		table.BorderWidth = 5;

		// Just a text entry
		Entry nameEntry = new Entry("Bob");
		table.AttachExpanded(row++, "Name", nameEntry);

		// An unexpanded enumeration combo box
		EnumComboBox ecb = new EnumComboBox(typeof(ButtonsType));
		ecb.ActiveEnum = ButtonsType.YesNo;
		table.Attach(row++, "EnumComboBox", ecb);

		// An unexpanded described
		testEnumCombo = new EnumComboBox(typeof(TestEnum));
		testEnumCombo.Changed += OnTestEnumChanged;
		table.Attach(row++, "Enum.ComboBox TestEnum", testEnumCombo);

		// Add the string entry
		StringListEntry sle = new StringListEntry();
		sle.Values = new string [] {
			"Line 1",
			"Line 2",
			"Line 3",
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

#region Events
	/// <summary>
	/// Fired when the test enumeration changes.
	/// </summary>
	private void OnTestEnumChanged(object obj, EventArgs args)
	{
		Demo.Statusbar.Push(0,
			       "TestEnum: "
			       + testEnumCombo.ActiveEnum
			       + " ("
			       + testEnumCombo.Active
			       + ")");
	}
#endregion
}
