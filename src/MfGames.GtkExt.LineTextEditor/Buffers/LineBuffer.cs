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
using System.Collections.Generic;

using MfGames.GtkExt.LineTextEditor.Events;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Represents a virtual collection of lines for viewing and
	/// editing.
	/// </summary>
	public abstract class LineBuffer
	{
		#region Buffer

		/// <summary>
		/// Gets the number of lines in the buffer.
		/// </summary>
		/// <value>The line count.</value>
		public abstract int LineCount { get; }

		/// <summary>
		/// If set to <see langword="true"/>, the buffer is read-only and the editing
		/// commands should throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public abstract bool ReadOnly { get; }

		#endregion

		#region Lines

		/// <summary>
		/// Gets the length of the line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>The length of the line.</returns>
		public abstract int GetLineLength(int lineIndex);

		/// <summary>
		/// Gets the formatted line number for a given line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>A formatted line number.</returns>
		public abstract string GetLineNumber(int lineIndex);

		/// <summary>
		/// Gets the text for a specific line.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <returns></returns>
		public string GetLineText(int lineIndex)
		{
			return GetLineText(lineIndex, 0, Int32.MaxValue);
		}

		/// <summary>
		/// Gets the text of a given line in the buffer.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer. If the index is beyond the end of the buffer, the last line is used.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="endIndex">The end index. If the index is greater than the index, the end of the line is used.</param>
		/// <returns></returns>
		public abstract string GetLineText(
			int lineIndex,
			int startIndex,
			int endIndex);

		/// <summary>
		/// Normalizes the index of the line and makes sure nothing is beyond
		/// the limits.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <returns></returns>
		public int NormalizeLineIndex(int lineIndex)
		{
			return Math.Min(LineCount - 1, lineIndex);
		}

		#endregion

		#region Formatting

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or Int32.MaxValue for
		/// the last line.</param>
		/// <returns></returns>
		public virtual string GetLineMarkup(int lineIndex)
		{
			string text = GetLineText(lineIndex, 0, Int32.MaxValue);

			return PangoUtility.Escape(text);
		}

		/// <summary>
		/// Gets the name of the line style associated with this line. If the
		/// default style is desired, then this can return null. Otherwise, it
		/// has to be a name of an existing style. If this returns a style name
		/// that doesn't exist, then an exception will be thrown.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or Int32.MaxValue for
		/// the last line.</param>
		/// <returns></returns>
		public virtual string GetLineStyleName(int lineIndex)
		{
			return null;
		}

		#endregion

		#region Indicators

		/// <summary>
		/// Gets the line indicators for a given character range.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="startCharacterIndex">Start character in the line text.</param>
		/// <param name="endCharacterIndex">End character in the line text.</param>
		/// <returns></returns>
		public virtual IEnumerable<ILineIndicator> GetLineIndicators(
			int lineIndex,
			int startCharacterIndex,
			int endCharacterIndex)
		{
			return new List<ILineIndicator>();
		}

		/// <summary>
		/// Gets the line indicators for the entire line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="lineIndex">Index of the line.</param>
		/// <returns></returns>
		public IEnumerable<ILineIndicator> GetLineIndicators(
			IDisplayContext displayContext,
			int lineIndex)
		{
			return GetLineIndicators(lineIndex, 0, Int32.MaxValue);
		}

		#endregion

		#region Operations

		/// <summary>
		/// Performs the given operation, raising any events for changing.
		/// </summary>
		/// <param name="operation">The operation.</param>
		public abstract void Do(ILineBufferOperation operation);

		/// <summary>
		/// Used to indicate that a line changed.
		/// </summary>
		public event EventHandler<LineChangedArgs> LineChanged;

		/// <summary>
		/// Occurs when lines are inserted into the buffer.
		/// </summary>
		public event EventHandler<LineRangeEventArgs> LinesDeleted;

		/// <summary>
		/// Occurs when lines are inserted into the buffer.
		/// </summary>
		public event EventHandler<LineRangeEventArgs> LinesInserted;

		/// <summary>
		/// Raises the <see cref="LineChanged"/> event
		/// </summary>
		/// <param name="e">The e.</param>
		protected virtual void RaiseLineChanged(LineChangedArgs e)
		{
			var listeners = LineChanged;

			if (listeners != null)
			{
				listeners(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="LinesDeleted"/> event.
		/// </summary>
		/// <param name="e">The e.</param>
		protected virtual void RaiseLinesDeleted(LineRangeEventArgs e)
		{
			var listeners = LinesDeleted;

			if (listeners != null)
			{
				listeners(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="LinesInserted"/> event.
		/// </summary>
		/// <param name="e">The e.</param>
		protected virtual void RaiseLinesInserted(LineRangeEventArgs e)
		{
			var listeners = LinesInserted;

			if (listeners != null)
			{
				listeners(this, e);
			}
		}

		#endregion
	}
}