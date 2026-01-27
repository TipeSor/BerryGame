using System.Numerics;
using Raylib_cs;

namespace BerryEngine
{
    public static class GUI
    {
        private static int Layer = 0;
        private static int Zindex = 0;
        private static GameObject? current = null;

        public static void SetLayer(int layer)
        {
            Layer = layer;
        }

        public static void SetZindex(int zindex)
        {
            Zindex = zindex;
        }

        public static void Begin(GameObject obj)
        {
            Layer = 0;
            Zindex = 0;
            current = obj;
        }

        public static void End()
        {
            current = null;
        }

        public static void DrawTexture(
            Texture2D texture,
            Rectangle rect,
            Color tint)
        {
            if (Event.Current.Type != EventType.Repaint)
                return;

            if (current == null)
                return;

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = current,
                Draw = () => Raylib.DrawTextureV(texture, rect.Position, tint)
            });
        }

        public static void Label(
            string text,
            Vector2 position,
            Color tint)
        {
            if (Event.Current.Type != EventType.Repaint)
                return;

            if (current == null)
                return;

            const int fontSize = 16;
            const float spacing = 1f;

            Vector2 size = Raylib.MeasureTextEx(
                Raylib.GetFontDefault(),
                text,
                fontSize,
                spacing
            );

            Rectangle rect = new(position, size);

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = current,
                Draw = () => Raylib.DrawText(text, (int)position.X, (int)position.Y, 16, tint)
            });
        }

        public static void DrawOutline(
            Rectangle rect,
            float thickness,
            Color tint)
        {
            if (Event.Current.Type != EventType.Repaint)
                return;

            if (current == null)
                return;

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = current,
                Draw = () => Raylib.DrawRectangleLinesEx(rect, thickness, tint)
            });
        }

        public static void DrawDot(
            Vector2 position,
            float thickness,
            Color tint)
        {
            if (Event.Current.Type != EventType.Repaint)
                return;

            if (current == null)
                return;

            Rectangle rect = new()
            {
                X = position.X - (thickness / 2.0f),
                Y = position.Y - (thickness / 2.0f),
                Width = thickness,
                Height = thickness
            };

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = (Layer * 1000) + Zindex++,
                Rect = rect,
                Handler = current,
                Draw = () => Raylib.DrawCircleV(position, thickness, tint)
            });
        }
    }
}
