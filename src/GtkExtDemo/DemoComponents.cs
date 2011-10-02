#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using Gtk;

using MfGames.GtkExt;

#endregion

namespace GtkExtDemo
{
    /// <summary>
    /// Shows off the various GtkExt components.
    /// </summary>
    public class DemoComponents : DemoTab
    {
        private readonly EnumComboBox testEnumCombo;

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
            var ecb = new EnumComboBox(typeof(ButtonsType));
            ecb.ActiveEnum = ButtonsType.YesNo;
            table.Attach(row++, "EnumComboBox", ecb);

            // An unexpanded described
            testEnumCombo = new EnumComboBox(typeof(ExampleEnumeration));
            testEnumCombo.Changed += OnTestEnumChanged;
            table.Attach(
                row++,
                "ExampleEnumeration",
                testEnumCombo);

            // Add the string entry
            var sle = new StringListEntry();
            sle.Values = new[] { "Line 1", "Line 2", "Line 3", };
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
        private void OnTestEnumChanged(
            object obj,
            EventArgs args)
        {
            DemoWindow.Statusbar.Push(
                0,
                "ExampleEnumeration: " + testEnumCombo.ActiveEnum + " (" +
                testEnumCombo.Active + ")");
        }

        #endregion
    }
}