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

using MfGames.GtkExt.LineTextEditor.Events;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Interfaces
{
	/// <summary>
	/// Represents a virtualized collection of lines for viewing and
	/// editing.
	/// </summary>
	public interface ILineBuffer
	{
		#region Buffer Viewing

		int LineCount { get; }

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		bool ReadOnly { get; }

		/// <summary>
		/// Gets the length of the line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>The length of the line.</returns>
		int GetLineLength(int lineIndex);

		/// <summary>
		/// Gets the formatted line number for a given line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>A formatted line number or null.</returns>
		string GetLineNumber(int lineIndex);

		/// <summary>
		/// Gets the text of a given line in the buffer. If endIndex is -1, it
		/// indicates the end of the line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or -1 for the last line.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="endIndex">The end index or -1 for the last character in the line.</param>
		/// <returns></returns>
		string GetLineText(
			int lineIndex,
			int startIndex,
			int endIndex);

		#endregion

		#region Buffer Operations

		/// <summary>
		/// Performs the given operation, raising any events for changing.
		/// </summary>
		/// <param name="operation">The operation.</param>
		void Do(ILineBufferOperation operation);

		/// <summary>
		/// Used to indicate that a line changed.
		/// </summary>
		event EventHandler<LineChangedArgs> LineChanged;

		/// <summary>
		/// Occurs when lines are inserted into the buffer.
		/// </summary>
		event EventHandler<LineRangeEventArgs> LinesDeleted;

		/// <summary>
		/// Occurs when lines are inserted into the buffer.
		/// </summary>
		event EventHandler<LineRangeEventArgs> LinesInserted;

		#endregion
	}
}