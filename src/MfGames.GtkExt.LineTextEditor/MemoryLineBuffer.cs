using System;
using System.Collections.Generic;

namespace MfGames.GtkExt.LineTextEditor
{
	public class MemoryLineBuffer : ILineBuffer
	{
		#region Constructors
		
		public MemoryLineBuffer ()
		{
			lines = new List<string>();
		}
		
		#endregion

		#region Buffer Events
		
		public event EventHandler LineCountChanged;
		public event EventHandler LineChanged;
		public event EventHandler LineInserted;
		public event EventHandler LineDeleted;
		public event EventHandler LineBufferChanged;

		#endregion
		
		#region Buffer Viewing

		private List<string> lines;
		private bool readOnly;
		
		public int LineCount { get { return lines.Count; } }

		/// <summary>
		/// If set to true, the buffer is read-only and the editing commands
		/// should throw an InvalidOperationException.
		/// </summary>
		public bool ReadOnly
		{
			get { return readOnly; }
			set { readOnly = value; }
		}

		public string GetLineText(
			int line,
		    int startIndex,
		    int endIndex)
		{
			// Get the entire line from the buffer. This will throw the argument
			// out of range exception if the line can't be found.
			string text = lines[line];
			
			// If we are from 0 to -1, then we want the entire string and don't
			// need to do anything else.
			if (startIndex == 0 && endIndex == -1)
			{
				return text;
			}
			
			// Make sure the indexes are normalized. The end index can be -1 which
			// means the end of the string.
			if (endIndex == -1)
			{
				endIndex = text.Length;
			}
			
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			
			if (startIndex > endIndex)
			{
				throw new ArgumentOutOfRangeException("endIndex cannot be before startIndex");
			}
			
			// Substrings use lengths, not end indexes.
			int length = endIndex - startIndex;
			
			// Return the substring of the text based on indexes.
			return text.Substring(startIndex, length);
		}
		
		public string GetLineNumber(
			int line)
		{
			// Line numebers are given as 1-based instead of 0-based.
			return (line + 1).ToString("N0");
		}
		
		public int GetLineLength(
			int line)
		{
			return lines[line].Length;
		}

		#endregion
		
		#region Buffer Editing
		
		public void InsertLines(
			int afterLine,
		    int count)
		{
			if (afterLine == -1)
			{
				afterLine = lines.Count;
			}
			
			for (int index = 0; index < count; index++)
			{
				lines.Insert(afterLine, string.Empty);
			}
		}
		
		public void DeleteLines(
		    int startLine,
		    int endLine)
		{
			if (endLine == -1)
			{
				endLine = lines.Count - 1;
			}
			
			if (startLine > endLine)
			{
				throw new ArgumentOutOfRangeException("endLine cannot be before startLine");
			}
			
			int count = endLine - startLine;
			lines.RemoveRange(startLine, count);
		}
		
		public void SetLineText(
		    int line,
		    int startIndex,
		    int endIndex,
		    string text)
		{
			// If we don't have a valid text, throw an exception.
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			
			// Get the entire line from the buffer. This will throw the argument
			// out of range exception if the line can't be found.
			string lineText = lines[line];
			
			// If we are from 0 to -1, then we want to replace the entire string.
			if (startIndex == 0 && endIndex == -1)
			{
				lines[line] = text;
				return;
			}
			
			// Make sure the indexes are normalized. The end index can be -1 which
			// means the end of the string.
			if (endIndex == -1)
			{
				endIndex = lineText.Length;
			}
			
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			
			if (startIndex > endIndex)
			{
				throw new ArgumentOutOfRangeException("endIndex cannot be before startIndex");
			}
			
			// Break out the components of the intial line before and after the
			// indexes.
			string before = lineText.Substring(0, startIndex);
			string after = lineText.Substring(endIndex);
			
			// Add all three together and put them back into the array.
			lines[line] = before + text + after;
		}
		
		#endregion
	}
}
