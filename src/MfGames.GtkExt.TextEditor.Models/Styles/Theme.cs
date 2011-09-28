#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using Color = Cairo.Color;
using IDictionary = System.Collections.Generic;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Styles
{
    /// <summary>
    /// Contains the screen style used for rendering the various elements of the
    /// text editor.
    /// </summary>
    public class Theme
    {
        #region Constants

        /// <summary>
        /// The name of the background region style.
        /// </summary>
        public const string BackgroundRegionStyleName = "EditorViewBackground";

        /// <summary>
        /// Contains the name of the base style.
        /// </summary>
        public const string BaseStyleName = "Base";

        /// <summary>
        /// The name of the style for rendering the current line.
        /// </summary>
        public const string CurrentLineRegionStyleName = "EditorViewCurrentLine";

        /// <summary>
        /// The name of the style for rendering the current wrapped line.
        /// </summary>
        public const string CurrentWrappedLineRegionStyleName =
            "EditorViewCurrentWrappedLine";

        /// <summary>
        /// The name of the line number renderer style.
        /// </summary>
        public const string LineNumberStyle = "LineNumber";

        /// <summary>
        /// The name of the parent style for all margin renderers.
        /// </summary>
        public const string MarginStyle = "Margin";

        /// <summary>
        /// The name for the default text style.
        /// </summary>
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
            lineStyles = new BlockStyleDictionary<LineBlockStyle>();

            // Set up the base style as a fallback for everything.
            var baseStyle = new LineBlockStyle();
            baseStyle.MarginStyles.Add(LineNumberStyle);

            var marginStyle = new LineBlockStyle(baseStyle);

            // Set the default text style.
            var textStyle = new LineBlockStyle(baseStyle);
            textStyle.Margins.Top = 4;
            textStyle.Margins.Bottom = 4;
            textStyle.Margins.Left = 8;

            var lineNumberStyle = textStyle.MarginStyles.Add(LineNumberStyle);
            lineNumberStyle.Alignment = Alignment.Right;
            lineNumberStyle.BackgroundColor = new Color(0.9, 0.9, 0.9);
            lineNumberStyle.ForegroundColor = new Color(0.5, 0.5, 0.5);
            lineNumberStyle.Borders.Right = new Border(
                1, new Color(0.5, 0.5, 0.5));
            lineNumberStyle.Padding.Right = 4;
            lineNumberStyle.Padding.Left = 4;
            lineNumberStyle.Padding.Top = 4;
            lineNumberStyle.Padding.Bottom = 4;
            lineNumberStyle.Margins.Right = 0;

            // Store the styles in the theme.
            lineStyles[BaseStyleName] = baseStyle;
            lineStyles[MarginStyle] = marginStyle;
            lineStyles[TextStyle] = textStyle;

            // Common region styles for the editor.
            regionStyles = new BlockStyleDictionary<RegionBlockStyle>();

            var backgroundRegionStyle = new RegionBlockStyle();
            backgroundRegionStyle.BackgroundColor = new Color(1, 1, 1);
            regionStyles[BackgroundRegionStyleName] = backgroundRegionStyle;

            var currentLineRegionStyle = new RegionBlockStyle();
            currentLineRegionStyle.BackgroundColor = new Color(
                255 / 255.0, 250 / 255.0, 205 / 255.0);
            regionStyles[CurrentLineRegionStyleName] = currentLineRegionStyle;

            var currentWrappedLineRegionStyle = new RegionBlockStyle();
            currentWrappedLineRegionStyle.BackgroundColor =
                new Color(238 / 255.0, 233 / 255.0, 191 / 255.0);
            regionStyles[CurrentWrappedLineRegionStyleName] =
                currentWrappedLineRegionStyle;

            // Indicator styles.
            indicatorStyles = new HashDictionary<string, IndicatorStyle>();
        }

        #endregion

        #region Region Styles

        private readonly BlockStyleDictionary<RegionBlockStyle> regionStyles;

        /// <summary>
        /// Gets the region styles.
        /// </summary>
        public IDictionary<string, RegionBlockStyle> RegionStyles
        {
            get { return regionStyles; }
        }

        #endregion

        #region Line Styles

        private readonly BlockStyleDictionary<LineBlockStyle> lineStyles;

        /// <summary>
        /// Gets the selector styles.
        /// </summary>
        /// <value>The selectors.</value>
        public IDictionary<string, LineBlockStyle> LineStyles
        {
            get { return lineStyles; }
        }

        /// <summary>
        /// Gets the text block style.
        /// </summary>
        /// <value>The text block style.</value>
        public LineBlockStyle TextLineStyle
        {
            get { return lineStyles[TextStyle]; }
        }

        #endregion

        #region Indicator Styles

        private readonly HashDictionary<string, IndicatorStyle> indicatorStyles;

        /// <summary>
        /// Gets or sets the pixel height of the indicator lines.
        /// </summary>
        /// <value>The height of the indicator pixel.</value>
        public int IndicatorPixelHeight { get; set; }

        /// <summary>
        /// Gets or sets the pixel gap between multiple indicators.
        /// </summary>
        /// <value>The indicator ratio pixel gap.</value>
        public int IndicatorRatioPixelGap { get; set; }

        /// <summary>
        /// Gets or sets the indicator render style.
        /// </summary>
        /// <value>The indicator render style.</value>
        public IndicatorRenderStyle IndicatorRenderStyle { get; set; }

        /// <summary>
        /// Gets the indicator styles in this theme.
        /// </summary>
        /// <value>The indicator styles.</value>
        public IDictionary<string, IndicatorStyle> IndicatorStyles
        {
            get { return indicatorStyles; }
        }

        #endregion
    }
}