// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;

namespace MfGames.Commands.TextEditing
{
	/// <summary>
	/// A structure for representing a zero-based index.
	/// </summary>
	public struct Position
	{
		#region Methods

		/// <summary>
		/// Translates the magic number operations into a character index of
		/// a text string.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="lineText"></param>
		/// <returns></returns>
		public int Normalize(
			string lineText,
			Position position,
			bool goRight)
		{
			// All the magic values are negative, so if we don't have one, there is
			// nothing to do.
			if (Index >= 0)
			{
				return Index;
			}

			// If we have the end magic number, then the index is equal to the end
			// of the text line.
			if (Index == End.Index)
			{
				return lineText.Length;
			}

			// If we have the word, then we go either left or right.
			if (Index == Word.Index)
			{
				if (goRight)
				{
					int index = Math.Min(position.Index + 5, lineText.Length);
					return index;
				}
				else
				{
					int index = Math.Max(position.Index - 5, 0);
					return index;
				}
			}

			// If we got this far, we don't know how to process this.
			throw new IndexOutOfRangeException("Encountered an invalid index: " + Index);
		}

		public int Normalize(string lineText)
		{
			// All the magic values are negative, so if we don't have one, there is
			// nothing to do.
			if (Index >= 0)
			{
				return Index;
			}

			// If we have the end magic number, then the index is equal to the end
			// of the text line.
			if (Index == End.Index)
			{
				return lineText.Length;
			}

			// If we got this far, we don't know how to process this.
			throw new IndexOutOfRangeException("Encountered an invalid index: " + Index);
		}

		#endregion

		#region Operators

		public static explicit operator Position(int index)
		{
			var position = new Position(index);
			return position;
		}

		public static implicit operator int(Position position)
		{
			int index = position.Index;
			return index;
		}

		#endregion

		#region Constructors

		public Position(int index)
		{
			Index = index;
		}

		#endregion

		#region Fields

		/// <summary>
		/// A position that represents the beginning of a line or range.
		/// </summary>
		public static Position Begin = new Position(0);

		/// <summary>
		/// A magic number that represents the end of a buffer (for Line) or the end
		/// of the line (for Character).
		/// </summary>
		public static readonly Position End = new Position(-113);

		/// <summary>
		/// Contains the zero-based index for the position.
		/// </summary>
		public int Index;

		/// <summary>
		/// A magic number that represents a word break for the line.
		/// </summary>
		public static readonly Position Word = new Position(-1053);

		#endregion
	}
}
