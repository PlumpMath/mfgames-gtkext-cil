// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands.TextEditing
{
	/// <summary>
	/// A structure for representing a zero-based index.
	/// </summary>
	public struct Position
	{
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

		#endregion
	}
}
