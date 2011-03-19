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

using C5;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
	/// <summary>
	/// Represents the various style elements of the text editor. This is based
	/// on CSS styles in that if a style isn't defined at this level, the
	/// parent elements are checked. This allows for simple changes at higher
	/// levels to cascade down to child elements.
	/// </summary>
	public class LineBlockStyle : BlockStyle
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LineBlockStyle"/> class.
		/// </summary>
		public LineBlockStyle()
		{
			children = new LinkedList<LineBlockStyle>();
			MarginStyles = new MarginBlockStyleCollection(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LineBlockStyle"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public LineBlockStyle(LineBlockStyle parent)
			: this()
		{
			Parent = parent;
		}

		#endregion

		#region Cascading

		private readonly C5.LinkedList<LineBlockStyle> children;
		private LineBlockStyle parent;

		/// <summary>
		/// Gets the children styles associated with this one.
		/// </summary>
		/// <value>The children.</value>
		public C5.LinkedList<LineBlockStyle> Children
		{
			[DebuggerStepThrough]
			get { return children; }
		}

		/// <summary>
		/// Gets or sets the parent selector style. Setting the parent will
		/// also establish a corresponding link in the parent's children styles
		/// into this one. Setting to null will remove any cascading.
		/// </summary>
		/// <value>The parent.</value>
		public LineBlockStyle Parent
		{
			[DebuggerStepThrough]
			get { return parent; }
			set
			{
				if (parent != null)
				{
					parent.Children.Remove(this);
				}

				parent = value;

				if (parent != null)
				{
					parent.Children.Add(this);
				}
			}
		}

		/// <summary>
		/// Gets the parent block style for this element.
		/// </summary>
		/// <value>The parent block style.</value>
		protected override BlockStyle ParentBlockStyle
		{
			get { return parent; }
		}

		#endregion

		#region Margins

		/// <summary>
		/// Gets the margin styles.
		/// </summary>
		/// <value>The margin styles.</value>
		public MarginBlockStyleCollection MarginStyles { get; private set; }

		#endregion

	}
}