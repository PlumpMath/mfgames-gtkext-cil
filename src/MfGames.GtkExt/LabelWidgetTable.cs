using Gtk;
using System;

namespace MfGames.GtkExt
{
	/// <summary>
	/// Extends the table to handle the common formatting of a label and
	/// widget layout where the layout is two columns.
	/// </summary>
	public class LabelWidgetTable
	: Table
	{
#region Constructors
		/// <summary>
		/// Constructs the number of rows and columns. As a note, this
		/// generates double the number of columns.
		/// </summary>
		public LabelWidgetTable(uint rows, uint cols)
			: base(rows, cols, false)
		{
			// Sanity checking
			if (cols <= 1)
				throw new Exception("Cannot create a LabelWidgetTable "
					+ "with less than"
					+ " two columns");

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
		public void Attach(uint row, string label,
			params Widget [] widgets)
		{
			// Wrap the widgets to expand it
			Widget [] boxes = new Widget [widgets.Length];

			for (int i = 0; i < widgets.Length; i++)
			{
				HBox box = new HBox();
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
		public void AttachExpanded(uint row, string label,
			params Widget [] widgets)
		{
			// Build up the label
			if (label == null)
				label = "";

			if (label != "")
				label += ":";

			Label l = new Label();
			l.Markup = "<b>" + label + "</b>";
			l.Xalign = 1.0f;

			// Attach it
			Attach(l,
				0, 1, row, row + 1,
				AttachOptions.Fill,
				AttachOptions.Fill,
				0, 0);

			// Attach the rest of the widgets
			for (uint i = 0; i < widgets.Length; i++)
			{
				Attach(widgets[i],
					i + 1, i + 2, row, row + 1,
					AttachOptions.Fill | AttachOptions.Expand,
					AttachOptions.Fill | AttachOptions.Expand,
					0, 0);
			}
		}
#endregion
	}
}
