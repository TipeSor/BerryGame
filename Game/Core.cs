using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    internal record Core : IEventHandler, IDisposable
    {
        public Rectangle Rect;

        public Vector2 Position => Rect.Position;
        public Vector2 Center => Rect.Center;
        public Vector2 Size => Rect.Size;

        public Texture2D Texture;

        public Core(Vector2 center)
        {
            Vector2 size = Vector2.One * 64;
            Rect = new Rectangle(center - (size / 2.0f), size);
            LoadTexture();
        }

        public void LoadTexture()
        {
            int width = (int)Size.X;
            int height = (int)Size.Y;

            RenderTexture2D rtex = Raylib.LoadRenderTexture(width, height);

            Raylib.BeginTextureMode(rtex);
            {
                Raylib.ClearBackground(Color.Blue);
            }
            Raylib.EndTextureMode();

            Image img = Raylib.LoadImageFromTexture(rtex.Texture);
            Texture = Raylib.LoadTextureFromImage(img);

            Raylib.UnloadImage(img);
            Raylib.UnloadRenderTexture(rtex);
        }

        public void Collect(Fruit fruit)
        {
            fruit.IsActive = false;
            fruit.Dispose();
        }

        public void OnEvent()
        {
            GUI.DrawTexture(Texture, Rect, Color.White);
        }

        public void Dispose()
        {
            if (Texture.Id != 0)
            {
                Raylib.UnloadTexture(Texture);
                Texture = default;
            }
        }
    }
}
