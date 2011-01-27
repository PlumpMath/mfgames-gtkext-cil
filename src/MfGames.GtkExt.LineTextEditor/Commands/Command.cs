using C5;

using MfGames.GtkExt.LineTextEditor.Interfaces;

namespace MfGames.GtkExt.LineTextEditor.Commands
{
    /// <summary>
    /// Implements a command, which is a collection of operations both
    /// to perform the command and undo it.
    /// </summary>
    public class Command
    {
        #region Constuctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public Command()
        {
            Operations = new ArrayList<ILineBufferOperation>();
            UndoOperations = new ArrayList<ILineBufferOperation>();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Gets the operations for the command.
        /// </summary>
        public ArrayList<ILineBufferOperation> Operations { get; private set; }

        /// <summary>
        /// Gets the undo operations for this command.
        /// </summary>
        public ArrayList<ILineBufferOperation> UndoOperations { get; private set; }

        #endregion
    }
}