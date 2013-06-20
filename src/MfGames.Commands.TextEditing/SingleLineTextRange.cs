namespace MfGames.Commands.TextEditing
{
	/// <summary>
	/// Represents a range of characters on a single line.
	/// </summary>
	public class SingleLineTextRange
	{
		#region Properties

		/// <summary>
		/// Contains the beginning character position in the line.
		/// </summary>
		public Position CharacterBegin { get; private set; }

		/// <summary>
		/// Contains the ending character position in the line.
		/// </summary>
		public Position CharacterEnd { get; private set; }

		/// <summary>
		/// Contains the line to modify.
		/// </summary>
		public Position Line { get; private set; }

		#endregion

		#region Constructors

		public SingleLineTextRange(
			Position line,
			Position characterBegin,
			Position characterEnd)
		{
			Line = line;
			CharacterBegin = characterBegin;
			CharacterEnd = characterEnd;
		}

		#endregion
	}
}