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

#endregion

namespace MfGames.GtkExt
{
    /// <summary>
    /// Extends the table to handle the common formatting of a label and
    /// widget layout where the layout is two columns.
    /// </summary>
    public class LabelWidgetTable : Table
    {
        #region Constructors

        /// <summary>
        /// Constructs the number of rows and columns. As a note, this
        /// generates double the number of columns.
        /// </summary>
        public LabelWidgetTable(
            uint rows,
            uint cols)
            : base(rows, cols, false)
        {
            // Sanity checking
            if (cols <= 1)
            {
                throw new Exception(
                    "Cannot create a LabelWidgetTable " + "with less than" +
                    " two columns");
            }

            // Set some initial values
            RowSpacing = 5;
            ColumnSpacing = 5;
        }

        #endregion

        #region Rows

        /// <summary>
        /// Attachs a row into the table, formatting the label properly to
        /// the name (and adding a ":" as needed). If the label is blank
        /// or null, it does not format it.
        /// </summary>
        public void Attach(
            uint row,
            string label,
            params Widget[] widgets)
        {
            // Wrap the widgets to expand it
            var boxes = new Widget[widgets.Length];

            for (int i = 0; i < widgets.Length; i++)
            {
                var box = new HBox();
                box.PackStart(widgets[i], false, false, 0);
                box.PackStart(new Label(), true, true, 0);
                boxes[i] = box;
            }

            // Add the row
            AttachExpanded(row, label, boxes);
        }

        /// <summary>
        /// Attaches a row into the table, but expands it to fill the
        /// value.
        /// </summary>
        public void AttachExpanded(
            uint row,
            string label,
            params Widget[] widgets)
        {
            // Build up the label
            if (label == null)
            {
                label = "";
            }

            if (label != "")
            {
                label += ":";
            }

            var l = new Label();
            l.Markup = "<b>" + label + "</b>";
            l.Xalign = 1.0f;

            // Attach it
            Attach(
                l,
                0,
                1,
                row,
                row + 1,
                AttachOptions.Fill,
                AttachOptions.Fill,
                0,
                0);

            // Attach the rest of the widgets
            for (uint i = 0; i < widgets.Length; i++)
            {
                Attach(
                    widgets[i],
                    i + 1,
                    i + 2,
                    row,
                    row + 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Fill | AttachOptions.Expand,
                    0,
                    0);
            }
        }

        #endregion
    }
}