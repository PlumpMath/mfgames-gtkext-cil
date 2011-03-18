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

using System.Diagnostics;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Models.Styles
{
	/// <summary>
	/// Represents the borders of all four sides.
	/// </summary>
	public class Borders
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Borders"/> class.
		/// </summary>
		public Borders()
		{
			Top = Right = Bottom = Left = new Border();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Borders"/> class.
		/// </summary>
		/// <param name="top">The top.</param>
		/// <param name="right">The right.</param>
		/// <param name="bottom">The bottom.</param>
		/// <param name="left">The left.</param>
		public Borders(
			Border top,
			Border right,
			Border bottom,
			Border left)
		{
			Top = top;
			Right = right;
			Bottom = bottom;
			Left = left;
		}

		#endregion

		#region Borders

		private Border bottom;
		private Border left;
		private Border right;
		private Border top;

		/// <summary>
		/// Gets or sets the bottom border.
		/// </summary>
		/// <value>The bottom.</value>
		public Border Bottom
		{
			[DebuggerStepThrough]
			get { return bottom; }
			set { bottom = value ?? new Border(); }
		}

		/// <summary>
		/// Gets the sum of the top and bottom spacing.
		/// </summary>
		/// <value>The height.</value>
		public double Height
		{
			get { return Top.LineWidth + Bottom.LineWidth; }
		}

		/// <summary>
		/// Gets or sets the left border.
		/// </summary>
		/// <value>The left.</value>
		public Border Left
		{
			[DebuggerStepThrough]
			get { return left; }
			set { left = value ?? new Border(); }
		}

		/// <summary>
		/// Gets or sets the right border.
		/// </summary>
		/// <value>The right.</value>
		public Border Right
		{
			[DebuggerStepThrough]
			get { return right; }
			set { right = value ?? new Border(); }
		}

		/// <summary>
		/// Gets or sets the top border.
		/// </summary>
		/// <value>The top.</value>
		public Border Top
		{
			[DebuggerStepThrough]
			get { return top; }
			set { top = value ?? new Border(); }
		}

		/// <summary>
		/// Gets the sum of the left and right spacing.
		/// </summary>
		/// <value>The width.</value>
		public double Width
		{
			get { return Right.LineWidth + Left.LineWidth; }
		}

		#endregion
	}
}