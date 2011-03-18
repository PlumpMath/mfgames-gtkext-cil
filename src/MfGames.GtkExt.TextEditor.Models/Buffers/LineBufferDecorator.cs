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
using System.Diagnostics;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Abstract class which wraps around a <see cref="LineBuffer"/> and allows for
	/// overriding of various methods and properties. This implementation simply
	/// calls the underlying <see cref="LineBuffer"/>.
	/// </summary>
	public abstract class LineBufferDecorator : LineBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LineBufferDecorator"/> class.
		/// </summary>
		/// <param name="lineBuffer">The buffer.</param>
		protected LineBufferDecorator(LineBuffer lineBuffer)
		{
			if (lineBuffer == null)
			{
				throw new ArgumentNullException("lineBuffer");
			}

			this.lineBuffer = lineBuffer;
			this.lineBuffer.LineChanged += OnLineChanged;
			this.lineBuffer.LinesInserted += OnLinesInserted;
			this.lineBuffer.LinesDeleted += OnLinesDeleted;
		}

		#endregion

		#region Buffer

		private readonly LineBuffer lineBuffer;

		/// <summary>
		/// Gets the underlying buffer for this proxy.
		/// </summary>
		/// <value>The buffer.</value>
		protected LineBuffer LineBuffer
		{
			[DebuggerStepThrough]
			get { return lineBuffer; }
		}

		/// <summary>
		/// Gets the number of lines in the buffer.
		/// </summary>
		/// <value>The line count.</value>
		public override int LineCount
		{
			get { return LineBuffer.LineCount; }
		}

		/// <summary>
		/// If set to <see langword="true"/>, the buffer is read-only and the editing
		/// commands should throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public override bool ReadOnly
		{
			get { return LineBuffer.ReadOnly; }
		}

		#endregion

		#region Lines

		/// <summary>
		/// Gets the length of the line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>The length of the line.</returns>
		public override int GetLineLength(int lineIndex)
		{
			return LineBuffer.GetLineLength(lineIndex);
		}

		/// <summary>
		/// Gets the formatted line number for a given line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer.</param>
		/// <returns>A formatted line number.</returns>
		public override string GetLineNumber(int lineIndex)
		{
			return LineBuffer.GetLineNumber(lineIndex);
		}

		/// <summary>
		/// Gets the text of a given line in the buffer.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer. If the index is beyond the end of the buffer, the last line is used.</param>
		/// <param name="characters">The character range to pull the text.</param>
		/// <returns></returns>
		public override string GetLineText(
			int lineIndex,
			CharacterRange characters)
		{
			return LineBuffer.GetLineText(lineIndex, characters);
		}

		#endregion

		#region Formatting

		/// <summary>
		/// Gets the Pango markup for a given line.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or Int32.MaxValue for
		/// the last line.</param>
		/// <returns></returns>
		public override string GetLineMarkup(int lineIndex)
		{
			return LineBuffer.GetLineMarkup(lineIndex);
		}

		/// <summary>
		/// Gets the name of the line style associated with this line. If the
		/// default style is desired, then this can return <see langword="null"/>. Otherwise, it
		/// has to be a name of an existing style. If this returns a style name
		/// that doesn't exist, then an exception will be thrown.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or Int32.MaxValue for
		/// the last line.</param>
		/// <returns></returns>
		public override string GetLineStyleName(int lineIndex)
		{
			return LineBuffer.GetLineStyleName(lineIndex);
		}

		#endregion

		#region Operations

		/// <summary>
		/// Performs the given operation, raising any events for changing.
		/// </summary>
		/// <param name="operation">The operation.</param>
		public override void Do(ILineBufferOperation operation)
		{
			LineBuffer.Do(operation);
		}

		private void OnLineChanged(
			object sender,
			LineChangedArgs e)
		{
			RaiseLineChanged(e);
		}

		private void OnLinesDeleted(
			object sender,
			LineRangeEventArgs e)
		{
			RaiseLinesDeleted(e);
		}

		private void OnLinesInserted(
			object sender,
			LineRangeEventArgs e)
		{
			RaiseLinesInserted(e);
		}

		#endregion
	}
}