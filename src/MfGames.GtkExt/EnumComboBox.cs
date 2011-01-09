using Gtk;
using System;
using System.ComponentModel;
using System.Reflection;

namespace MfGames.GtkExt
{
	/// <summary>
	/// Encapsulates the functionality of a specific combo box
	/// that takes a single enumeration and allows for retrieval
	/// of those types.
	/// </summary>
	public class EnumComboBox
	: ComboBox
	{
#region Constructors
		/// <summary>
		/// Constructs the combo box from the given type.
		/// </summary>
		public EnumComboBox(Type type)
		{
			// Create the model
			this.type = type;
			ListStore store = new ListStore(typeof(string),
							typeof(string));
			Model = (TreeModel) store;
			
			// Set up the rendering
			CellRendererText crt = new CellRendererText(); 
			PackStart(crt, true);
			SetAttributes(crt, "text", 1);

			// Populate the data from the fields
			foreach (string value in Enum.GetNames(type))
			{
				// Create the enumeration
				Enum e = (Enum) Enum.Parse(type, value);

				// See if we can find a fancier name
				string name = GetDescription(e);

				// Add this row
				store.AppendValues(value,
						   name);
			}

			// Set the initial active to be the first one
			Active = 0;
		}
#endregion

#region Properties
		private Type type;

		/// <summary>
		/// Sets or gets the active enumeration for this
		/// component.
		/// </summary>
		public Enum ActiveEnum
		{
			get
			{
				// Pull out the current element
				ListStore store = (ListStore) Model;
				TreeIter iter;

				if (!GetActiveIter(out iter))
					throw new Exception("Cannot handle "
							    + "missing data");

				// Parse the results and return it
				string value =
				  store.GetValue(iter, 0).ToString();
				return (Enum) Enum.Parse(type, value);
			}

			set
			{
				// Go through the elements and find
				// the string value
				string str = value.ToString();

				// Go through it
				ListStore store = (ListStore) Model;
				int index = 0;

				foreach (object [] row in store)
				{
					// Check for match
					if (str == row[0].ToString())
					{
						Active = index;
						return;
					}

					// Increment it
					index++;
				}
			}
		}
#endregion

#region Reflection
		/// <summary>
		/// Returns any fancy name, if it has any.
		/// </summary>
		private string GetDescription(Enum e)
		{
			// Try to get the Description attribute
			FieldInfo info = GetFieldInfo(e);
			Type t = typeof(DescriptionAttribute);
			object[] attributes =
			  info.GetCustomAttributes(t, true);
			
			foreach (DescriptionAttribute da in attributes)
			{
				return da.Description;
			}

			// Return the fallback name
			return e.ToString();
		}

		/// <summary>
		/// Returns the field info object for this enum.
		/// </summary>
		private FieldInfo GetFieldInfo(Enum e)
		{
			// Go through the type and find the field
			Type t = e.GetType();

			foreach (FieldInfo fi in t.GetFields())
			{
				// Check the name
				if (fi.Name == e.ToString())
					return fi;
			}
			
			// Cannot find it
			throw new Exception("Cannot find type: " + e);
		}
#endregion
	}
}
