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

using System.Diagnostics;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Defines an operation that changes text of a single line.
	/// </summary>
	public class SetTextOperation : ILineBufferOperation
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SetTextOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="text">The text.</param>
		public SetTextOperation(
			int lineIndex,
			string text)
		{
			LineIndex = lineIndex;
			Text = text;
		}

		#endregion

		#region Operation

		/// <summary>
		/// Gets the type of the operation representing this object.
		/// </summary>
		/// <value>The type of the operation.</value>
		public LineBufferOperationType LineBufferOperationType
		{
			[DebuggerStepThrough]
			get { return LineBufferOperationType.SetText; }
		}

		/// <summary>
		/// Gets or sets the index of the line.
		/// </summary>
		/// <value>The index of the line.</value>
		public int LineIndex { get; private set; }

		/// <summary>
		/// Gets the text for this operation.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		#endregion
	}
}