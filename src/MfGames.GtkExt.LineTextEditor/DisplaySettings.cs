using System;

namespace MfGames.GtkExt.LineTextEditor
{
    /// <summary>
    /// Contains the settings for the text editor.
    /// </summary>
    public class DisplaySettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplaySettings"/> class.
        /// </summary>
        public DisplaySettings()
        {
            ShowLineNumbers = true;
            CaretScrollPad = 3;
            ShowScrollPadding = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the scroll padding area should be visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show scroll padding]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowScrollPadding
        {
            get; set;
        }

        public bool ShowLineNumbers { get; set; }

        public int CaretScrollPad { get; set; }

        #endregion
    }
}