// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace GtkExtDemo
{
	/// <summary>
	/// Represents the common functionality of all the demo notebook tab.
	/// </summary>
	public abstract class DemoTab: VBox
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoTab"/> class.
		/// </summary>
		protected DemoTab()
		{
			BorderWidth = 12;
		}

		#endregion
	}
}
