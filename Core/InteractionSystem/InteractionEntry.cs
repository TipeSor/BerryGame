using Raylib_cs;

namespace BerryGame
{
    internal struct InteractionEntry
    {
        public int Z;
        public Rectangle Rect;
        public IEventHandler Handler;
        public Action Draw;
    }
}
