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

using Gtk;

using MfGames.GtkExt.LineTextEditor.Actions;
using MfGames.GtkExt.LineTextEditor.Commands;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Interfaces
{
	/// <summary>
	/// Defines the interface to the context for a specific action.
	/// </summary>
	public interface IActionContext
	{
		/// <summary>
		/// Gets the commands for the text editor.
		/// </summary>
		CommandManager Commands { get; }

		/// <summary>
		/// Gets the display context for this action.
		/// </summary>
		/// <value>The display.</value>
		IDisplayContext DisplayContext { get; }

		/// <summary>
		/// Gets the action states associated with the action.
		/// </summary>
		ActionStateCollection States { get; }

		/// <summary>
		/// Creates the context menu for the caret position.
		/// </summary>
		/// <returns></returns>
		Menu CreateContextMenu();

		/// <summary>
		/// Performs the given operation on the line buffer.
		/// </summary>
		/// <param name="operation">The operation.</param>
		void Do(ILineBufferOperation operation);

		/// <summary>
		/// Performs the given command on the line buffer.
		/// </summary>
		/// <param name="command">The command.</param>
		void Do(Command command);
	}
}