namespace MfGames.GtkExt.LineTextEditor.Buffers
{
    /// <summary>
    /// Represents a range inside a buffer.
    /// </summary>
    public struct BufferSegment
    {
        #region Properties

        public BufferPosition StartPosition { get; set; }

        public BufferPosition EndPosition { get; set; }

        #endregion
    }
}