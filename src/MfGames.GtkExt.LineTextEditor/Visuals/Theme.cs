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

using MfGames.GtkExt.LineTextEditor.Enumerations;

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
		public const string CurrentLineNumberStyle = "CurrentLineNumber";
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

			var marginStyle = new BlockStyle(baseStyle);

			var lineNumberStyle = new BlockStyle(marginStyle);
			lineNumberStyle.Alignment = Alignment.Right;
			lineNumberStyle.BackgroundColor = new Color(0.9, 0.9, 0.9);
			lineNumberStyle.ForegroundColor = new Color(0.5, 0.5, 0.5);
			lineNumberStyle.Borders.Right = new Border(1, new Color(0.5, 0.5, 0.5));
			lineNumberStyle.Padding.Right = 4;
			lineNumberStyle.Padding.Left = 4;
			lineNumberStyle.Margins.Right = 0;
			lineNumberStyle.Margins.Top = 4;
			lineNumberStyle.Margins.Bottom = 4;

			var currentLineNumberStyle = new BlockStyle(lineNumberStyle);
			currentLineNumberStyle.BackgroundColor = new Color(0.8, 0.8, 0.8);
			currentLineNumberStyle.ForegroundColor = new Color(0.0, 0.0, 0.0);

			var textStyle = new BlockStyle(baseStyle);
			textStyle.Margins.Top = 4;
			textStyle.Margins.Bottom = 4;
			textStyle.Margins.Left = 8;

			blockStyles[BaseStyle] = baseStyle;
			blockStyles[MarginStyle] = marginStyle;
			blockStyles[LineNumberStyle] = lineNumberStyle;
			blockStyles[CurrentLineNumberStyle] = currentLineNumberStyle;
			blockStyles[TextStyle] = textStyle;

			// Colors
			BackgroundColor = new Color(1, 1, 1);
			IndicatorBackgroundColor = new Color(1, 1, 1);
			CurrentLineBackgroundColor = new Color(255 / 255.0, 250 / 255.0, 205 / 255.0);
			CurrentWrappedLineBackgroundColor = new Color(
				238 / 255.0, 233 / 255.0, 191 / 255.0);

			// Indicator styles.
			indicatorStyles = new HashDictionary<string, IndicatorStyle>();
		}

		#endregion

		#region Colors

		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color BackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the background color for the currently selected line.
		/// </summary>
		/// <value>The color of the current line background.</value>
		public Color? CurrentLineBackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the background color of the current positioned wrapped
		/// line.
		/// </summary>
		/// <value>The color of the current wrapped line background.</value>
		public Color? CurrentWrappedLineBackgroundColor { get; set; }

		/// <summary>
		/// Gets or sets the background color of an indicator bar.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color IndicatorBackgroundColor { get; set; }

		#endregion

		#region Block Styles

		private readonly HashDictionary<string, BlockStyle> blockStyles;

		/// <summary>
		/// Gets the selector styles.
		/// </summary>
		/// <value>The selectors.</value>
		public HashDictionary<string, BlockStyle> BlockStyles
		{
			get { return blockStyles; }
		}

		/// <summary>
		/// Gets the current line number block style.
		/// </summary>
		/// <value>The line number block style.</value>
		public BlockStyle CurrentLineNumberBlockStyle
		{
			get { return blockStyles[CurrentLineNumberStyle]; }
		}

		/// <summary>
		/// Gets the line number block style.
		/// </summary>
		/// <value>The line number block style.</value>
		public BlockStyle LineNumberBlockStyle
		{
			get { return blockStyles[LineNumberStyle]; }
		}

		/// <summary>
		/// Gets the text block style.
		/// </summary>
		/// <value>The text block style.</value>
		public BlockStyle TextBlockStyle
		{
			get { return blockStyles[TextStyle]; }
		}

		#endregion

		#region Indicator Styles

		private readonly HashDictionary<string, IndicatorStyle> indicatorStyles;

		/// <summary>
		/// Gets or sets the indicator render style.
		/// </summary>
		/// <value>The indicator render style.</value>
		public IndicatorRenderStyle IndicatorRenderStyle { get; set; }

		/// <summary>
		/// Gets the indicator styles in this theme.
		/// </summary>
		/// <value>The indicator styles.</value>
		public HashDictionary<string, IndicatorStyle> IndicatorStyles
		{
			get { return indicatorStyles; }
		}

		#endregion
	}
}