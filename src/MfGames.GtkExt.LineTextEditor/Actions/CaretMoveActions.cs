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

using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Interfaces;

using Pango;

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
		/// Moves the caret down one line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void Down(IDisplayContext displayContext)
		{
			// Get the position of the text. This uses the wrapped line index
			// because we do relative movements to the screen.
			BufferPosition position = displayContext.Caret.BufferPosition;
			ILineLayoutBuffer buffer = displayContext.LineLayoutBuffer;
			Layout layout = buffer.GetLineLayout(displayContext, position.LineIndex);

			int wrappedLineIndex, x;
			layout.IndexToLineX(
				position.CharacterIndex, true, out wrappedLineIndex, out x);

			// We'll need to redraw things.
			try
			{
				// Check to see if we are the last wrapped line of the last line.
				if (position.LineIndex == buffer.LineCount &&
				    wrappedLineIndex == layout.LineCount - 1)
				{
					// We can't do anything, so just move to the end of the line.
					position.CharacterIndex = buffer.GetLineLength(position.LineIndex);
					return;
				}

				// If we are the last line in the layout, we need to move down
				// a line and get the new layout.
				if (wrappedLineIndex == layout.LineCount - 1)
				{
					// We are the last line in the layout but we aren't the last
					// line in the buffer.
					position.LineIndex++;
					layout = buffer.GetLineLayout(displayContext, position.LineIndex);
					wrappedLineIndex = 0;
				}
				else
				{
					// We can move down inside the layout.
					wrappedLineIndex++;
				}

				// Figure out the character position from the given wrapped line.
				LayoutLine wrappedLine = layout.Lines[wrappedLineIndex];
				int characterIndex, trailing;

				wrappedLine.XToIndex(x, out characterIndex, out trailing);

				// Set the position in the new string but shift back slight to prevent
				// it from sliding to the side.
				position.CharacterIndex = Math.Max(0, characterIndex - 1);
			}
			finally
			{
				// Cause the text editor to redraw itself.
				((TextEditor) displayContext).QueueDraw();
			}
		}

		/// <summary>
		/// Moves the caret left one character.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void Left(IDisplayContext displayContext)
		{
			// Move the character position.
			BufferPosition position = displayContext.Caret.BufferPosition;

			if (position.CharacterIndex == 0)
			{
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.CharacterIndex =
						displayContext.LineLayoutBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.CharacterIndex--;
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
			// Get the text and line for the position in question.
			BufferPosition position = displayContext.Caret.BufferPosition;
			string text = displayContext.LineLayoutBuffer.GetLineText(
				position.LineIndex, 0, -1);

			// Figure out the boundaries.
			int leftBoundary, rightBoundary;
			displayContext.WordSplitter.FindWordBoundaries(
				text, position.CharacterIndex, out leftBoundary, out rightBoundary);

			// If there is no left boundary, we move up a line.
			if (leftBoundary == -1)
			{
				// Check to see if we are at the top of the line or not.
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.CharacterIndex =
						displayContext.LineLayoutBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.CharacterIndex = leftBoundary;
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

			if (position.CharacterIndex == buffer.GetLineLength(position.LineIndex))
			{
				if (position.LineIndex < buffer.LineCount - 1)
				{
					position.LineIndex++;
					position.CharacterIndex = 0;
				}
			}
			else
			{
				position.CharacterIndex++;
			}

			// Cause the text editor to redraw itself.
			((TextEditor) displayContext).QueueDraw();
		}

		/// <summary>
		/// Moves the caret right one word.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void RightWord(IDisplayContext displayContext)
		{
			// Get the text and line for the position in question.
			BufferPosition position = displayContext.Caret.BufferPosition;
			string text = displayContext.LineLayoutBuffer.GetLineText(
				position.LineIndex, 0, -1);

			// Figure out the boundaries.
			int leftBoundary, rightBoundary;
			displayContext.WordSplitter.FindWordBoundaries(
				text, position.CharacterIndex, out leftBoundary, out rightBoundary);

			// If there is no right boundary, we move down a line.
			if (rightBoundary == -1)
			{
				// Check to see if we are at the top of the line or not.
				if (position.LineIndex <= displayContext.LineLayoutBuffer.LineCount)
				{
					position.LineIndex++;
					position.CharacterIndex = 0;
				}
			}
			else
			{
				position.CharacterIndex = rightBoundary;
			}

			// Cause the text editor to redraw itself.
			((TextEditor) displayContext).QueueDraw();
		}

		/// <summary>
		/// Moves the caret up one line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		public static void Up(IDisplayContext displayContext)
		{
			// Get the position of the text. This uses the wrapped line index
			// because we do relative movements to the screen.
			BufferPosition position = displayContext.Caret.BufferPosition;
			ILineLayoutBuffer buffer = displayContext.LineLayoutBuffer;
			Layout layout = buffer.GetLineLayout(displayContext, position.LineIndex);

			int wrappedLineIndex, x;
			layout.IndexToLineX(
				position.CharacterIndex, true, out wrappedLineIndex, out x);

			// We need to redraw things, so add a finally.
			try
			{
				// Check to see if we are the first wrapped line of the first line.
				if (position.LineIndex == 0 && wrappedLineIndex == 0)
				{
					// We can't do anything, so just move to the end of the line.
					position.CharacterIndex = 0;
					return;
				}

				// If we are the first line in the layout, we need to move up
				// a line and get the new layout.
				if (wrappedLineIndex == 0)
				{
					// We are the first line in the layout but we aren't the first
					// line in the buffer.
					position.LineIndex--;
					layout = buffer.GetLineLayout(displayContext, position.LineIndex);
					wrappedLineIndex = layout.LineCount - 1;
				}
				else
				{
					// We can move up inside the layout.
					wrappedLineIndex--;
				}

				// Figure out the character position from the given wrapped line.
				LayoutLine wrappedLine = layout.Lines[wrappedLineIndex];
				int characterIndex, trailing;

				wrappedLine.XToIndex(x, out characterIndex, out trailing);

				// Set the position in the new string but shift back slight to prevent
				// it from sliding to the side.
				position.CharacterIndex = Math.Max(0, characterIndex - 1);
			}
			finally
			{
				// Cause the text editor to redraw itself.
				((TextEditor) displayContext).QueueDraw();
			}
		}
	}
}