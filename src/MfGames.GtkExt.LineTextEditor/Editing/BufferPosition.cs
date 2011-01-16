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

namespace MfGames.GtkExt.LineTextEditor.Editing
{
	/// <summary>
	/// Represents a position within the text buffer using the line as a primary
	/// and the character within the line's text.
	/// </summary>
	public struct BufferPosition
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BufferPosition"/> struct.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="character">The character.</param>
		public BufferPosition(
			int line,
			int character)
			: this()
		{
			Line = line;
			Character = character;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the character.
		/// </summary>
		/// <value>The character.</value>
		public int Character { get; set; }

		/// <summary>
		/// Gets or sets the line.
		/// </summary>
		/// <value>The line.</value>
		public int Line { get; set; }

		#endregion

		#region Coordinates

		/// <summary>
		/// Converts the given line and character coordinates into pixel coordinates
		/// on the display.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns></returns>
		public PointD ToScreenCoordinates(IDisplayContext displayContext)
		{
			// Figure out the top of the current line.
			int y = displayContext.LineLayoutBuffer.GetLineLayoutHeight(
				displayContext, 0, Line);

			// Return the results.
			return new PointD(100, 100);
		}

		#endregion
	}
}