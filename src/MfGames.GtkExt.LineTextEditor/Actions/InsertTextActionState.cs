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

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Commands;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
	/// <summary>
	/// Represents an action state that combines multiple text inserts
	/// together when they are part of the same word (as defined by the word
	/// splitter).
	/// </summary>
	public class InsertTextActionState : IActionState
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="InsertTextActionState"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="text">The text.</param>
		public InsertTextActionState(
			Command command,
			string text)
		{
			Command = command;
			Text = text;
		}

		#endregion

		#region Action State

		/// <summary>
		/// Determines whether this action state can be removed. This is also
		/// an opportunity for the action to clean up before removed.
		/// </summary>
		/// <returns>
		///   <c>true</c> if this instance can remove; otherwise, <c>false</c>.
		/// </returns>
		public bool CanRemove()
		{
			return true;
		}

		#endregion

		#region Insert Text

		/// <summary>
		/// Gets the command associated with the last insert text state.
		/// </summary>
		/// <value>The command.</value>
		public Command Command { get; private set; }

		/// <summary>
		/// Gets or sets the end position.
		/// </summary>
		/// <value>The end position.</value>
		public BufferPosition EndPosition
		{
			get { return Command.EndPosition; }
			set { Command.EndPosition = value; }
		}

		/// <summary>
		/// Gets the set text operation.
		/// </summary>
		/// <value>The set text operation.</value>
		public SetTextOperation SetTextOperation
		{
			get
			{
				// There is only one operation in the list and that is the
				// set text operation.
				return (SetTextOperation) Command.Operations[0];
			}
		}

		/// <summary>
		/// Gets or sets the text inserted.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		#endregion
	}
}