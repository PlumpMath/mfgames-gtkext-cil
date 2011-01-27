namespace MfGames.GtkExt.LineTextEditor.Buffers
{
    public class BufferSegment
    {
        private BufferPosition startPosition;
        public BufferPosition StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }

        private BufferPosition endPosition;
        public BufferPosition EndPosition
        {
            get { return endPosition; }
            set { endPosition = value; }
        }
    }
}