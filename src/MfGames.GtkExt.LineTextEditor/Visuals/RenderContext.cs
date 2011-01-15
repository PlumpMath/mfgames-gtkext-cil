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

using Cairo;

using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Visuals
{
	/// <summary>
	/// Implements a basic render context used for rendering various elements
	/// of the text editor.
	/// </summary>
	public class RenderContext : IRenderContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RenderContext"/> class.
		/// </summary>
		/// <param name="cairoContext">The cairo context.</param>
		public RenderContext(Context cairoContext)
		{
			CairoContext = cairoContext;
		}

		#endregion

		#region Render Context

		/// <summary>
		/// Gets the Cairo context for rendering.
		/// </summary>
		/// <value>The cairo context.</value>
		public Context CairoContext { get; set; }

		#endregion
	}
}