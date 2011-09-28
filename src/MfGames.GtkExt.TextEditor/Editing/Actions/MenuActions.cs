#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using Cairo;

using Gdk;

using Gtk;

using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Renderers;

using Global = Gtk.Global;
using Key = Gdk.Key;

#endregion

namespace MfGames.GtkExt.TextEditor.Editing.Actions
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
        /// <param name="controller">The action context.</param>
        [Action]
        [KeyBinding(Key.Return, ModifierType.Mod1Mask)]
        public static void ShowContextMenu(EditorViewController controller)
        {
            // Get the coordinates to display the menu. Since the popup that
            // sets the coordinates based on the screen, we have to adjust from
            // text-specific coordinates clear to screen coordinates.

            // Start by getting the widget's screen coordinates.
            IDisplayContext displayContext = controller.DisplayContext;
            int screenX, screenY;

            displayContext.GdkWindow.GetOrigin(out screenX, out screenY);

            // Figure out the position of the position in the screen. We add
            // the screen relative coordinate to the widget-relative, plus add
            // the height of a single line to shift it down slightly.
            int lineHeight;
            PointD point =
                displayContext.Caret.Position.ToScreenCoordinates(
                    displayContext, out lineHeight);

            int widgetY = (int) (screenY + lineHeight + point.Y);

            // Create the context menu and show it on the screen right below
            // where the user is currently focused.
            Menu contextMenu = controller.CreateContextMenu();

            if (contextMenu != null)
            {
                contextMenu.ShowAll();
                contextMenu.Popup(
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
                    Global.CurrentEventTime);
            }
        }

        #endregion
    }
}