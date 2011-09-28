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

using MfGames.GtkExt.TextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.TextEditor.Editing.Actions
{
    /// <summary>
    /// Used to contain the state between vertical movement.
    /// </summary>
    public class VerticalMovementActionState : IActionState
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalMovementActionState"/> class.
        /// </summary>
        /// <param name="layoutLineX">The X coordinate in Pango units.</param>
        public VerticalMovementActionState(int layoutLineX)
        {
            LayoutLineX = layoutLineX;
        }

        #endregion

        #region Action State

        /// <summary>
        /// Determines whether this action state can be removed. This is also
        /// an opportunity for the action to clean up before removed.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can remove; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRemove()
        {
            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the line X for vertical movements.
        /// </summary>
        /// <value>
        /// The line X-coordinate.
        /// </value>
        public int LayoutLineX { get; set; }

        #endregion
    }
}