using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public static class GUI
    {
        private static int Layer = 0;
        private static int Zindex = 0;
        private static IEventHandler? currentHandler = null;

        public static void SetLayer(int layer)
        {
            Layer = layer;
        }

        public static void SetZindex(int zindex)
        {
            Zindex = zindex;
        }

        public static void Begin(IEventHandler hander)
        {
            Layer = 0;
            Zindex = 0;
            currentHandler = hander;
        }

        public static void End()
        {
            currentHandler = null;
        }

        public static void DrawTexture(
            Texture2D texture,
            Rectangle rect,
            Color tint)
        {
            if (Event.current.Type != EventType.Repaint)
                return;

            if (currentHandler == null)
                return;

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = currentHandler,
                Draw = () => Raylib.DrawTextureV(texture, rect.Position, tint)
            });
        }

        public static void Label(
            string text,
            Vector2 position,
            Color tint)
        {
            if (Event.current.Type != EventType.Repaint)
                return;

            if (currentHandler == null)
                return;

            const int fontSize = 16;
            const float spacing = 1f;

            Vector2 size = Raylib.MeasureTextEx(
                Raylib.GetFontDefault(),
                text,
                fontSize,
                spacing
            );

            Rectangle rect = new Rectangle(position, size);

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = currentHandler,
                Draw = () => Raylib.DrawText(text, (int)position.X, (int)position.Y, 16, tint)
            });
        }

        public static void DrawOutline(
            Rectangle rect,
            float thickness,
            Color tint)
        {
            if (Event.current.Type != EventType.Repaint)
                return;

            if (currentHandler == null)
                return;

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = currentHandler,
                Draw = () => Raylib.DrawRectangleLinesEx(rect, thickness, tint)
            });
        }
    }
}
