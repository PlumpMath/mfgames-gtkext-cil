// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Diagnostics.Contracts;

namespace MfGames.Commands.TextEditing
{
	/// <summary>
	/// A selection in a text buffer with the two anchors of the selection
	/// being represented by <see cref="TextPosition"/>. This is an immutable class.
	/// </summary>
	public class TextSelection
	{
		#region Properties

		/// <summary>
		/// Contains the starting text position for the selection.
		/// </summary>
		public TextPosition Begin { get; private set; }

		/// <summary>
		/// Contains the ending text position for the selection.
		/// </summary>
		public TextPosition End { get; private set; }

		#endregion

		#region Constructors

		public TextSelection(
			TextPosition begin,
			TextPosition end)
		{
			// Establish our code contracts.
			Contract.Requires<ArgumentNullException>(begin != null);
			Contract.Requires<ArgumentNullException>(end != null);

			// Save the positions as member variables.
			Begin = begin;
			End = end;
		}

		#endregion
	}
}
