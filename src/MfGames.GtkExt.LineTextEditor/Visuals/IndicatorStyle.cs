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

using Cairo;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Visuals
{
	/// <summary>
	/// Encapsulates a style of drawing indicators.
	/// </summary>
	public class IndicatorStyle : IComparable<IndicatorStyle>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IndicatorStyle"/> class.
		/// </summary>
		public IndicatorStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IndicatorStyle"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="color">The color.</param>
		public IndicatorStyle(
			string name,
			int priority,
			Color color)
		{
			Name = name;
			Priority = priority;
			Color = color;
		}

		#region Properties

		/// <summary>
		/// Gets or sets the color of the indicator line.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get; set; }

		/// <summary>
		/// Gets or sets the name of the style.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the priority of the style. The higher indicators have
		/// priority over the lower ones.
		/// </summary>
		/// <value>The priority.</value>
		public int Priority { get; set; }

		#endregion

		#region Comparison

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(IndicatorStyle other)
		{
			return other.Priority.CompareTo(Priority);
		}

		#endregion
	}
}