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

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
	/// <summary>
	/// Represents the borders of all four sides.
	/// </summary>
	public class OptionalBorders
	{
		#region Borders

		/// <summary>
		/// Gets or sets the bottom border.
		/// </summary>
		/// <value>The bottom.</value>
		public Border Bottom { get; set; }

		/// <summary>
		/// Gets a value indicating whether all four directions have values.
		/// </summary>
		/// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
		public bool Complete
		{
			get { return Right != null && Left != null && Top != null && Bottom != null; }
		}

		/// <summary>
		/// Gets a value indicating whether all four directions have no values.
		/// </summary>
		/// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
		public bool Empty
		{
			get { return Right == null && Left == null && Top == null && Bottom == null; }
		}

		/// <summary>
		/// Gets or sets the left border.
		/// </summary>
		/// <value>The left.</value>
		public Border Left { get; set; }

		/// <summary>
		/// Gets or sets the right border.
		/// </summary>
		/// <value>The right.</value>
		public Border Right { get; set; }

		/// <summary>
		/// Gets or sets the top border.
		/// </summary>
		/// <value>The top.</value>
		public Border Top { get; set; }

		#endregion

		#region Conversion

		/// <summary>
		/// Converts the optional borders object to a borders object.
		/// </summary>
		/// <returns></returns>
		public Borders ToBorders()
		{
			return new Borders(
				Top ?? new Border(),
				Right ?? new Border(),
				Bottom ?? new Border(),
				Left ?? new Border());
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("Borders? T:{0} R:{1} B:{2} L:{3}", Top, Right, Bottom, Left);
		}

		#endregion
	}
}