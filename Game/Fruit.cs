using System.Numerics;
using System.Security.Cryptography;
using Raylib_cs;

namespace BerryGame
{
    internal record Fruit : IEventHandler, IDisposable
    {
        public static long count = 0;
        public static Texture2D Texture;

        public Rectangle Rect;

        private Vector2 Start;
        private Vector2 Target => MainCore.Center;
        private float PathProgress;
        private readonly Core MainCore;

        public Vector2 Position => Rect.Position;
        public Vector2 Center => Rect.Center;
        public Vector2 Size => Rect.Size;

        public bool Picked;
        public bool IsActive;

        private Color Color;

        private bool _disposed;

        public Fruit(Core core, Vector2 center, Color color)
        {
            Vector2 size = Vector2.One * 16;

            Rect = new Rectangle(
                center - (size / 2.0f),
                size
            );

            Start = Rect.Center;
            PathProgress = 0.0f;
            MainCore = core;

            Picked = false;
            IsActive = true;

            Color = color;

            if (count++ == 0)
                LoadTexture();
        }

        public void LoadTexture()
        {
            {
                int width = (int)Size.X;
                int height = (int)Size.Y;

                RenderTexture2D rtex = Raylib.LoadRenderTexture(width, height);

                Raylib.BeginTextureMode(rtex);
                {
                    Raylib.ClearBackground(Color.White);
                }
                Raylib.EndTextureMode();

                Image img = Raylib.LoadImageFromTexture(rtex.Texture);
                Texture = Raylib.LoadTextureFromImage(img);

                Raylib.UnloadImage(img);
                Raylib.UnloadRenderTexture(rtex);
            }
        }

        public void Update()
        {
            if (!IsActive)
                return;

            if (!Picked)
                return;

            PathProgress += TimeManager.Delta;
            if (PathProgress >= 1)
            {
                MainCore.Collect(this);
                return;
            }

            Rect.Position = Vector2.Lerp(Start, Target, PathProgress) - (Rect.Size / 2.0f);
        }

        public void OnEvent()
        {
            if (IsActive)
            {
                GUI.SetLayer(1);
                GUI.DrawTexture(Texture, Rect, Color);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Program.handlersToRemove.Push(this);
            if (Texture.Id != 0 && --count == 0)
            {
                Raylib.UnloadTexture(Texture);
                Texture = default;
            }
        }
    }
}
