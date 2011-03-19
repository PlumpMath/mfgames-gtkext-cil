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

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Implements a class that takes the operations and splits them into 
	/// protected functions to simplify extending the operations.
	/// </summary>
	public abstract class MultiplexedOperationLineBuffer : LineBuffer
	{
		#region Operations

		/// <summary>
		/// Performs the given operation on the line buffer. This will raise any
		/// events that were appropriate for the operation.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		public override LineBufferOperationResults Do(ILineBufferOperation operation)
		{
			// Check for null values.
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}

			// Break out the operation and call the appropriate function.
			switch (operation.OperationType)
			{
				case LineBufferOperationType.SetText:
					return Do((SetTextOperation) operation);

				case LineBufferOperationType.InsertText:
					return Do((InsertTextOperation) operation);

				case LineBufferOperationType.DeleteText:
					return Do((DeleteTextOperation) operation);

				case LineBufferOperationType.DeleteLines:
					return Do((DeleteLinesOperation) operation);

				case LineBufferOperationType.InsertLines:
					return Do((InsertLinesOperation) operation);

				case LineBufferOperationType.ExitLine:
					return Do((ExitLineOperation) operation);

				default:
					throw new ArgumentOutOfRangeException(
						"operation", "Operation implements unknown OperationType.");
			}
		}

		/// <summary>
		/// Indicates that the user has exited the line.
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <returns></returns>
		protected virtual LineBufferOperationResults Do(ExitLineOperation operation)
		{
			return new LineBufferOperationResults();
		}

		/// <summary>
		/// Inserts text into the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected abstract LineBufferOperationResults Do(
			InsertTextOperation operation);

		/// <summary>
		/// Deletes text from the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected abstract LineBufferOperationResults Do(
			DeleteTextOperation operation);

		/// <summary>
		/// Performs the set text operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected abstract LineBufferOperationResults Do(SetTextOperation operation);

		/// <summary>
		/// Performs the insert lines operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected abstract LineBufferOperationResults Do(
			InsertLinesOperation operation);

		/// <summary>
		/// Performs the delete lines operation on the buffer.
		/// </summary>
		/// <param name="operation">The operation to perform.</param>
		/// <returns>
		/// The results to the changes to the buffer.
		/// </returns>
		protected abstract LineBufferOperationResults Do(
			DeleteLinesOperation operation);

		#endregion
	}
}