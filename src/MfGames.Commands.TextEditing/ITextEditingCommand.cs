// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.Commands.TextEditing
{
	/// <summary>
	/// Represents the common functionality shared among all text editing commands
	/// including indicators for adjusting position and selection.
	/// </summary>
	/// <typeparam name="TContext">The context of the editing command.</typeparam>
	public interface ITextEditingCommand<in TContext>: IUndoableCommand<TContext>
	{
		#region Properties

		/// <summary>
		/// When set to true, then the command should update the text position
		/// (caret) of the context executing.
		/// </summary>
		bool UpdateTextPosition { get; set; }

		/// <summary>
		/// When set to true, then the command should update the text selection
		/// while executing.
		/// </summary>
		bool UpdateTextSelection { get; set; }

		#endregion
	}
}
