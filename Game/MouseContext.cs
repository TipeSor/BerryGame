using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public static class MouseContext
    {
        public static bool IsUsed = false;

        public static Vector2 Position => Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Shared.Camera);
        public static Vector2 Delta => Raylib.GetMouseDelta();

        public static bool CheckOverlap(GameObject obj)
        {
            return Raylib.CheckCollisionPointRec(Position, obj.Rect);
        }

        public static bool IsButtonDown(MouseButton button)
        {
            if (IsUsed) return false;

            return Raylib.IsMouseButtonPressed(button);
        }

        public static void Update()
        {
            IsUsed = false;
        }

        public static void Use()
        {
            IsUsed = true;
        }
    }
}
