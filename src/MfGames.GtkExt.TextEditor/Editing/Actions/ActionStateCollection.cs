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

using System;
using System.Collections.Generic;

using C5;

using MfGames.GtkExt.TextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.TextEditor.Editing.Actions
{
    /// <summary>
    /// Contains a collection of action states that can be conditionally
    /// removed with various states removed.
    /// </summary>
    public class ActionStateCollection : ArrayList<IActionState>
    {
        /// <summary>
        /// Gets an action state inside the collection or null.
        /// </summary>
        /// <typeparam name="TActionState">The type of the action state.</typeparam>
        /// <returns></returns>
        public TActionState Get<TActionState>()
            where TActionState: class, IActionState
        {
            foreach (IActionState state in this)
            {
                if (state is TActionState)
                {
                    return (TActionState) state;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes all the states, allowing each one to prevent its removal.
        /// </summary>
        public void RemoveAll()
        {
            RemoveAllExcluding();
        }

        /// <summary>
        /// Removes all states excluding the ones given.
        /// </summary>
        /// <param name="types">The types.</param>
        public void RemoveAllExcluding(params Type[] types)
        {
            RemoveAllExcluding((IEnumerable<Type>) types);
        }

        /// <summary>
        /// Removes all the elements inside the array except for those in the
        /// list.
        /// </summary>
        /// <param name="excludeTypes">The types not to remove.</param>
        public void RemoveAllExcluding(IEnumerable<Type> excludeTypes)
        {
            // Go through a list and build up a list of things to remove.
            var removeStates = new ArrayList<IActionState>();

            foreach (IActionState state in this)
            {
                // First check to see if this state is on the exclude list.
                bool found = false;

                foreach (Type type in excludeTypes)
                {
                    if (type.IsAssignableFrom(state.GetType()))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                // Check to see if this action is willing to let itself be
                // removed.
                if (state.CanRemove())
                {
                    removeStates.Add(state);
                }
            }

            // Go through the remove list and remove them.
            foreach (IActionState state in removeStates)
            {
                Remove(state);
            }
        }
    }
}