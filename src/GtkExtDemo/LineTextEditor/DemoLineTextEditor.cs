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

using Gtk;

using MfGames.GtkExt.LineTextEditor;
using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace GtkExtDemo.LineTextEditor
{
	/// <summary>
	/// Contains the basic control for showing off the features of the line
	/// text editor.
	/// </summary>
	public class DemoLineTextEditor : VBox
	{
		public DemoLineTextEditor()
		{
			// Create a memory line buffer with random text so the user can
			// edit the buffer.
			ILineBuffer patternLineBuffer = new PatternLineBuffer(1024, 256, 4);
			ILineBuffer lineBuffer = new MemoryLineBuffer(patternLineBuffer);

			// Create an unformatted markup buffer and simple layout with a cache.
			ILineMarkupBuffer lineMarkupBuffer =
				new UnformattedLineMarkupBuffer(lineBuffer);
			ILineLayoutBuffer lineLayoutBuffer =
				new SimpleLineLayoutBuffer(lineMarkupBuffer);
			ILineLayoutBuffer cachedLayoutBuffer =
				new CachedLineLayoutBuffer(lineLayoutBuffer);

			// Create the text editor with the resulting buffer.
			var textEditor = new TextEditor(cachedLayoutBuffer);

			// Wrap the text editor in a scrollbar.
			var scrolledWindow = new ScrolledWindow();
			scrolledWindow.VscrollbarPolicy = PolicyType.Always;
			scrolledWindow.Add(textEditor);

			// Add the editor to the current tab.
			PackStart(scrolledWindow, true, true, 0);
		}
	}
}