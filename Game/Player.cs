using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public record Player : IEventHandler
    {
        public Camera2D camera;
        public Rectangle WorldRect;
        public Vector2 ScreenSize;

        private Vector2 MousePosition;

        public Vector2 Position => camera.Target - (camera.Offset / 2.0f);

        public Player(Rectangle worldRect, Vector2 screenSize)
        {
            WorldRect = worldRect;
            ScreenSize = screenSize;

            camera =
                new Camera2D(
                    offset: Vector2.Zero,
                    target: WorldRect.Center - (ScreenSize / 2.0f),
                    rotation: 0,
                    zoom: 1
                );
        }

        public void OnEvent()
        {
            string text =
            $"""
            x: {Position.X}
            y: {Position.Y}
            """;

            if (Event.current.IsMouse)
                MousePosition = Event.current.MousePosition;

            GUI.Label(text, Position + camera.Offset / 2.0f, Color.White);
            GUI.DrawDot(MousePosition, 8, Color.DarkBlue);
        }

        public void Update()
        {
            Vector2 delta = Vector2.Zero;
            float speed = 200;

            if (Raylib.IsKeyDown(KeyboardKey.W))
                delta -= Vector2.UnitY;
            if (Raylib.IsKeyDown(KeyboardKey.S))
                delta += Vector2.UnitY;
            if (Raylib.IsKeyDown(KeyboardKey.A))
                delta -= Vector2.UnitX;
            if (Raylib.IsKeyDown(KeyboardKey.D))
                delta += Vector2.UnitX;

            camera.Target += delta * TimeManager.Delta * speed;
            MousePosition += delta * TimeManager.Delta * speed;

            camera.Target = Vector2.Clamp(
                value1: camera.Target,
                min: WorldRect.Position,
                max: WorldRect.Position + WorldRect.Size - ScreenSize
            );
        }
    }
}
