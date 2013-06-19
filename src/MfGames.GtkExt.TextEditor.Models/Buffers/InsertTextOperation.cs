// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Diagnostics;
using System.Text;
using MfGames.Commands.TextEditing;

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Represents an operation to insert text into the buffer. Unlike
	/// <see cref="SetTextOperation"/>, this inserts text into a specific position
	/// and returns the buffer position for the end of the insert.
	/// </summary>
	public class InsertTextOperation: ILineBufferOperation,
		IInsertTextCommand<OperationContext>
	{
		#region Properties

		/// <summary>
		/// Gets or sets the buffer position for the insert operation.
		/// </summary>
		/// <value>The buffer position.</value>
		public TextPosition BufferPosition { get; private set; }

		public bool CanUndo
		{
			get { return true; }
		}

		public bool IsTransient
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the type of the operation representing this object.
		/// </summary>
		/// <value>The type of the operation.</value>
		public LineBufferOperationType OperationType
		{
			[DebuggerStepThrough] get { return LineBufferOperationType.InsertText; }
		}

		/// <summary>
		/// Gets the text for this operation.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		#endregion

		#region Methods

		public void Do(OperationContext state)
		{
			// Grab the line from the line buffer.
			string lineText = state.LineBuffer.GetLineText(
				BufferPosition.Line,
				LineContexts.Unformatted);
			var buffer = new StringBuilder(lineText);

			// Normalize the character ranges.
			int characterIndex = BufferPosition.Character.Normalize(lineText);

			buffer.Insert(characterIndex, Text);

			// Set the line in the buffer.
			lineText = buffer.ToString();
			state.LineBuffer.SetText(BufferPosition.Line, lineText);
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
		/// Initializes a new instance of the <see cref="InsertTextOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="characterIndex">Index of the character.</param>
		/// <param name="text">The text.</param>
		public InsertTextOperation(
			Position lineIndex,
			Position characterIndex,
			string text)
			: this(new TextPosition(lineIndex, characterIndex), text)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InsertTextOperation"/> class.
		/// </summary>
		/// <param name="bufferPosition">The buffer position.</param>
		/// <param name="text">The text.</param>
		public InsertTextOperation(
			TextPosition bufferPosition,
			string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			BufferPosition = bufferPosition;
			Text = text;
		}

		#endregion
	}
}
