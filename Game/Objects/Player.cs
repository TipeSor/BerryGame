using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public class Player : GameObject, IGUIHandler, IDrawable
    {
        public Camera2D Camera;
        public Rectangle WorldRect => Shared.WorldRect;
        public Vector2 ScreenSize => Shared.ScreenSize;

        public Texture2D Texture;

        public int Layer => -1;
        public int Depth => 0;

        public GameObject? Following;

        public override void Awake()
        {
            Camera = new Camera2D(
                offset: Vector2.Zero,
                target: WorldRect.Center - (ScreenSize / 2.0f),
                rotation: 0,
                zoom: 1
            );

            LoadTexture();
        }

        public override void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Q))
            {
                if (Manager.TryFindAll(out Minion[] obj))
                {
                    int index = Following is null ? 0 : Array.IndexOf(obj, Following);
                    int next = (index + 1) % obj.Length;
                    Following = obj[next];
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.E))
            {
                Manager.Create<Minion>(MouseContext.Position, Vector2.One * 16);
            }

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

            if (Following is not null && delta != Vector2.Zero)
                Following = null;

            if (Following is null)
                Camera.Target += delta * TimeManager.Delta * speed;
            else
                Camera.Target = Vector2.Lerp(Camera.Target, Following.Position - (ScreenSize / 2.0f), TimeManager.Delta * 3);

            Camera.Target = Vector2.Clamp(
                value1: Camera.Target,
                min: WorldRect.Position,
                max: WorldRect.Position + WorldRect.Size - ScreenSize
            );

            Position = Camera.Target;
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

        public void OnGUI()
        {
            string text =
            $"""
            x: {Position.X}
            y: {Position.Y}
            """;

            GUILayout.Label(text, Color.White);
        }

        public void Draw()
        {
            int start_x = (int)(Camera.Target.X / Texture.Width) * Texture.Width;
            int start_y = (int)(Camera.Target.Y / Texture.Height) * Texture.Height;

            int end_x = (int)(start_x + ScreenSize.X) + Texture.Width;
            int end_y = (int)(start_y + ScreenSize.Y) + Texture.Height;

            Raylib.BeginMode2D(Camera);
            for (int y = start_y; y < end_y; y += Texture.Height)
                for (int x = start_x; x < end_x; x += Texture.Width)
                    Raylib.DrawTextureV(
                        texture: Texture,
                        position: new Vector2(x, y),
                        tint: Color.White);
            Raylib.EndMode2D();
            Raylib.DrawFPS(100, 16);
        }
    }
}
