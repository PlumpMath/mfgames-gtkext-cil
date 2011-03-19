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
using System.Diagnostics;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
	/// <summary>
	/// Represents a style for rendering the margin.
	/// </summary>
	public class MarginBlockStyle : BlockStyle
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MarginBlockStyle"/> class.
		/// </summary>
		/// <param name="styleName">Name of the style.</param>
		/// <param name="parentLineStyle">The parent line style.</param>
		public MarginBlockStyle(
			string styleName,
			LineBlockStyle parentLineStyle)
		{
			if (styleName == null)
			{
				throw new ArgumentNullException("styleName");
			}

			this.styleName = styleName;
			this.parentLineStyle = parentLineStyle;
		}

		#endregion

		#region Relationships

		private readonly LineBlockStyle parentLineStyle;
		private readonly string styleName;
		private MarginBlockStyle parentMarginStyle;

		/// <summary>
		/// Gets the ParentBlockStyle block style for this element.
		/// </summary>
		/// <value>The ParentBlockStyle block style.</value>
		protected override BlockStyle ParentBlockStyle
		{
			get
			{
				// If we have a margin style, we use that.
				if (parentMarginStyle != null)
				{
					return parentMarginStyle;
				}

				// If we don't have a parent line style, then return null.
				if (parentLineStyle == null)
				{
					return null;
				}

				// Otherwise, return the parent's line number.
				return parentLineStyle.MarginStyles.GetParent(styleName);
			}
		}

		/// <summary>
		/// Gets the line style associated with this margin style.
		/// </summary>
		/// <value>The line style.</value>
		public LineBlockStyle ParentLineStyle
		{
			[DebuggerStepThrough]
			get { return parentLineStyle; }
		}

		/// <summary>
		/// Gets the parent margin style.
		/// </summary>
		/// <value>The margin style.</value>
		public MarginBlockStyle ParentMarginStyle
		{
			[DebuggerStepThrough]
			get { return parentMarginStyle; }
			[DebuggerStepThrough]
			set { parentMarginStyle = value; }
		}

		/// <summary>
		/// Gets the name of the style.
		/// </summary>
		/// <value>The name of the style.</value>
		public string StyleName
		{
			get { return styleName; }
		}

		#endregion
	}
}