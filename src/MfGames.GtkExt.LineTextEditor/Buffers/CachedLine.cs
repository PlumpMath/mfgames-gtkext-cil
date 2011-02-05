using System;

using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Contains information about a single cached line in memory. This
	/// contains information about the height and style for a given line.
	/// </summary>
	internal class CachedLine
	{
		#region Properties

		/// <summary>
		/// Gets or sets the height of the line.
		/// </summary>
		/// <value>The height.</value>
		public int Height { get; set; }

		/// <summary>
		/// Gets or sets the Pango layout for the line.
		/// </summary>
		/// <value>The layout.</value>
		public Layout Layout { get; set; }

		/// <summary>
		/// Gets or sets the style for the line.
		/// </summary>
		/// <value>The style.</value>
		public BlockStyle Style { get; set; }

		/// <summary>
		/// Resets the cached line.
		/// </summary>
		public void Reset()
		{
			Height = 0;
			Style = null;
			Layout = null;
		}

		#endregion

		#region Conversion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return String.Format("CachedLine: Height={0}", Height);
		}

		#endregion
	}
}