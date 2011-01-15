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
	public class BlockStyle
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockStyle"/> class.
		/// </summary>
		public BlockStyle()
		{
			children = new LinkedList<BlockStyle>();
			margins = new OptionalSpacing();
			padding = new OptionalSpacing();
			borders = new OptionalBorders();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockStyle"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public BlockStyle(BlockStyle parent)
			: this()
		{
			Parent = parent;
		}

		#endregion

		#region Cascading

		private readonly LinkedList<BlockStyle> children;
		private BlockStyle parent;

		/// <summary>
		/// Gets the children styles associated with this one.
		/// </summary>
		/// <value>The children.</value>
		public LinkedList<BlockStyle> Children
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
		public BlockStyle Parent
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
		/// Gets or sets the style usage.
		/// </summary>
		/// <value>The style usage.</value>
		public StyleUsage StyleUsage { get; set; }

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

		#region Spacing and Borders

		private OptionalBorders borders;
		private OptionalSpacing margins;
		private OptionalSpacing padding;

		/// <summary>
		/// Gets or sets the borders.
		/// </summary>
		/// <value>The borders.</value>
		public OptionalBorders Borders
		{
			[DebuggerStepThrough]
			get { return borders; }
			[DebuggerStepThrough]
			set { borders = value ?? new OptionalBorders(); }
		}

		/// <summary>
		/// Gets the height of the various elements in the style.
		/// </summary>
		/// <value>The height.</value>
		public int Height
		{
			get { return (int) (GetMargins().Height + GetPadding().Height); }
		}

		/// <summary>
		/// Gets or sets the margins.
		/// </summary>
		/// <value>The margins.</value>
		public OptionalSpacing Margins
		{
			[DebuggerStepThrough]
			get { return margins; }
			set { margins = value ?? new OptionalSpacing(); }
		}

		/// <summary>
		/// Gets or sets the padding.
		/// </summary>
		/// <value>The padding.</value>
		public OptionalSpacing Padding
		{
			[DebuggerStepThrough]
			get { return padding; }
			set { padding = value ?? new OptionalSpacing(); }
		}

		/// <summary>
		/// Gets the width of the various elements in the style.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return (int) (GetMargins().Width + GetPadding().Width); }
		}

		/// <summary>
		/// Gets the completed borders by processing the parents.
		/// </summary>
		/// <returns></returns>
		public Borders GetBorders()
		{
			// If we have all four borders, then return them.
			if (borders.Complete)
			{
				return borders.ToBorders();
			}

			// If the current is empty, then just get the parent.
			if (borders.Empty)
			{
				return parent != null ? parent.GetBorders() : new Borders();
			}

			// If we don't have a parent, then we set the rest of the values
			// to zero and return it.
			if (parent == null)
			{
				return borders.ToBorders();
			}

			// If we have a parent, then we need to merge all the values
			// together.
			Borders parentBorders = parent.GetBorders();

			return new Borders(
				borders.Top ?? parentBorders.Top,
				borders.Right ?? parentBorders.Right,
				borders.Bottom ?? parentBorders.Bottom,
				borders.Left ?? parentBorders.Left);
		}

		/// <summary>
		/// Gets the completed margins by processing the parents.
		/// </summary>
		/// <returns></returns>
		public Spacing GetMargins()
		{
			// If we have all four margins, then return them.
			if (margins.Complete)
			{
				return margins.ToSpacing();
			}

			// If the current is empty, then just get the parent.
			if (margins.Empty)
			{
				return parent != null ? parent.GetMargins() : new Spacing();
			}

			// If we don't have a parent, then we set the rest of the values
			// to zero and return it.
			if (parent == null)
			{
				return margins.ToSpacing();
			}

			// If we have a parent, then we need to merge all the values
			// together.
			Spacing parentMargins = parent.GetMargins();

			return
				new Spacing(
					margins.Top.HasValue ? margins.Top.Value : parentMargins.Top,
					margins.Right.HasValue ? margins.Right.Value : parentMargins.Right,
					margins.Bottom.HasValue ? margins.Bottom.Value : parentMargins.Bottom,
					margins.Left.HasValue ? margins.Left.Value : parentMargins.Left);
		}

		/// <summary>
		/// Gets the completed paddings by processing the parents.
		/// </summary>
		/// <returns></returns>
		public Spacing GetPadding()
		{
			// If we have all four padding, then return them.
			if (padding.Complete)
			{
				return padding.ToSpacing();
			}

			// If the current is empty, then just get the parent.
			if (padding.Empty)
			{
				return parent != null ? parent.GetPadding() : new Spacing();
			}

			// If we don't have a parent, then we set the rest of the values
			// to zero and return it.
			if (parent == null)
			{
				return padding.ToSpacing();
			}

			// If we have a parent, then we need to merge all the values
			// together.
			Spacing parentPadding = parent.GetPadding();

			return
				new Spacing(
					padding.Top.HasValue ? padding.Top.Value : parentPadding.Top,
					padding.Right.HasValue ? padding.Right.Value : parentPadding.Right,
					padding.Bottom.HasValue ? padding.Bottom.Value : parentPadding.Bottom,
					padding.Left.HasValue ? padding.Left.Value : parentPadding.Left);
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

		#endregion
	}
}