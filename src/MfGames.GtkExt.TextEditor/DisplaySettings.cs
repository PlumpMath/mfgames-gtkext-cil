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

		public int CaretScrollPad { get; set; }
		public bool ShowLineNumbers { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the scroll padding area should be visible.
		/// </summary>
		/// <value>
		///   <c>true</c> if [show scroll padding]; otherwise, <c>false</c>.
		/// </value>
		public bool ShowScrollPadding { get; set; }

		#endregion
	}
}