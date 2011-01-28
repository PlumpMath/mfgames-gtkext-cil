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

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Creates a line markup that does no formatting on a inner line object.
	/// </summary>
	public class UnformattedLineMarkupBuffer : LineBufferProxy, ILineMarkupBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UnformattedLineMarkupBuffer"/> class.
		/// </summary>
		/// <param name="lineBuffer">The line buffer.</param>
		public UnformattedLineMarkupBuffer(ILineBuffer lineBuffer)
			: base(lineBuffer)
		{
		}

		#endregion

		#region Markup

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">The line.</param>
		/// <returns></returns>
		public string GetLineMarkup(
			IDisplayContext displayContext,
			int lineIndex)
		{
			string text = GetLineText(lineIndex, 0, Int32.MaxValue);

			return PangoUtility.Escape(text);
		}

		/// <summary>
		/// Gets the line style for a given line.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="lineIndex">The line number.</param>
		/// <returns></returns>
		public BlockStyle GetLineStyle(
			IDisplayContext displayContext,
			int lineIndex)
		{
			return displayContext.Theme.TextBlockStyle;
		}

		#endregion
	}
}