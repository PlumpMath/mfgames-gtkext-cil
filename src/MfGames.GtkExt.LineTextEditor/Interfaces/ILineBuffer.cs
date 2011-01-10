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

#endregion

namespace MfGames.GtkExt.LineTextEditor
{
	/// <summary>
	/// Represents a virtualized collection of lines for viewing and
	/// editing.
	/// </summary>
	public interface ILineBuffer
	{
		#region Buffer Events

		event EventHandler LineBufferChanged;
		event EventHandler LineChanged;
		event EventHandler LineCountChanged;
		event EventHandler LineDeleted;
		event EventHandler LineInserted;

		#endregion

		#region Buffer Viewing

		int LineCount { get; }

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		bool ReadOnly { get; }

		int GetLineLength(int line);
		string GetLineNumber(int line);

		string GetLineText(
			int line,
			int startIndex,
			int endIndex);

		#endregion

		#region Buffer Editing

		void DeleteLines(
			int startLine,
			int endLine);

		void InsertLines(
			int afterLine,
			int count);

		void SetLineText(
			int line,
			int startIndex,
			int endIndex,
			string text);

		#endregion
	}
}