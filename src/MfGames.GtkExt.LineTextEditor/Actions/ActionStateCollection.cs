using System;

using C5;

using MfGames.GtkExt.LineTextEditor.Interfaces;

namespace MfGames.GtkExt.LineTextEditor.Actions
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
            where TActionState: class, IActionState, new()
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
    }
}