#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
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

using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

#endregion

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