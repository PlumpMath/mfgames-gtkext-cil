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

using C5;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
	/// <summary>
	/// Contains a list of margin block styles.
	/// </summary>
	public class MarginBlockStyleCollection : ArrayList<MarginBlockStyle>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MarginBlockStyleCollection"/> class.
		/// </summary>
		/// <param name="lineStyle">The line style.</param>
		public MarginBlockStyleCollection(LineBlockStyle lineStyle)
		{
			if (lineStyle == null)
			{
				throw new ArgumentNullException("lineStyle");
			}

			this.lineStyle = lineStyle;
		}

		#endregion

		#region Relationship

		private readonly LineBlockStyle lineStyle;

		/// <summary>
		/// Creates a new margin block style and returns it.
		/// </summary>
		/// <param name="styleName">Name of the style.</param>
		/// <returns></returns>
		public MarginBlockStyle Add(string styleName)
		{
			// Check for nulls.
			if (styleName == null)
			{
				throw new ArgumentNullException("styleName");
			}

			// See if we already have it.
			foreach (MarginBlockStyle marginStyle in this)
			{
				if (marginStyle.StyleName == styleName)
				{
					return marginStyle;
				}
			}

			// Create a new one and return it.
			var style = new MarginBlockStyle(styleName, lineStyle);

			Add(style);

			return style;
		}

		/// <summary>
		/// Gets the margin block style, moving up in parent if it isn't local.
		/// </summary>
		/// <param name="styleName">Name of the style.</param>
		/// <returns></returns>
		public MarginBlockStyle Get(string styleName)
		{
			// Look through our styles for the name.
			foreach (MarginBlockStyle marginStyle in this)
			{
				if (marginStyle.StyleName == styleName)
				{
					return marginStyle;
				}
			}

			// We couldn't find it, so check the parent.
			return GetParent(styleName);
		}

		/// <summary>
		/// Gets the style from the parent.
		/// </summary>
		/// <param name="styleName">Name of the style.</param>
		/// <returns></returns>
		public MarginBlockStyle GetParent(string styleName)
		{
			LineBlockStyle parentLineStyle = lineStyle.Parent;

			return parentLineStyle == null
			       	? null
			       	: parentLineStyle.MarginStyles.Get(styleName);
		}

		#endregion
	}
}