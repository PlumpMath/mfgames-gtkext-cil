// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Diagnostics;

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	/// <summary>
	/// Represents an operation to insert text into the buffer. Unlike
	/// <see cref="SetTextOperation"/>, this deletes text at a specific position
	/// and returns the buffer position for resulting position.
	/// </summary>
	public class DeleteTextOperation: ILineBufferOperation
	{
		#region Properties

		/// <summary>
		/// Gets or sets the character range to delete.
		/// </summary>
		/// <value>The character range.</value>
		public CharacterRange CharacterRange { get; set; }

		/// <summary>
		/// Gets or sets the index of the line.
		/// </summary>
		/// <value>The index of the line.</value>
		public int LineIndex { get; set; }

		/// <summary>
		/// Gets the type of the operation representing this object.
		/// </summary>
		/// <value>The type of the operation.</value>
		public LineBufferOperationType OperationType
		{
			[DebuggerStepThrough] get { return LineBufferOperationType.DeleteText; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteTextOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="startCharacterIndex">Start index of the character.</param>
		/// <param name="endCharacterIndex">End index of the character.</param>
		public DeleteTextOperation(
			int lineIndex,
			int startCharacterIndex,
			int endCharacterIndex)
			: this(lineIndex, new CharacterRange(startCharacterIndex, endCharacterIndex))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteTextOperation"/> class.
		/// </summary>
		/// <param name="lineIndex">Index of the line.</param>
		/// <param name="characterRange">The character range.</param>
		public DeleteTextOperation(
			int lineIndex,
			CharacterRange characterRange)
		{
			if (lineIndex < 0)
			{
				throw new ArgumentOutOfRangeException(
					"lineIndex", "Line index cannot be negative.");
			}

			LineIndex = lineIndex;
			CharacterRange = characterRange;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteTextOperation"/> class.
		/// </summary>
		/// <param name="bufferPosition">The buffer position.</param>
		/// <param name="length">The length.</param>
		public DeleteTextOperation(
			BufferPosition bufferPosition,
			int length)
		{
			LineIndex = bufferPosition.LineIndex;
			CharacterRange = CharacterRange.FromLength(
				bufferPosition.CharacterIndex, length);
		}

		#endregion
	}
}
