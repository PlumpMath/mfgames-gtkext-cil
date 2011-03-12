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

using Cairo;

using Gdk;

using MfGames.GtkExt.Extensions.Pango;
using MfGames.GtkExt.LineTextEditor.Attributes;
using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Editing;
using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Renderers;

using Pango;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
	/// <summary>
	/// Contains the various actions used for moving the caret (cursor) around
	/// the text buffer.
	/// </summary>
	[ActionFixture]
	public static class MoveActions
	{
		#region Cursor Movement

		/// <summary>
		/// Moves the caret to the end of the buffer.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.KP_Home, ModifierType.ControlMask)]
		[KeyBinding(Key.Home, ModifierType.ControlMask)]
		public static void BeginningOfBuffer(IActionContext actionContext)
		{
			// Redraw the previous area of the caret.
			IDisplayContext displayContext = actionContext.DisplayContext;

			// Queue a draw of the old caret position.
			Caret caret = displayContext.Caret;

			displayContext.RequestRedraw(caret.GetDrawRegion());

			// Move the cursor and redraw the area.
			caret.Position = caret.Position.ToBeginningOfBuffer(displayContext);
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(caret.GetDrawRegion());
		}

		/// <summary>
		/// Moves the caret to the end of the visible line.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.KP_Home)]
		[KeyBinding(Key.Home)]
		public static void BeginningOfWrappedLine(IActionContext actionContext)
		{
			// Redraw the previous area of the caret.
			IDisplayContext displayContext = actionContext.DisplayContext;

			// Queue a draw of the old caret position.
			Caret caret = displayContext.Caret;

			displayContext.RequestRedraw(caret.GetDrawRegion());

			// Move the cursor and redraw the area.
			caret.Position = caret.Position.ToBeginningOfWrappedLine(displayContext);
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(caret.GetDrawRegion());
		}

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
			// Extract a number of useful variable for this method.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			TextRenderer buffer = displayContext.TextRenderer;

			// Queue a draw of the old caret position.
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());

			// Figure out the layout and wrapped line we are currently on.
			Layout layout;
			int wrappedLineIndex;
			LayoutLine wrappedLine = position.GetWrappedLine(
				displayContext, out layout, out wrappedLineIndex);

			// Figure out the X coordinate of the line. If there is an action context,
			// use that. Otherwise, calculate it from the character index of the position.
			int lineX = GetLineX(actionContext, wrappedLine, position);

			// Figure out which wrapped line we'll be moving the caret to.
			if (wrappedLine.IsLastLineInLayout())
			{
				// If we are the last line in the buffer, just do nothing.
				if (position.IsLastLineInBuffer(buffer))
				{
					return;
				}

				// Move to the next line.
				position.LineIndex++;
				layout = buffer.GetLineLayout(position.LineIndex);
				wrappedLine = layout.Lines[0];
			}
			else
			{
				// Just move down in the layout.
				wrappedLineIndex++;
				wrappedLine = layout.Lines[wrappedLineIndex];
			}

			// The wrapped line has the current wrapped line, so use the lineX
			// to figure out which character to use.
			int trailing;
			int index;

			wrappedLine.XToIndex(lineX, out index, out trailing);
			position.CharacterIndex = index;
			displayContext.Caret.Position = position;

			// Draw the new location of the caret.
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());
		}

		/// <summary>
		/// Moves the caret to the end of the buffer.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.KP_End, ModifierType.ControlMask)]
		[KeyBinding(Key.End, ModifierType.ControlMask)]
		public static void EndOfBuffer(IActionContext actionContext)
		{
			// Redraw the previous area of the caret.
			IDisplayContext displayContext = actionContext.DisplayContext;

			// Queue a draw of the old caret position.
			Caret caret = displayContext.Caret;

			displayContext.RequestRedraw(caret.GetDrawRegion());

			// Move the cursor and redraw the area.
			caret.Position = caret.Position.ToEndOfBuffer(displayContext);
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(caret.GetDrawRegion());
		}

		/// <summary>
		/// Moves the caret to the end of the line.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.KP_End)]
		[KeyBinding(Key.End)]
		public static void EndOfWrappedLine(IActionContext actionContext)
		{
			// Redraw the previous area of the caret.
			IDisplayContext displayContext = actionContext.DisplayContext;

			// Queue a draw of the old caret position.
			Caret caret = displayContext.Caret;

			displayContext.RequestRedraw(caret.GetDrawRegion());

			// Move the cursor and redraw the area.
			caret.Position = caret.Position.ToEndOfWrappedLine(displayContext);
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(caret.GetDrawRegion());
		}

		/// <summary>
		/// Gets the line X coordinates from either the state if we have one
		/// or calculate it from the buffer position's X coordinate.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <param name="wrappedLine">The wrapped line.</param>
		/// <param name="position">The position.</param>
		/// <returns></returns>
		private static int GetLineX(
			IActionContext actionContext,
			LayoutLine wrappedLine,
			BufferPosition position)
		{
			int lineX;
			var state = actionContext.States.Get<VerticalMovementActionState>();

			if (state == null)
			{
				// Calculate the line state from the caret position.
				lineX = wrappedLine.IndexToX(position.CharacterIndex, false);

				// Save a new state into the states.
				state = new VerticalMovementActionState(lineX);
				actionContext.States.Add(state);
			}
			else
			{
				// Get the line coordinate from the state.
				lineX = state.LayoutLineX;
			}

			return lineX;
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
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;

			if (position.CharacterIndex == 0)
			{
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.CharacterIndex =
						displayContext.LineBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.CharacterIndex--;
			}

			// Cause the text editor to redraw itself.
			displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());
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
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			string text = displayContext.LineBuffer.GetLineText(position.LineIndex);

			// If there is no left boundary, we move up a line.
			int leftBoundary = displayContext.WordSplitter.GetPreviousWordBoundary(
				text, position.CharacterIndex);

			if (leftBoundary == -1)
			{
				// Check to see if we are at the top of the line or not.
				if (position.LineIndex > 0)
				{
					position.LineIndex--;
					position.CharacterIndex =
						displayContext.LineBuffer.GetLineLength(position.LineIndex);
				}
			}
			else
			{
				position.CharacterIndex = leftBoundary;
			}

			// Cause the text editor to redraw itself.
			displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());
		}

		/// <summary>
		/// Moves the caret down one page.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Page_Down)]
		[KeyBinding(Key.Page_Down)]
		public static void PageDown(IActionContext actionContext)
		{
			// Extract a number of useful variable for this method.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;

			// Queue a draw of the old caret position.
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());

			// Figure out the layout and wrapped line we are currently on.
			Layout layout;
			int wrappedLineIndex;
			LayoutLine wrappedLine = position.GetWrappedLine(
				displayContext, out layout, out wrappedLineIndex);

			// Figure out where in the buffer we're located.
			int lineHeight;

			PointD point = position.ToScreenCoordinates(displayContext, out lineHeight);

			// Shift down the buffer a full page size and clamp it to the actual
			// buffer size.
			double bufferY =
				Math.Min(
					point.Y + displayContext.VerticalAdjustment.PageSize,
					displayContext.TextRenderer.GetLineLayoutHeight(0, Int32.MaxValue));

			// Figure out the X coordinate of the line. If there is an action context,
			// use that. Otherwise, calculate it from the character index of the position.
			int lineX = Units.ToPixels(GetLineX(actionContext, wrappedLine, position));

			// Move to the calculated point.
			Point(
				displayContext, new PointD(lineX, bufferY - displayContext.BufferOffsetY));
		}

		/// <summary>
		/// Moves the caret down one page.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Page_Up)]
		[KeyBinding(Key.Page_Up)]
		public static void PageUp(IActionContext actionContext)
		{
			// Extract a number of useful variable for this method.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;

			// Queue a draw of the old caret position.
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());

			// Figure out the layout and wrapped line we are currently on.
			Layout layout;
			int wrappedLineIndex;
			LayoutLine wrappedLine = position.GetWrappedLine(
				displayContext, out layout, out wrappedLineIndex);

			// Figure out where in the buffer we're located.
			int lineHeight;

			PointD point = position.ToScreenCoordinates(displayContext, out lineHeight);

			// Shift down the buffer a full page size and clamp it to the actual
			// buffer size.
			double bufferY =
				Math.Max(point.Y - displayContext.VerticalAdjustment.PageSize, 0);

			// Figure out the X coordinate of the line. If there is an action context,
			// use that. Otherwise, calculate it from the character index of the position.
			int lineX = Units.ToPixels(GetLineX(actionContext, wrappedLine, position));

			// Move to the calculated point.
			Point(
				displayContext, new PointD(lineX, bufferY - displayContext.BufferOffsetY));
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
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			LineBuffer lineBuffer = displayContext.LineBuffer;

			if (position.CharacterIndex == lineBuffer.GetLineLength(position.LineIndex))
			{
				if (position.LineIndex < lineBuffer.LineCount - 1)
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
			displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());
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
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			string text = displayContext.LineBuffer.GetLineText(position.LineIndex);

			// If there is no right boundary, we move down a line.
			int rightBoundary = displayContext.WordSplitter.GetNextWordBoundary(
				text, position.CharacterIndex);

			if (rightBoundary == -1)
			{
				// Check to see if we are at the top of the line or not.
				if (position.LineIndex <= displayContext.LineBuffer.LineCount)
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
			displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());
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
			// Extract a number of useful variable for this method.
			IDisplayContext displayContext = actionContext.DisplayContext;
			BufferPosition position = displayContext.Caret.Position;
			TextRenderer buffer = displayContext.TextRenderer;

			// Queue a draw of the old caret position.
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());

			// Figure out the layout and wrapped line we are currently on.
			Layout layout;
			int wrappedLineIndex;
			LayoutLine wrappedLine = position.GetWrappedLine(
				displayContext, out layout, out wrappedLineIndex);

			// Figure out the X coordinate of the line. If there is an action context,
			// use that. Otherwise, calculate it from the character index of the position.
			int lineX = GetLineX(actionContext, wrappedLine, position);

			// Figure out which wrapped line we'll be moving the caret to.
			if (wrappedLineIndex == 0)
			{
				// If we are the last line in the buffer, just do nothing.
				if (position.LineIndex == 0)
				{
					return;
				}

				// Move to the next line.
				position.LineIndex--;
				layout = buffer.GetLineLayout(position.LineIndex);
				wrappedLineIndex = layout.LineCount - 1;
				wrappedLine = layout.Lines[wrappedLineIndex];
			}
			else
			{
				// Just move down in the layout.
				wrappedLineIndex--;
				wrappedLine = layout.Lines[wrappedLineIndex];
			}

			// The wrapped line has the current wrapped line, so use the lineX
			// to figure out which character to use.
			int trailing;
			int index;

			wrappedLine.XToIndex(lineX, out index, out trailing);
			position.CharacterIndex = index;

			// Draw the new location of the caret.
			displayContext.Caret.Position = position;
			displayContext.ScrollToCaret();
			displayContext.RequestRedraw(displayContext.Caret.GetDrawRegion());
		}

		#endregion

		#region Coordinates

		/// <summary>
		/// Gets the buffer position from a given point.
		/// </summary>
		/// <param name="widgetPoint">The widget point.</param>
		/// <param name="displayContext">The display context.</param>
		/// <returns></returns>
		public static BufferPosition GetBufferPosition(
			PointD widgetPoint,
			IDisplayContext displayContext)
		{
			double y = widgetPoint.Y + displayContext.BufferOffsetY;
			int lineIndex = displayContext.TextRenderer.GetLineLayoutRange(y);
			Layout layout = displayContext.TextRenderer.GetLineLayout(lineIndex);

			// Shift the buffer-relative coordinates to layout-relative coordinates.
			double layoutY = y;

			if (lineIndex > 0)
			{
				layoutY -= displayContext.TextRenderer.GetLineLayoutHeight(0, lineIndex - 1);
			}

			int pangoLayoutY = Units.FromPixels((int) layoutY);

			// Determines where in the layout is the point.
			int pangoLayoutX = Units.FromPixels((int) widgetPoint.X);
			int characterIndex, trailing;

			layout.XyToIndex(
				pangoLayoutX, pangoLayoutY, out characterIndex, out trailing);

			// Return the buffer position.
			return new BufferPosition(lineIndex, characterIndex);
		}

		/// <summary>
		/// Moves the caret to a specific widget-relative point on the screen.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="widgetPoint">The point in the widget.</param>
		public static void Point(
			IDisplayContext displayContext,
			PointD widgetPoint)
		{
			// Find the line layout that is closest to the point given.
			displayContext.Caret.Position = GetBufferPosition(
				widgetPoint, displayContext);

			// Move to and draw the caret.
			displayContext.ScrollToCaret();
		}

		#endregion

		#region Selection

		/// <summary>
		/// Performs an action that handles a move action coupled with an
		/// extend or set selection.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		/// <param name="action">The action.</param>
		private static void SelectAction(
			IActionContext actionContext,
			Action<IActionContext> action)
		{
			// Grab the anchor position of the selection since that will
			// remain the same after the command.
			Caret caret = actionContext.DisplayContext.Caret;
			BufferPosition anchorPosition = caret.Selection.AnchorPosition;

			// Perform the move command.
			action(actionContext);

			// Restore the anchor position which will extend the selection back.
			caret.Selection.AnchorPosition = anchorPosition;
		}

		/// <summary>
		/// Selects all the text in the buffer.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		[Action]
		[KeyBinding(Key.A, ModifierType.ControlMask)]
		public static void SelectAll(IActionContext actionContext)
		{
			actionContext.DisplayContext.Caret.Selection.AnchorPosition =
				new BufferPosition(0, 0);
			actionContext.DisplayContext.Caret.Selection.TailPosition =
				new BufferPosition(Int32.MaxValue, Int32.MaxValue);

			actionContext.DisplayContext.RequestRedraw();
		}

		/// <summary>
		/// Expands the selection to the beginning of the buffer.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Home, ModifierType.ShiftMask | ModifierType.ControlMask)]
		[KeyBinding(Key.Home, ModifierType.ShiftMask | ModifierType.ControlMask)]
		public static void SelectBeginningOfBuffer(IActionContext actionContext)
		{
			SelectAction(actionContext, BeginningOfBuffer);
		}

		/// <summary>
		/// Expands the selection to the beginning of the line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Home, ModifierType.ShiftMask)]
		[KeyBinding(Key.Home, ModifierType.ShiftMask)]
		public static void SelectBeginningOfWrappedLine(IActionContext actionContext)
		{
			SelectAction(actionContext, BeginningOfWrappedLine);
		}

		/// <summary>
		/// Expands the selection down one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Down, ModifierType.ShiftMask)]
		[KeyBinding(Key.Down, ModifierType.ShiftMask)]
		public static void SelectDown(IActionContext actionContext)
		{
			SelectAction(actionContext, Down);
		}

		/// <summary>
		/// Expands the selection to the end of the buffer.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_End, ModifierType.ShiftMask | ModifierType.ControlMask)]
		[KeyBinding(Key.End, ModifierType.ShiftMask | ModifierType.ControlMask)]
		public static void SelectEndOfBuffer(IActionContext actionContext)
		{
			SelectAction(actionContext, EndOfBuffer);
		}

		/// <summary>
		/// Expands the selection to the end of the line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_End, ModifierType.ShiftMask)]
		[KeyBinding(Key.End, ModifierType.ShiftMask)]
		public static void SelectEndOfWrappedLine(IActionContext actionContext)
		{
			SelectAction(actionContext, EndOfWrappedLine);
		}

		/// <summary>
		/// Expands the selection left one character.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Left, ModifierType.ShiftMask)]
		[KeyBinding(Key.Left, ModifierType.ShiftMask)]
		public static void SelectLeft(IActionContext actionContext)
		{
			SelectAction(actionContext, Left);
		}

		/// <summary>
		/// Expands the selection left one word.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Left, ModifierType.ShiftMask | ModifierType.ControlMask)]
		[KeyBinding(Key.Left, ModifierType.ShiftMask | ModifierType.ControlMask)]
		public static void SelectLeftWord(IActionContext actionContext)
		{
			SelectAction(actionContext, LeftWord);
		}

		/// <summary>
		/// Expands the selection down one page.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Page_Down, ModifierType.ShiftMask)]
		[KeyBinding(Key.Page_Down, ModifierType.ShiftMask)]
		public static void SelectPageDown(IActionContext actionContext)
		{
			SelectAction(actionContext, PageDown);
		}

		/// <summary>
		/// Expands the selection up one page.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Page_Up, ModifierType.ShiftMask)]
		[KeyBinding(Key.Page_Up, ModifierType.ShiftMask)]
		public static void SelectPageUp(IActionContext actionContext)
		{
			SelectAction(actionContext, PageUp);
		}

		/// <summary>
		/// Expands the selection right one character.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Right, ModifierType.ShiftMask)]
		[KeyBinding(Key.Right, ModifierType.ShiftMask)]
		public static void SelectRight(IActionContext actionContext)
		{
			SelectAction(actionContext, Right);
		}

		/// <summary>
		/// Expands the selection right one word.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Right, ModifierType.ShiftMask | ModifierType.ControlMask)]
		[KeyBinding(Key.Right, ModifierType.ShiftMask | ModifierType.ControlMask)]
		public static void SelectRightWord(IActionContext actionContext)
		{
			SelectAction(actionContext, RightWord);
		}

		/// <summary>
		/// Expands the selection up one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		[Action]
		[ActionState(typeof(VerticalMovementActionState))]
		[KeyBinding(Key.KP_Up, ModifierType.ShiftMask)]
		[KeyBinding(Key.Up, ModifierType.ShiftMask)]
		public static void SelectUp(IActionContext actionContext)
		{
			SelectAction(actionContext, Up);
		}

		#endregion
	}
}