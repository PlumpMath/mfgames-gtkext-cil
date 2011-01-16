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

using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
	/// <summary>
	/// Contains the various actions used for moving the caret (cursor) around
	/// the text buffer.
	/// </summary>
	public static class CaretMoveActions
	{
		/// <summary>
		/// Moves the caret left one character.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void Left(IDisplayContext displayContext)
		{
			// Move the character position.
			BufferPosition position = displayContext.Caret.BufferPosition;

			if (position.Character == 0)
			{
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.Character =
						displayContext.LineLayoutBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.Character--;
			}

			// Cause the text editor to redraw itself.
			((TextEditor) displayContext).QueueDraw();
		}

		/// <summary>
		/// Moves the caret right one character.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void Right(IDisplayContext displayContext)
		{
			// Move the character position.
			BufferPosition position = displayContext.Caret.BufferPosition;
			ILineLayoutBuffer buffer = displayContext.LineLayoutBuffer;

			if (position.Character == buffer.GetLineLength(position.LineIndex))
			{
				if (position.LineIndex < buffer.LineCount - 1)
				{
					position.LineIndex++;
					position.Character = 0;
				}
			}
			else
			{
				position.Character++;
			}

			// Cause the text editor to redraw itself.
			((TextEditor) displayContext).QueueDraw();
		}

		/// <summary>
		/// Moves the caret left one word.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void LeftWord(IDisplayContext displayContext)
		{
		}
	}
}