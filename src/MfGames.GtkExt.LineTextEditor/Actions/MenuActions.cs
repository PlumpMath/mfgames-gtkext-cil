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

using Cairo;

using Gdk;

using Gtk;

using MfGames.GtkExt.LineTextEditor.Attributes;
using MfGames.GtkExt.LineTextEditor.Interfaces;

using Key=Gdk.Key;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Actions
{
    /// <summary>
    /// Contains the various actions used for moving the caret (cursor) around
    /// the text buffer.
    /// </summary>
    [ActionFixture]
    public static class MenuActions
    {
        #region Context Menu

        /// <summary>
        /// Moves the caret to the end of the buffer.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        [Action]
        [KeyBinding(Key.Return, ModifierType.Mod1Mask)]
        public static void ShowContextMenu(IActionContext actionContext)
        {
            // Get the coordinates to display the menu. Since the popup that
			// sets the coordinates based on the screen, we have to adjust from
			// text-specific coordinates clear to screen coordinates.

			// Start by getting the widget's screen coordinates.
			IDisplayContext displayContext = actionContext.DisplayContext;
			int screenX, screenY;

        	displayContext.GdkWindow.GetOrigin(
        		out screenX,
        		out screenY);

			// Figure out the position of the position in the screen. We add
			// the screen relative coordinate to the widget-relative, plus add
			// the height of a single line to shift it down slightly.
			int lineHeight;
            PointD point = displayContext.Caret.Position.ToScreenCoordinates(displayContext, out lineHeight);

        	int widgetY = (int) (screenY + lineHeight + point.Y);

            // Create the context menu to show.
            Menu menu = actionContext.CreateContextMenu();
            menu.ShowAll();
        	menu.Popup(
        		null,
        		null,
        		delegate(Menu delegateMenu,
        		         out int x,
        		         out int y,
        		         out bool pushIn)
        		{
        			x = screenX;
        			y = widgetY;
        			pushIn = true;
        		},
        		3,
        		Gtk.Global.CurrentEventTime);
        }

        #endregion
    }
}