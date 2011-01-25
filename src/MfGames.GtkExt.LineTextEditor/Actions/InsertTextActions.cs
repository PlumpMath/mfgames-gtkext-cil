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

using MfGames.GtkExt.LineTextEditor.Attributes;
using MfGames.GtkExt.LineTextEditor.Commands;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
	/// <summary>
	/// Contains the various actions used for moving the caret (cursor) around
	/// the text buffer.
	/// </summary>
	[ActionFixture]
	public static class InsertTextActions
	{
		/// <summary>
		/// Moves the caret down one line.
		/// </summary>
		/// <param name="actionContext">The display context.</param>
		/// <param name="unicode">The unicode.</param>
		public static void InsertText(
			IActionContext actionContext,
			char unicode)
		{
			// Create a set text operation.
			SetTextOperation operation = new SetTextOperation();
			operation.Position = actionContext.DisplayContext.Caret.Position;
			operation.Text = unicode.ToString();

			// Perform the operation on the buffer.
			actionContext.Do(operation);

			// Shift the position over.
			CaretMoveActions.Right(actionContext);
		}
	}
}