using System.Numerics;
using Raylib_cs;

namespace BerryEngine
{
    public static class GUILayout
    {
        private static GameObject current = null!;
        private static Vector2 Cursor;

        public static void Reset()
        {
            Cursor = Vector2.Zero;
        }

        public static void Begin(GameObject obj)
        {
            current = obj;
        }

        public static void End()
        {
            current = null!;
        }

        public static void Label(string text, Color tint)
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

            Rectangle rect = new(Cursor, size);

            InteractionQueue.Submit(new InteractionEntry
            {
                Z = 0,
                Rect = rect,
                Handler = current,
                Draw = () => Raylib.DrawText(text, (int)rect.X, (int)rect.Y, 16, tint)
            });

            Cursor += Vector2.UnitY * rect.Size.Y;
        }
    }
}
