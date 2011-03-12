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
using MfGames.GtkExt.LineTextEditor.Renderers;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

using Rectangle=Pango.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
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
		{
			lineIndex = line;
			characterIndex = character;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BufferPosition"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		public BufferPosition(BufferPosition position)
		{
			lineIndex = position.LineIndex;
			characterIndex = position.CharacterIndex;
		}

		#endregion

		#region Properties

		private int characterIndex;

		private int lineIndex;

		/// <summary>
		/// Gets or sets the character. In terms of caret positions, the position
		/// is always to the left of the character, not trailing it.
		/// </summary>
		/// <value>The character.</value>
		public int CharacterIndex
		{
			get { return characterIndex; }
			set { characterIndex = value; }
		}

		/// <summary>
		/// Gets or sets the line.
		/// </summary>
		/// <value>The line.</value>
		public int LineIndex
		{
			get { return lineIndex; }
			set { lineIndex = value; }
		}

		#endregion

		#region Movement

		/// <summary>
		/// Gets the wrapped line associated with this buffer position.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns></returns>
		public LayoutLine GetWrappedLine(IDisplayContext displayContext)
		{
			Layout layout;
			int wrappedLineIndex;

			return GetWrappedLine(displayContext, out layout, out wrappedLineIndex);
		}

		/// <summary>
		/// Gets the wrapped line associated with this buffer position.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="layout">The layout.</param>
		/// <param name="wrappedLineIndex">Index of the wrapped line.</param>
		/// <returns></returns>
		public LayoutLine GetWrappedLine(
			IDisplayContext displayContext,
			out Layout layout,
			out int wrappedLineIndex)
		{
			// Get the layout associated with the line.
			layout = displayContext.TextRenderer.GetLineLayout(LineIndex);

			// Get the wrapped line associated with this character position.
			int x;

			layout.IndexToLineX(CharacterIndex, false, out wrappedLineIndex, out x);

			// Return the resulting line.
			return layout.Lines[wrappedLineIndex];
		}

		/// <summary>
		/// Determines whether the position is at the beginning of a wrapped line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is begining of wrapped line] [the specified display context]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsBeginingOfWrappedLine(IDisplayContext displayContext)
		{
			return CharacterIndex == GetWrappedLine(displayContext).StartIndex;
		}

		/// <summary>
		/// Determines whether the position is at the beginning of the line.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <returns>
		/// 	<c>true</c> if [is beginning of buffer] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsBeginningOfBuffer(TextRenderer buffer)
		{
			return LineIndex == 0 && CharacterIndex == 0;
		}

		/// <summary>
		/// Determines whether the position is at the beginning of the line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is beginning of buffer] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsBeginningOfBuffer(IDisplayContext displayContext)
		{
			return IsBeginningOfBuffer(displayContext.TextRenderer);
		}

		/// <summary>
		/// Determines whether the position is at the beginning of a line.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <returns>
		/// 	<c>true</c> if [is beginning of line] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsBeginningOfLine(TextRenderer buffer)
		{
			return CharacterIndex == 0;
		}

		/// <summary>
		/// Determines whether the position is at the beginning of a line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is beginning of line] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsBeginningOfLine(IDisplayContext displayContext)
		{
			return IsBeginningOfLine(displayContext.TextRenderer);
		}

		/// <summary>
		/// Determines whether the position is at the end of the buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <returns>
		/// 	<c>true</c> if [is end of buffer] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEndOfBuffer(TextRenderer buffer)
		{
			return LineIndex == buffer.LineBuffer.LineCount - 1 && IsEndOfLine(buffer);
		}

		/// <summary>
		/// Determines whether the position is at the end of the buffer.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is end of buffer] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEndOfBuffer(IDisplayContext displayContext)
		{
			return IsEndOfBuffer(displayContext.TextRenderer);
		}

		/// <summary>
		/// Determines whether the position is at the end of the line.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <returns>
		/// 	<c>true</c> if [is end of line] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEndOfLine(TextRenderer buffer)
		{
			return CharacterIndex == buffer.LineBuffer.GetLineLength(LineIndex);
		}

		/// <summary>
		/// Determines whether the position is at the end of the line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is end of line] [the specified buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEndOfLine(IDisplayContext displayContext)
		{
			return IsEndOfLine(displayContext.TextRenderer);
		}

		/// <summary>
		/// Determines whether the position is at the end of a wrapped line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is end of wrapped line] [the specified display context]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEndOfWrappedLine(IDisplayContext displayContext)
		{
			// Get the wrapped line and layout.
			Layout layout;
			int wrappedLineIndex;
			LayoutLine wrappedLine = GetWrappedLine(
				displayContext, out layout, out wrappedLineIndex);

			// Move to the end of the wrapped line. If this isn't the last, we
			// need to shift back one character.
			int wrappedCharacterIndex = wrappedLine.StartIndex + wrappedLine.Length;

			if (wrappedLineIndex != layout.LineCount - 1)
			{
				wrappedCharacterIndex--;
			}

			// Return if these are equal.
			return CharacterIndex == wrappedCharacterIndex;
		}

		/// <summary>
		/// Determines whether this position represents the last line in the buffer.
		/// </summary>
		/// <param name="lineLayoutBuffer">The line layout buffer.</param>
		/// <returns>
		/// 	<c>true</c> if [is last line in buffer] [the specified line layout buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsLastLineInBuffer(TextRenderer lineLayoutBuffer)
		{
			return LineIndex == lineLayoutBuffer.LineBuffer.LineCount - 1;
		}

		/// <summary>
		/// Determines whether this position represents the last line in the buffer.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <returns>
		/// 	<c>true</c> if [is last line in buffer] [the specified line layout buffer]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsLastLineInBuffer(IDisplayContext displayContext)
		{
			return IsLastLineInBuffer(displayContext.TextRenderer);
		}

		/// <summary>
		/// Moves the position to end beginning of buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public BufferPosition ToBeginningOfBuffer(TextRenderer buffer)
		{
			return new BufferPosition(0, 0);
		}

		/// <summary>
		/// Moves the position to end beginning of buffer.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public BufferPosition ToBeginningOfBuffer(IDisplayContext displayContext)
		{
			return ToBeginningOfBuffer(displayContext.TextRenderer);
		}

		/// <summary>
		/// Moves the position to the beginning of line.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public BufferPosition ToBeginningOfLine(TextRenderer buffer)
		{
			return new BufferPosition(lineIndex, 0);
		}

		/// <summary>
		/// Moves the position to the beginning of line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public BufferPosition ToBeginningOfLine(IDisplayContext displayContext)
		{
			return ToBeginningOfLine(displayContext.TextRenderer);
		}

		/// <summary>
		/// Moves the position to the beginning of wrapped line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public BufferPosition ToBeginningOfWrappedLine(IDisplayContext displayContext)
		{
			return new BufferPosition(
				lineIndex, GetWrappedLine(displayContext).StartIndex);
		}

		/// <summary>
		/// Moves the position to the end of buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public BufferPosition ToEndOfBuffer(TextRenderer buffer)
		{
			int endLineIndex = buffer.LineBuffer.LineCount - 1;

			return new BufferPosition(endLineIndex, buffer.LineBuffer.GetLineLength(endLineIndex));
		}

		/// <summary>
		/// Moves the position to the end of buffer.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public BufferPosition ToEndOfBuffer(IDisplayContext displayContext)
		{
			return ToEndOfBuffer(displayContext.TextRenderer);
		}

		/// <summary>
		/// Moves the position to the end of line.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public BufferPosition ToEndOfLine(TextRenderer buffer)
		{
			return new BufferPosition(lineIndex, buffer.LineBuffer.GetLineLength(lineIndex));
		}

		/// <summary>
		/// Moves the position to the end of line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public BufferPosition ToEndOfLine(IDisplayContext displayContext)
		{
			return ToEndOfLine(displayContext.TextRenderer);
		}

		/// <summary>
		/// Moves the position to the end of wrapped line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public BufferPosition ToEndOfWrappedLine(IDisplayContext displayContext)
		{
			// Get the wrapped line and layout.
			Layout layout;
			int wrappedLineIndex;
			LayoutLine wrappedLine = GetWrappedLine(
				displayContext, out layout, out wrappedLineIndex);

			// Move to the end of the wrapped line. If this isn't the last, we
			// need to shift back one character.
			int index = wrappedLine.StartIndex + wrappedLine.Length;

			if (wrappedLineIndex != layout.LineCount - 1)
			{
				index--;
			}

			return new BufferPosition(lineIndex, index);
		}

		#endregion

		#region Coordinates

		/// <summary>
		/// Converts the given line and character coordinates into pixel coordinates
		/// on the display.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineHeight">Will contains the height of the current line.</param>
		/// <returns></returns>
		public PointD ToScreenCoordinates(
			IDisplayContext displayContext,
			out int lineHeight)
		{
			// Pull out some of the common things we'll be using in this method.
			TextRenderer buffer = displayContext.TextRenderer;
			int bufferLineIndex = buffer.LineBuffer.NormalizeLineIndex(LineIndex);
			Layout layout = buffer.GetLineLayout(bufferLineIndex);
			BlockStyle style = buffer.GetLineStyle(bufferLineIndex);

			// Figure out the top of the current line in relation to the entire
			// buffer and view. For lines beyond the first, we use
			// GetLineLayoutHeight because it also takes into account the line 
			// spacing and borders which we would have to calculate otherwise.
			double y = bufferLineIndex == 0
			           	? 0
			           	: buffer.GetLineLayoutHeight(0, bufferLineIndex - 1);

			// Add the style offset for the top-padding.
			y += style.Top;

			// We need to figure out the relative position. If the position equals
			// the length of the string, we want to put the caret at the end of the
			// character. Otherwise, we put it on the front of the character to
			// indicate insert point.
			bool trailing = false;
			int character = CharacterIndex;

			if (character == buffer.LineBuffer.GetLineLength(bufferLineIndex))
			{
				// Shift back one character to calculate the position and put
				// the cursor at the end of the character.
				character--;
				trailing = true;
			}

			// Figure out which wrapped line we are actually on and the position
			// inside that line. If the character equals the length of the string,
			// then we want to move to the end of it.
			int wrappedLineIndex;
			int layoutX;
			layout.IndexToLineX(character, trailing, out wrappedLineIndex, out layoutX);

			// Get the relative offset into the wrapped lines.
			Rectangle layoutPoint = layout.IndexToPos(CharacterIndex);

			y += Units.ToPixels(layoutPoint.Y);

			// Get the height of the wrapped line.
			Rectangle ink = Rectangle.Zero;
			Rectangle logical = Rectangle.Zero;

			layout.Lines[wrappedLineIndex].GetPixelExtents(ref ink, ref logical);
			lineHeight = logical.Height;

			// Return the results.
			return new PointD(Units.ToPixels(layoutX), y);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Determines if two positions are equal.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <returns></returns>
		public bool Equals(BufferPosition other)
		{
			return other.characterIndex == characterIndex && other.lineIndex == lineIndex;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (obj.GetType() != typeof(BufferPosition))
			{
				return false;
			}
			return Equals((BufferPosition) obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return (characterIndex * 397) ^ lineIndex;
			}
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(BufferPosition a,
		                               BufferPosition b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Implements the operator &gt;.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator >(BufferPosition a,
		                              BufferPosition b)
		{
			return !(a < b);
		}

		/// <summary>
		/// Implements the operator &gt;=.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator >=(BufferPosition a,
		                               BufferPosition b)
		{
			return a > b || a == b;
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(BufferPosition a,
		                               BufferPosition b)
		{
			return !a.Equals(b);
		}

		/// <summary>
		/// Implements the operator &lt;.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator <(BufferPosition a,
		                              BufferPosition b)
		{
			if (a.lineIndex < b.lineIndex)
			{
				return true;
			}

			if (a.lineIndex > b.lineIndex)
			{
				return false;
			}

			if (a.characterIndex < b.CharacterIndex)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Implements the operator &lt;=.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator <=(BufferPosition a,
		                               BufferPosition b)
		{
			return a < b || a == b;
		}

		#endregion

		#region Conversion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("Buffer Position ({0}, {1})", lineIndex, characterIndex);
		}

		#endregion
	}
}