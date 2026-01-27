using Raylib_cs;

namespace BerryEngine
{
    internal struct InteractionEntry
    {
        public int Z;
        public Rectangle Rect;
        public GameObject Handler;
        public Action Draw;
    }
}
