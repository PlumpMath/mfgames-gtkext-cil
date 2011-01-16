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
	/// Contains the screen style used for rendering the various elements of the
	/// text editor.
	/// </summary>
	public class Theme
	{
		#region Constants

		public const string BaseStyle = "Base";
		public const string BodyStyle = "Body";
		public const string LineNumberStyle = "LineNumber";
		public const string MarginStyle = "Margin";
		public const string TextStyle = "Text";

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Theme"/> class.
		/// </summary>
		public Theme()
		{
			// Create the initial styles along with the styles used internally
			// by the text editor.
			blockStyles = new HashDictionary<string, BlockStyle>();

			var baseStyle = new BlockStyle();

			var bodyStyle = new BlockStyle(baseStyle);
			bodyStyle.BackgroundColor = new Color(1, 1, 1);

			var marginStyle = new BlockStyle(baseStyle);

			var lineNumberStyle = new BlockStyle(marginStyle);
			lineNumberStyle.Alignment = Alignment.Right;
			lineNumberStyle.BackgroundColor = new Color(0.9, 0.9, 0.9);
			lineNumberStyle.ForegroundColor = new Color(0.5, 0.5, 0.5);
			lineNumberStyle.Borders.Right = new Border(1, new Color(0.5, 0.5, 0.5));
			lineNumberStyle.Padding.Right = 2;
			lineNumberStyle.Padding.Left = 4;
			lineNumberStyle.Margins.Right = 8;

			var textStyle = new BlockStyle(baseStyle);
			textStyle.Margins.Top = 8;
			textStyle.Margins.Bottom = 8;

			blockStyles[BaseStyle] = baseStyle;
			blockStyles[BodyStyle] = bodyStyle;
			blockStyles[MarginStyle] = marginStyle;
			blockStyles[LineNumberStyle] = lineNumberStyle;
			blockStyles[TextStyle] = textStyle;
		}

		#endregion

		#region Styles

		private readonly HashDictionary<string, BlockStyle> blockStyles;

		/// <summary>
		/// Gets the selector styles.
		/// </summary>
		/// <value>The selectors.</value>
		public HashDictionary<string, BlockStyle> BlockStyles
		{
			get { return blockStyles; }
		}

		#endregion
	}
}