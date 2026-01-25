using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public record Player : IEventHandler
    {
        public Camera2D camera;
        public Rectangle WorldRect;
        public Vector2 ScreenSize;

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
            x: {camera.Target.X - (camera.Offset.X / 2.0f)}
            y: {camera.Target.Y - (camera.Offset.Y / 2.0f)}
            """;

            GUI.Label(text, Position, Color.White);
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

            camera.Target = Vector2.Clamp(
                value1: camera.Target,
                min: WorldRect.Position,
                max: WorldRect.Position + WorldRect.Size - ScreenSize
            );
        }
    }
}
