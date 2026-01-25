using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    internal record World : IEventHandler, IDisposable
    {
        public Texture2D Texture;

        public Player Player;
        public Vector2 ScreenSize;

        public World(Player player, Vector2 screenSize)
        {
            Player = player;
            ScreenSize = screenSize;
            LoadTexture();
        }

        public void LoadTexture()
        {
            int size = 64;
            int hsize = size / 2;
            int lsize = size - hsize;

            RenderTexture2D rtex = Raylib.LoadRenderTexture(size, size);

            Raylib.BeginTextureMode(rtex);
            {
                Raylib.ClearBackground(Color.Pink);
                Raylib.DrawRectangle(0, 0, hsize, hsize, Color.DarkPurple);
                Raylib.DrawRectangle(hsize, hsize, lsize, lsize, Color.DarkPurple);
            }
            Raylib.EndTextureMode();

            Image img = Raylib.LoadImageFromTexture(rtex.Texture);
            Texture = Raylib.LoadTextureFromImage(img);

            Raylib.UnloadImage(img);
            Raylib.UnloadRenderTexture(rtex);
        }

        public void OnEvent()
        {
            int start_x = (int)(Player.camera.Target.X / Texture.Width) * Texture.Width;
            int start_y = (int)(Player.camera.Target.Y / Texture.Height) * Texture.Height;

            int end_x = (int)(start_x + ScreenSize.X) + Texture.Width;
            int end_y = (int)(start_y + ScreenSize.Y) + Texture.Height;

            GUI.SetLayer(-1);
            for (int y = start_y; y < end_y; y += Texture.Height)
                for (int x = start_x; x < end_x; x += Texture.Width)
                    GUI.DrawTexture(
                        texture: Texture,
                        rect: new Rectangle(
                            x: x,
                            y: y,
                            width: Texture.Width,
                            height: Texture.Height),
                        tint: Color.White);
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
