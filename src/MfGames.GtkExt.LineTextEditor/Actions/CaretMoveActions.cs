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

using Gdk;

using MfGames.GtkExt.LineTextEditor.Attributes;
using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Interfaces;

using Pango;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
	/// <summary>
	/// Contains the various actions used for moving the caret (cursor) around
	/// the text buffer.
	/// </summary>
	[ActionFixture]
	public static class CaretMoveActions
	{
		/// <summary>
		/// Moves the caret down one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Down)]
		[KeyBinding(Key.Down)]
		public static void Down(IActionContext actionContext)
		{
			// Get the position of the text. This uses the wrapped line index
			// because we do relative movements to the screen.
			BufferPosition position = actionContext.Display.Caret.Position;
			ILineLayoutBuffer buffer = actionContext.Display.LineLayoutBuffer;
			Layout layout = buffer.GetLineLayout(actionContext.Display, position.LineIndex);

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
					layout = buffer.GetLineLayout(actionContext.Display, position.LineIndex);
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
				((TextEditor) actionContext.Display).QueueDraw();
			}
		}

		/// <summary>
		/// Moves the caret left one character.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[KeyBinding(Key.KP_Left)]
		[KeyBinding(Key.Left)]
		public static void Left(IActionContext actionContext)
		{
			// Move the character position.
			BufferPosition position = actionContext.Display.Caret.Position;

			if (position.CharacterIndex == 0)
			{
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.CharacterIndex =
						actionContext.Display.LineLayoutBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.CharacterIndex--;
			}

			// Cause the text editor to redraw itself.
			((TextEditor) actionContext.Display).QueueDraw();
		}

		/// <summary>
		/// Moves the caret left one word.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[KeyBinding(Key.KP_Left, ModifierType.ControlMask)]
		[KeyBinding(Key.Left, ModifierType.ControlMask)]
		public static void LeftWord(IActionContext actionContext)
		{
			// Get the text and line for the position in question.
			BufferPosition position = actionContext.Display.Caret.Position;
			string text = actionContext.Display.LineLayoutBuffer.GetLineText(
				position.LineIndex, 0, -1);

			// Figure out the boundaries.
			int leftBoundary, rightBoundary;
			actionContext.Display.WordSplitter.FindWordBoundaries(
				text, position.CharacterIndex, out leftBoundary, out rightBoundary);

			// If there is no left boundary, we move up a line.
			if (leftBoundary == -1)
			{
				// Check to see if we are at the top of the line or not.
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.CharacterIndex =
						actionContext.Display.LineLayoutBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.CharacterIndex = leftBoundary;
			}

			// Cause the text editor to redraw itself.
			((TextEditor) actionContext.Display).QueueDraw();
		}

		/// <summary>
		/// Moves the caret right one character.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[KeyBinding(Key.KP_Right)]
		[KeyBinding(Key.Right)]
		public static void Right(IActionContext actionContext)
		{
			// Move the character position.
			BufferPosition position = actionContext.Display.Caret.Position;
			ILineLayoutBuffer buffer = actionContext.Display.LineLayoutBuffer;

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
			((TextEditor) actionContext.Display).QueueDraw();
		}

		/// <summary>
		/// Moves the caret right one word.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[KeyBinding(Key.KP_Right, ModifierType.ControlMask)]
		[KeyBinding(Key.Right, ModifierType.ControlMask)]
		public static void RightWord(IActionContext actionContext)
		{
			// Get the text and line for the position in question.
			BufferPosition position = actionContext.Display.Caret.Position;
			string text = actionContext.Display.LineLayoutBuffer.GetLineText(
				position.LineIndex, 0, -1);

			// Figure out the boundaries.
			int leftBoundary, rightBoundary;
			actionContext.Display.WordSplitter.FindWordBoundaries(
				text, position.CharacterIndex, out leftBoundary, out rightBoundary);

			// If there is no right boundary, we move down a line.
			if (rightBoundary == -1)
			{
				// Check to see if we are at the top of the line or not.
				if (position.LineIndex <= actionContext.Display.LineLayoutBuffer.LineCount)
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
			((TextEditor) actionContext.Display).QueueDraw();
		}

		/// <summary>
		/// Moves the caret up one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Up)]
		[KeyBinding(Key.Up)]
		public static void Up(IActionContext actionContext)
		{
			// Get the position of the text. This uses the wrapped line index
			// because we do relative movements to the screen.
			BufferPosition position = actionContext.Display.Caret.Position;
			ILineLayoutBuffer buffer = actionContext.Display.LineLayoutBuffer;
			Layout layout = buffer.GetLineLayout(actionContext.Display, position.LineIndex);

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
					layout = buffer.GetLineLayout(actionContext.Display, position.LineIndex);
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
				((TextEditor) actionContext.Display).QueueDraw();
			}
		}
	}
}