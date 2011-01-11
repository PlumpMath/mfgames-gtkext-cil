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

using C5;

using Pango;

using Color=Cairo.Color;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Visuals
{
	/// <summary>
	/// Represents the various style elements of the text editor. This is based
	/// on CSS styles in that if a style isn't defined at this level, the
	/// parent elements are checked. This allows for simple changes at higher
	/// levels to cascade down to child elements.
	/// </summary>
	public class SelectorStyle
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectorStyle"/> class.
		/// </summary>
		public SelectorStyle()
		{
			children = new LinkedList<SelectorStyle>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectorStyle"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public SelectorStyle(SelectorStyle parent)
			: this()
		{
			Parent = parent;
		}

		#endregion

		#region Cascading

		private readonly LinkedList<SelectorStyle> children;
		private SelectorStyle parent;

		/// <summary>
		/// Gets the children styles associated with this one.
		/// </summary>
		/// <value>The children.</value>
		public LinkedList<SelectorStyle> Children
		{
			get { return children; }
		}

		/// <summary>
		/// Gets or sets the parent selector style. Setting the parent will
		/// also establish a corresponding link in the parent's children styles
		/// into this one. Setting to null will remove any cascading.
		/// </summary>
		/// <value>The parent.</value>
		public SelectorStyle Parent
		{
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

		#endregion

		#region Colors

		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color? BackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the foreground color.
		/// </summary>
		/// <value>The color of the foreground.</value>
		public Color? ForegroundColor { get; set; }

		/// <summary>
		/// Gets the background color from this selector or the parent.
		/// </summary>
		/// <returns></returns>
		public Color GetBackgroundColor()
		{
			// If we have a value, then use that directly.
			if (BackgroundColor.HasValue)
			{
				return BackgroundColor.Value;
			}

			// If we have a parent, then cascade up into it.
			if (parent != null)
			{
				return parent.GetBackgroundColor();
			}

			// Otherwise, return a sane default.
			return new Color(1, 1, 1);
		}

		/// <summary>
		/// Gets the foreground color from this selector or the parent.
		/// </summary>
		/// <returns></returns>
		public Color GetForegroundColor()
		{
			// If we have a value, then use that directly.
			if (ForegroundColor.HasValue)
			{
				return ForegroundColor.Value;
			}

			// If we have a parent, then cascade up into it.
			if (parent != null)
			{
				return parent.GetForegroundColor();
			}

			// Otherwise, return a sane default.
			return new Color(0, 0, 0);
		}

		#endregion

		#region Text

		/// <summary>
		/// Gets or sets the text alignment.
		/// </summary>
		/// <value>The alignment.</value>
		public Alignment? Alignment { get; set; }

		/// <summary>
		/// Gets or sets the font description.
		/// </summary>
		/// <value>The font description.</value>
		public FontDescription FontDescription { get; set; }

		/// <summary>
		/// Gets or sets the wrap mode.
		/// </summary>
		/// <value>The wrap mode.</value>
		public WrapMode? WrapMode { get; set; }

		/// <summary>
		/// Gets the alignment from this selector or the parent.
		/// </summary>
		/// <returns></returns>
		public Alignment GetAlignment()
		{
			// If we have a value, then use that directly.
			if (Alignment.HasValue)
			{
				return Alignment.Value;
			}

			// If we have a parent, then cascade up into it.
			if (parent != null)
			{
				return parent.GetAlignment();
			}

			// Otherwise, return a sane default.
			return Pango.Alignment.Left;
		}

		/// <summary>
		/// Gets the font description from this selector or the parent.
		/// </summary>
		/// <returns></returns>
		public FontDescription GetFontDescription()
		{
			// If we have a value, then use that directly.
			if (FontDescription != null)
			{
				return FontDescription;
			}

			// If we have a parent, then cascade up into it.
			if (parent != null)
			{
				return parent.GetFontDescription();
			}

			// Otherwise, return a sane default.
			return FontDescription.FromString("Sans 12");
		}

		/// <summary>
		/// Gets the wrap mode from this selector or the parent.
		/// </summary>
		/// <returns></returns>
		public WrapMode GetWrap()
		{
			// If we have a value, then use that directly.
			if (WrapMode.HasValue)
			{
				return WrapMode.Value;
			}

			// If we have a parent, then cascade up into it.
			if (parent != null)
			{
				return parent.GetWrap();
			}

			// Otherwise, return a sane default.
			return Pango.WrapMode.Word;
		}

		/// <summary>
		/// Sets the selector's style properties into the layout.
		/// </summary>
		/// <param name="layout">The layout.</param>
		public void SetLayout(Layout layout)
		{
			layout.Wrap = GetWrap();
			layout.Alignment = GetAlignment();
			layout.FontDescription = GetFontDescription();
		}

		#endregion
	}
}