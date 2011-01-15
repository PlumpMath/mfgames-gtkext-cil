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

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a proxy for a ILineMarkupBuffer that calls the underlying
	/// buffer for all the methods in ILineMarkupBuffer classes.
	/// </summary>
	public class LineMarkupBufferProxy : LineBufferProxy, ILineMarkupBuffer
	{
		#region Constructors

		public LineMarkupBufferProxy(ILineMarkupBuffer buffer)
			: base(buffer)
		{
		}

		#endregion

		#region Buffer

		/// <summary>
		/// Gets the line markup buffer.
		/// </summary>
		/// <value>The line markup buffer.</value>
		protected ILineMarkupBuffer LineMarkupBuffer
		{
			get
			{
				// This works since we passed in a ILineMarkupBuffer into
				// the class and ILineMarkupBuffer extends ILineBuffer.
				return (ILineMarkupBuffer) LineBuffer;
			}
		}

		#endregion

		#region Markup

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		public virtual string GetLineMarkup(int line)
		{
			return LineMarkupBuffer.GetLineMarkup(line);
		}

		/// <summary>
		/// Gets the line style for a given line.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="line">The line number.</param>
		/// <returns></returns>
		public virtual BlockStyle GetLineStyle(
			TextEditor textEditor,
			int line)
		{
			return LineMarkupBuffer.GetLineStyle(textEditor, line);
		}

		#endregion
	}
}