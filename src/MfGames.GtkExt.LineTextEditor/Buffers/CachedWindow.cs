#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

#region Namespaces

using System;

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a cache window that keeps track of a range of lines and
	/// their heights and allows for the individual lines to be unloaded
	/// but the overall height and bulk properties to be retained.
	/// </summary>
	internal class CachedWindow
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedWindow"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="windowIndex">Index of the cache window.</param>
		public CachedWindow(
			CachedLineLayoutBuffer buffer,
			int windowIndex)
		{
			ParentBuffer = buffer;
			WindowIndex = windowIndex;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the height of the entire cache window.
		/// </summary>
		/// <value>The height.</value>
		internal int? Height { get; set; }

		/// <summary>
		/// Gets or sets the last accessed time stamp.
		/// </summary>
		/// <value>The last accessed.</value>
		internal DateTime LastAccessed { get; set; }

		/// <summary>
		/// Gets or sets an array of lines within the buffer.
		/// </summary>
		/// <value>The lines.</value>
		internal CachedLine[] Lines { get; set; }

		/// <summary>
		/// Gets or sets the buffer.
		/// </summary>
		/// <value>The buffer.</value>
		internal CachedLineLayoutBuffer ParentBuffer { get; private set; }

		/// <summary>
		/// Gets the end line inside the window.
		/// </summary>
		/// <value>The end line.</value>
		internal int WindowEndLine
		{
			get { return WindowStartLine + ParentBuffer.WindowSize - 1; }
		}

		/// <summary>
		/// Gets or sets the index of the window.
		/// </summary>
		/// <value>The index of the window.</value>
		internal int WindowIndex { get; private set; }

		/// <summary>
		/// Gets the starting line inside the window..
		/// </summary>
		/// <value>The start line.</value>
		internal int WindowStartLine
		{
			get { return WindowIndex * ParentBuffer.WindowSize; }
		}

		#endregion

		#region Caching

		private bool needPopulate;

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="line">The line index which may not be in the window.</param>
		/// <returns></returns>
		public Layout GetLineLayout(
			IDisplayContext displayContext,
			int line)
		{
			// Make sure we are asking for lines in the range of the cache
			// window. If we ask for something outside of that, return 0.
			if (line > WindowEndLine)
			{
				return null;
			}

			if (line < WindowStartLine)
			{
				return null;
			}

			// We have to have this window populated.
			Populate(displayContext);

			// Get the line index inside the window.
			int windowLineIndex = line - WindowStartLine;

			return Lines[windowLineIndex].Layout;
		}

		/// <summary>
		/// Gets the line layout containing the Y coordinate relative to the
		/// cache window.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="y">The window-relative Y pixels.</param>
		/// <returns></returns>
		public int GetLineLayoutContaining(
			IDisplayContext displayContext,
			int y)
		{
			// We need to have this window populated.
			Populate(displayContext);

			// Go through the lines in the cache and find the line.
			int height = 0;

			for (int lineIndex = 0; lineIndex < Lines.Length; lineIndex++)
			{
				// Determine if the Y coordinate is inside the line.
				CachedLine cachedLine = Lines[lineIndex];

				if (y >= height && y <= height + cachedLine.Height)
				{
					return WindowStartLine + lineIndex;
				}

				// Add the height to the line.
				height += cachedLine.Height;
			}

			// We can't find it, so throw an exception.
			throw new Exception("Cannot find line at y coordinate: " + y);
		}

		/// <summary>
		/// Gets the line layout height for the entire cache window.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <returns></returns>
		internal int GetLineLayoutHeight(IDisplayContext displayContext)
		{
			// Check to see if we already have it populated.
			if (Height.HasValue)
			{
				return Height.Value;
			}

			// Use the text editor to populate the height.
			Height = ParentBuffer.LineLayoutBuffer.GetLineLayoutHeight(
				displayContext, WindowStartLine, WindowEndLine);
			return Height.Value;
		}

		/// <summary>
		/// Gets the height of the text for individual lines
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="startLine">The start line which may not be in the window.</param>
		/// <param name="endLine">The end line which may not be in the window.</param>
		/// <returns></returns>
		internal int GetLineLayoutHeight(
			IDisplayContext displayContext,
			int startLine,
			int endLine)
		{
			// Make sure we are asking for lines in the range of the cache
			// window. If we ask for something outside of that, return 0.
			if (startLine > WindowEndLine)
			{
				return 0;
			}

			if (endLine < WindowStartLine)
			{
				return 0;
			}

			// Check to see if we are getting the entire height. This is to
			// optimize some requests and to avoid populating the lines if
			// we don't need to.
			if (startLine == WindowStartLine && endLine == WindowEndLine)
			{
				// Entire buffer request so optimize it.
				return GetLineLayoutHeight(displayContext);
			}

			// Make sure we're populated since we are getting individual
			// lines.
			Populate(displayContext);

			// Clamp the lines based on the limits of the cache window.
			startLine = Math.Max(startLine, WindowStartLine);
			endLine = Math.Min(endLine, WindowEndLine);

			// Figure out the index of the lines inside the window.
			int startLineIndex = startLine - WindowStartLine;
			int endLineIndex = endLine - WindowStartLine;

			// Add up the height of all the lines.
			int height = 0;

			for (int lineIndex = startLineIndex; lineIndex <= endLineIndex; lineIndex++)
			{
				height += Lines[lineIndex].Height;
			}

			return height;
		}

		/// <summary>
		/// Gets the line style for a given line.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		/// <param name="line">The line index which may not be in the window.</param>
		/// <returns></returns>
		public BlockStyle GetLineStyle(
			IDisplayContext displayContext,
			int line)
		{
			// Make sure we are asking for lines in the range of the cache
			// window. If we ask for something outside of that, return 0.
			if (line > WindowEndLine)
			{
				return null;
			}

			if (line < WindowStartLine)
			{
				return null;
			}

			// We have to have this window populated.
			Populate(displayContext);

			// Get the line index inside the window.
			int windowLineIndex = line - WindowStartLine;

			return Lines[windowLineIndex].Style;
		}

		/// <summary>
		/// Populates the individual lines within a cache window.
		/// </summary>
		/// <param name="displayContext">The text editor.</param>
		internal void Populate(IDisplayContext displayContext)
		{
			// Update our access time.
			LastAccessed = DateTime.UtcNow;

			// If the window already has lines or if we don't have a specific
			// request to repopulate it, then don't do anything.
			if (Lines != null && !needPopulate)
			{
				return;
			}

			// Only allocate lines if we don't have one.
			if (Lines == null)
			{
				// Get an array of lines from the list.
				if (ParentBuffer.AllocatedLines.Count <= 0)
				{
					// We don't have any allocated lines, so free the last.
					ParentBuffer.ClearLeastRecentlyUsedWindow();
				}

				Lines = ParentBuffer.AllocatedLines.Pop();
			}

			// Go through all the lines in the window and populate them.
			int height = 0;

			for (int lineIndex = 0; lineIndex < ParentBuffer.WindowSize; lineIndex++)
			{
				// Make sure we aren't going past the top line.
				int line = WindowStartLine + lineIndex;
				CachedLine cachedLine = Lines[lineIndex];

				if (line >= ParentBuffer.LineCount)
				{
					// Just reset the line.
					cachedLine.Reset();
					continue;
				}

				// If we have a height, then don't process it.
				if (cachedLine.Height > 0)
				{
					continue;
				}

				// Get the height of this line.
				cachedLine.Height =
					ParentBuffer.LineLayoutBuffer.GetLineLayoutHeight(
						displayContext, line, line);
				cachedLine.Style = ParentBuffer.LineLayoutBuffer.GetLineStyle(
					displayContext, line);
				cachedLine.Layout =
					ParentBuffer.LineLayoutBuffer.GetLineLayout(displayContext, line);

				height += cachedLine.Height;
			}

			// Set the height of the window.
			Height = height;
		}

		/// <summary>
		/// Clears out any cached values inside the window.
		/// </summary>
		public void Reset()
		{
			// Reset the cached values in the window.
			Height = null;
		}

		/// <summary>
		/// Clears out any cached values inside the window along with
		/// a specific line.
		/// </summary>
		public void Reset(int windowLineIndex)
		{
			Reset();
			needPopulate = true;
			Lines[windowLineIndex].Reset();
		}

		#endregion

		#region Conversion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return String.Format(
				"CachedWindow #{0}: Height={1}, HasLines={2}",
				WindowIndex,
				Height,
				Lines != null);
		}

		#endregion
	}
}