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

using C5;

using MfGames.GtkExt.LineTextEditor.Interfaces;
using MfGames.GtkExt.LineTextEditor.Visuals;

using Pango;

using Rectangle=Cairo.Rectangle;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Buffers
{
	/// <summary>
	/// Implements a simple cache ILineLayoutBuffer that keeps the various
	/// heights in memory to allow for rapid retrieval of line heights. This
	/// uses the idea of cache windows to keep track of individual lines while
	/// allow a window to be unloaded but the height of a line range to be
	/// retained.
	/// </summary>
	public class CachedLineLayoutBuffer : LineLayoutBufferProxy
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedLineLayoutBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public CachedLineLayoutBuffer(ILineLayoutBuffer buffer)
			: this(buffer, 8, 128)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedLineLayoutBuffer"/> class.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="maximumLoadedWindows">The maximum loaded windows.</param>
		/// <param name="windowSize">Size of the window.</param>
		public CachedLineLayoutBuffer(
			ILineLayoutBuffer buffer,
			int maximumLoadedWindows,
			int windowSize)
			: base(buffer)
		{
			// Set the cache window properties.
			this.windowSize = windowSize;

			// Create the collection of windows.
			windows = new ArrayList<CachedWindow>();

			// Pre-create the window arrays.
			allocatedLines = new ArrayList<CachedLine[]>();

			for (int index = 0; index < maximumLoadedWindows; index++)
			{
				// Create the array and add it to the allocated list.
				var lines = new CachedLine[windowSize];

				allocatedLines.Add(lines);

				for (int line = 0; line < windowSize; line++)
				{
					lines[line] = new CachedLine();
				}
			}
		}

		#endregion

		#region Caches

		/// <summary>
		/// Contains a list of allocated lines that were created but are currently
		/// not in use. This is to avoid memory pressure by allocating them once
		/// and not freeing the memory until the class is freed.
		/// </summary>
		private readonly ArrayList<CachedLine[]> allocatedLines;

		/// <summary>
		/// Contains all the windows in this cache.
		/// </summary>
		private readonly ArrayList<CachedWindow> windows;

		/// <summary>
		/// Contains the size of the individual windows.
		/// </summary>
		private readonly int windowSize;

		/// <summary>
		/// Goes through and makes sure all the windows are allocated for the
		/// underlying buffer.
		/// </summary>
		private void AllocateWindows()
		{
			// If we have no lines, then we don't need any buffers.
			int lineCount = LineMarkupBuffer.LineCount;

			if (lineCount == 0)
			{
				Clear();
			}

			// We need a window for every windowSize lines.
			int windowsNeeded = lineCount / windowSize;

			if (lineCount % windowSize > 0)
			{
				windowsNeeded++;
			}

			// If we don't have enough, allocate more.
			while (windows.Count < windowsNeeded)
			{
				windows.Add(new CachedWindow(this, windows.Count));
			}

			// If we have too many, then free them.
			while (windows.Count > windowsNeeded)
			{
				CachedWindow window = windows.RemoveLast();
				Clear(window);
			}
		}

		/// <summary>
		/// Clears out all the windows and returns the larger arrays back into
		/// the allocation list.
		/// </summary>
		private void Clear()
		{
			// Go through all the windows and returned the allocated lines back
			// to the list.
			foreach (CachedWindow window in windows)
			{
				Clear(window);
			}

			// Clear out the array.
			windows.Clear();
		}

		/// <summary>
		/// Clears the specified window of allocations.
		/// </summary>
		/// <param name="window">The window.</param>
		private void Clear(CachedWindow window)
		{
			if (window.Lines != null)
			{
				// Clear out the lines to make sure the garbage collection can
				// release them as needed.
				foreach (CachedLine line in window.Lines)
				{
					line.Height = 0;
					line.Style = null;
					line.Layout = null;
				}

				// Move the lines list back into the allocation list.
				allocatedLines.Add(window.Lines);
				window.Lines = null;
			}
		}

		/// <summary>
		/// Clears the least recently used window that has lines.
		/// </summary>
		private void ClearLeastRecentlyUsedWindow()
		{
			// Go through the windows and find the cache window that has the
			// oldest data.
			int lruWindowIndex = -1;
			DateTime lruWindowAccessed = DateTime.MaxValue;

			for (int index = 0; index < windows.Count; index++)
			{
				// If the window doesn't have lines, we don't use it.
				CachedWindow window = windows[index];

				if (window.Lines == null)
				{
					continue;
				}

				// Check to see if this window is older than the current.
				if (window.LastAccessed < lruWindowAccessed)
				{
					lruWindowAccessed = window.LastAccessed;
					lruWindowIndex = index;
				}
			}

			// The index will contains the last index.
			if (lruWindowIndex == -1)
			{
				throw new Exception("Cannot find LRU cache window");
			}

			// Clear this window.
			Clear(windows[lruWindowIndex]);
		}

		/// <summary>
		/// Gets the index of the window for a given line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		private int GetWindowIndex(int line)
		{
			// Figure out the window based on the windowSize.
			int window = line / windowSize;
			return window;
		}

		/// <summary>
		/// Marks the individual windows as needing to recalculate their heights.
		/// </summary>
		public override void Reset()
		{
			foreach (CachedWindow window in windows)
			{
				window.Reset();
				Clear(window);
			}
		}

		#endregion

		#region Line Layout

		/// <summary>
		/// Sets the width on the underlying buffer and resets the cache windows
		/// if the width changes.
		/// </summary>
		/// <value>The width.</value>
		public override int Width
		{
			set
			{
				// Check to see if we have a chance. If we don't, then we don't
				// have to do anything.
				if (base.Width == value)
				{
					return;
				}

				// Reset the cache windows since the line layout didn't change,
				// but we have to recalculate all the heights again.
				Reset();

				// Set the new width in the underlying buffer.
				base.Width = value;
			}
		}

		/// <summary>
		/// Gets the line layout for a given line.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="line">The line.</param>
		/// <returns></returns>
		public override Layout GetLineLayout(
			TextEditor textEditor,
			int line)
		{
			// Make sure we have all the windows allocated.
			AllocateWindows();

			// Go through the windows and find the starting one.
			int windowIndex = GetWindowIndex(line);
			CachedWindow window = windows[windowIndex];

			// Get the layout from the window.
			Layout layout = window.GetLineLayout(textEditor, line);
			return layout;
		}

		/// <summary>
		/// Uses the cache to retrieve the heights of the individual lines.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		/// <returns></returns>
		public override int GetLineLayoutHeight(
			TextEditor textEditor,
			int startLine,
			int endLine)
		{
			// Make sure we have all the windows allocated.
			AllocateWindows();

			// If we got a negative line, then set it to the end.
			if (endLine == -1)
			{
				endLine = LineCount - 1;
			}

			// Go through the windows and find the starting one.
			int startingWindowIndex = GetWindowIndex(startLine);
			int endingWindowIndex = GetWindowIndex(endLine);

			CachedWindow startingWindow = windows[startingWindowIndex];
			CachedWindow endingWindow = windows[endingWindowIndex];

			// Make sure that both the starting and ending windows are populated.
			// This handles if the windows are the same since Populate() checks
			// the loaded status.
			startingWindow.Populate(textEditor);
			endingWindow.Populate(textEditor);

			// Get the height of the lines inside the starting window.
			int height = startingWindow.GetLineLayoutHeight(
				textEditor, startLine, endLine);

			// If the end window is different, get those line heights also.
			if (startingWindowIndex != endingWindowIndex)
			{
				height += endingWindow.GetLineLayoutHeight(textEditor, startLine, endLine);
			}

			// Retrieve all the cache windows between the two ranges.
			for (int windowIndex = startingWindowIndex + 1;
			     windowIndex < endingWindowIndex;
			     windowIndex++)
			{
				CachedWindow window = windows[windowIndex];
				height += window.GetLineLayoutHeight(textEditor);
			}

			// Return the resulting height of the region.
			return height;
		}

		/// <summary>
		/// Gets the lines that are visible in the given view area.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="viewArea">The view area.</param>
		/// <param name="startLine">The start line.</param>
		/// <param name="endLine">The end line.</param>
		public override void GetLineLayoutRange(
			TextEditor textEditor,
			Rectangle viewArea,
			out int startLine,
			out int endLine)
		{
			// Go through and find the windows that have the starting and ending
			// area.
			int height = 0;
			int startWindowIndex = -1;
			int endWindowIndex = -1;
			int startWindowHeight = 0;
			int endWindowHeight = 0;
			double bottom = viewArea.Y + viewArea.Height;

			foreach (CachedWindow window in windows)
			{
				// Get the window height.
				int windowHeight = window.GetLineLayoutHeight(textEditor);

				// Check for starting line.
				if (viewArea.Y >= height && viewArea.Y <= height + windowHeight)
				{
					// The starting line is somewhere in this window.
					startWindowIndex = window.WindowIndex;
					startWindowHeight = height;
				}

				// Check for ending line.
				if (bottom >= height && bottom <= height + windowHeight)
				{
					// The starting line is somewhere in this window.
					endWindowIndex = window.WindowIndex;
					endWindowHeight = height;
				}

				// If we have both a start and end window, we're done with this
				// loop and can process it.
				if (startWindowIndex >= 0 && endWindowIndex >= 0)
				{
					break;
				}

				// Add to the current height.
				height += windowHeight;
			}

			// Make sure we have a starting and ending line index. If we don't have
			// a starting line, then just show the last one.
			if (startWindowIndex == -1)
			{
				startLine = endLine = LineCount - 1;
				return;
			}

			// Determine what the start line is inside the starting cache.
			var startWindowOffset = (int) (viewArea.Y - startWindowHeight);
			startLine = windows[startWindowIndex].GetLineLayoutContaining(
				textEditor, startWindowOffset);

			// Get the ending line from the ending cache.
			if (endWindowIndex == -1)
			{
				endLine = LineCount - 1;
				return;
			}

			var endWindowOffset = (int) (viewArea.Y + viewArea.Height - endWindowHeight);
			endLine = windows[endWindowIndex].GetLineLayoutContaining(
				textEditor, endWindowOffset);
		}

		/// <summary>
		/// Gets the line style for a given line.
		/// </summary>
		/// <param name="textEditor">The text editor.</param>
		/// <param name="line">The line number.</param>
		/// <returns></returns>
		public override BlockStyle GetLineStyle(
			TextEditor textEditor,
			int line)
		{
			// Make sure we have all the windows allocated.
			AllocateWindows();

			// Go through the windows and find the starting one.
			int windowIndex = GetWindowIndex(line);
			CachedWindow window = windows[windowIndex];

			// Get the layout from the window.
			BlockStyle style = window.GetLineStyle(textEditor, line);
			return style;
		}

		#endregion

		#region Nested type: CachedLine

		/// <summary>
		/// Contains information about a single cached line in memory. This
		/// contains information about the height and style for a given line.
		/// </summary>
		internal class CachedLine
		{
			#region Properties

			/// <summary>
			/// Gets or sets the height of the line.
			/// </summary>
			/// <value>The height.</value>
			public int Height { get; set; }

			/// <summary>
			/// Gets or sets the Pango layout for the line.
			/// </summary>
			/// <value>The layout.</value>
			public Layout Layout { get; set; }

			/// <summary>
			/// Gets or sets the style for the line.
			/// </summary>
			/// <value>The style.</value>
			public BlockStyle Style { get; set; }

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
				return string.Format("CachedLine: Height={0}", Height);
			}

			#endregion
		}

		#endregion

		#region Nested type: CachedWindow

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
			/// Gets or sets the last accessed timestamp.
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
				get { return WindowStartLine + ParentBuffer.windowSize - 1; }
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
				get { return WindowIndex * ParentBuffer.windowSize; }
			}

			#endregion

			#region Caching

			/// <summary>
			/// Gets the line layout for a given line.
			/// </summary>
			/// <param name="textEditor">The text editor.</param>
			/// <param name="line">The line index which may not be in the window.</param>
			/// <returns></returns>
			public Layout GetLineLayout(
				TextEditor textEditor,
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
				Populate(textEditor);

				// Get the line index inside the window.
				int windowLineIndex = line - WindowStartLine;

				return Lines[windowLineIndex].Layout;
			}

			/// <summary>
			/// Gets the line layout containing the Y coordinate relative to the
			/// cache window.
			/// </summary>
			/// <param name="textEditor">The text editor.</param>
			/// <param name="y">The window-relative Y pixels.</param>
			/// <returns></returns>
			public int GetLineLayoutContaining(
				TextEditor textEditor,
				int y)
			{
				// We need to have this window populated.
				Populate(textEditor);

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
			/// <param name="textEditor">The text editor.</param>
			/// <returns></returns>
			internal int GetLineLayoutHeight(TextEditor textEditor)
			{
				// Check to see if we already have it populated.
				if (Height.HasValue)
				{
					return Height.Value;
				}

				// Use the text editor to populate the height.
				Height = ParentBuffer.LineLayoutBuffer.GetLineLayoutHeight(
					textEditor, WindowStartLine, WindowEndLine);
				return Height.Value;
			}

			/// <summary>
			/// Gets the height of the text for individual lines
			/// </summary>
			/// <param name="textEditor">The text editor.</param>
			/// <param name="startLine">The start line which may not be in the window.</param>
			/// <param name="endLine">The end line which may not be in the window.</param>
			/// <returns></returns>
			internal int GetLineLayoutHeight(
				TextEditor textEditor,
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
					return GetLineLayoutHeight(textEditor);
				}

				// Make sure we're populated since we are getting individual
				// lines.
				Populate(textEditor);

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
			/// <param name="textEditor">The text editor.</param>
			/// <param name="line">The line index which may not be in the window.</param>
			/// <returns></returns>
			public BlockStyle GetLineStyle(
				TextEditor textEditor,
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
				Populate(textEditor);

				// Get the line index inside the window.
				int windowLineIndex = line - WindowStartLine;

				return Lines[windowLineIndex].Style;
			}

			/// <summary>
			/// Populates the individual lines within a cache window.
			/// </summary>
			/// <param name="textEditor">The text editor.</param>
			internal void Populate(TextEditor textEditor)
			{
				// If the window already has lines, then it doesn't need to be populated.
				if (Lines != null)
				{
					return;
				}

				// Get an array of lines from the list.
				if (ParentBuffer.allocatedLines.Count <= 0)
				{
					// We don't have any allocated lines, so free the last.
					ParentBuffer.ClearLeastRecentlyUsedWindow();
				}

				Lines = ParentBuffer.allocatedLines.Pop();
				LastAccessed = DateTime.UtcNow;

				// Go through all the lines in the window and populate them.
				int height = 0;

				for (int lineIndex = 0; lineIndex < ParentBuffer.windowSize; lineIndex++)
				{
					// Make sure we aren't going past the top line.
					int line = WindowStartLine + lineIndex;

					if (line > ParentBuffer.LineCount)
					{
						// Just reset the line.
						Lines[lineIndex].Height = 0;
						Lines[lineIndex].Style = null;
						Lines[lineIndex].Layout = null;
						continue;
					}

					// Get the height of this line.
					Lines[lineIndex].Height =
						ParentBuffer.LineLayoutBuffer.GetLineLayoutHeight(textEditor, line, line);
					Lines[lineIndex].Style =
						ParentBuffer.LineLayoutBuffer.GetLineStyle(textEditor, line);
					Lines[lineIndex].Layout =
						ParentBuffer.LineLayoutBuffer.GetLineLayout(textEditor, line);
					height += Lines[lineIndex].Height;
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
				return string.Format(
					"CachedWindow #{0}: Height={1}, HasLines={2}",
					WindowIndex,
					Height,
					Lines != null);
			}

			#endregion
		}

		#endregion
	}
}