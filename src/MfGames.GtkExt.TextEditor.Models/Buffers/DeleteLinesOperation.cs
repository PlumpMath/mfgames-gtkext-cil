// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using MfGames.Commands.TextEditing;

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Indicates an operation that inserts lines into a line buffer.
	/// </summary>
	public class DeleteLinesOperation: ILineBufferOperation,
		IDeleteLineCommand<OperationContext>
	{
		#region Properties

		public bool CanUndo
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the number of lines to delete.
		/// </summary>
		/// <value>The count.</value>
		public int Count { get; private set; }

		public bool IsTransient
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the index of the first line to start deleting.
		/// </summary>
		/// <value>The index of the line.</value>
		public int Line { get; private set; }

		/// <summary>
		/// Gets the type of the operation representing this object.
		/// </summary>
		/// <value>The type of the operation.</value>
		public LineBufferOperationType OperationType
		{
			get { return LineBufferOperationType.DeleteLines; }
		}

		#endregion

		#region Methods

		public void Do(OperationContext state)
		{
			// Delete the line from the buffer.
			state.LineBuffer.DeleteLines(Line, 1);
		}

		public void Redo(OperationContext state)
		{
			throw new NotImplementedException();
		}

		public void Undo(OperationContext state)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteLinesOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="count">The count.</param>
		public DeleteLinesOperation(
			int lineIndex,
			int count)
		{
			Line = lineIndex;
			Count = count;
		}

		#endregion
	}
}
