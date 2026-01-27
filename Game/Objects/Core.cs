using Raylib_cs;

namespace BerryGame
{
    public class Core : GameObject, IDrawable, IGUIHandler, IDisposable
    {
        public Texture2D Texture;

        public int Layer => 0;
        public int Depth => 0;

        public long BerryCounter = 0;

        public override void Awake()
        {
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

        public void Collect(Berry berry)
        {
            BerryCounter++;
            Manager.Destroy(berry);
        }

        public void Draw()
        {
            Raylib.BeginMode2D(Shared.Camera);
            Raylib.DrawTextureV(Texture, Rect.Position, Color.White);
            Raylib.EndMode2D();
        }

        public void OnGUI()
        {
            GUILayout.Label($"Berries: {BerryCounter}", Color.White);
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
