using System;

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
	/// <summary>
	/// Implements a region style for rendering.
	/// </summary>
	public class RegionBlockStyle : BlockStyle
	{
		#region Relationship

		/// <summary>
		/// Gets the ParentBlockStyle block style for this element.
		/// </summary>
		/// <value>
		/// The ParentBlockStyle block style.
		/// </value>
		protected override BlockStyle ParentBlockStyle
		{
			get { return null; }
		}

		#endregion
	}
}