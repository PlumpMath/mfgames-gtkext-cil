using System;

namespace MfGames.GtkExt.LineTextEditor
{
	/// <summary>
	/// Represents a virtualized collection of lines for viewing and
	/// editing.
	/// </summary>
	public interface ILineBuffer
	{
		#region Buffer Events
		
		event EventHandler LineCountChanged;
		event EventHandler LineChanged;
		event EventHandler LineInserted;
		event EventHandler LineDeleted;
		event EventHandler LineBufferChanged;

		#endregion
		
		#region Buffer Viewing
		
		int LineCount { get; }

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		bool ReadOnly { get; }

		string GetLineText(
			int line,
		    int startIndex,
		    int endIndex);
		
		string GetLineNumber(
			int line);
		
		int GetLineLength(
			int line);

		#endregion
		
		#region Buffer Editing
		
		void InsertLines(
			int afterLine,
		    int count);
		
		void DeleteLines(
		    int startLine,
		    int endLine);
		
		void SetLineText(
		    int line,
		    int startIndex,
		    int endIndex,
		    string text);
		
		#endregion
	}
}
