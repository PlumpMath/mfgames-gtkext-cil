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

using Cairo;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
	/// <summary>
	/// Represents a single edge of a border.
	/// </summary>
	public class Border
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Border"/> class.
		/// </summary>
		public Border()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Border"/> class.
		/// </summary>
		/// <param name="lineWidth">Width of the line.</param>
		/// <param name="color">The color.</param>
		public Border(
			double lineWidth,
			Color color)
		{
			LineWidth = lineWidth;
			Color = color;
		}

		#endregion

		#region Borders

		/// <summary>
		/// Gets or sets the color of the border.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get; set; }

		/// <summary>
		/// Gets or sets the width of the border.
		/// </summary>
		/// <value>The width.</value>
		public double LineWidth { get; set; }

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
			return string.Format("Border {0} {1}", LineWidth, Color.ToRgbHexString());
		}

		#endregion
	}
}