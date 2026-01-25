using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    internal record Bush : IEventHandler, IDisposable
    {
        public static long Count = 0;
        public static Texture2D Texture;

        public Rectangle Rect;
        private readonly Core MainCore;

        public Vector2 Position => Rect.Position;
        public Vector2 Center => Rect.Center;
        public Vector2 Size => Rect.Size;

        private readonly Queue<Fruit> Fruits;
        private readonly Color[] colors;
        public bool IsActive;

        public bool Hovered;

        private bool _disposed;

        private static Random rng = new Random();

        public Bush(Core core, Vector2 center)
        {
            Vector2 size = new Vector2(64, 32);

            Rect = new Rectangle(
                center - (size / 2.0f),
                size
            );

            MainCore = core;

            IsActive = true;

            Fruits = [];

            Hovered = false;

            if (Count++ == 0)
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

            if (Fruits.Count < 5)
            {
                float dx = rng.NextSingle() * Size.X;
                float dy = rng.NextSingle() * Size.Y;
                int temp = rng.Next(0, 40);
                Color color = new Color(rng.Next(200, 255), temp, temp);
                Fruit fruit = new(MainCore, Position + new Vector2(dx, dy), color);
                Fruits.Enqueue(fruit);
                Program.handlersToAdd.Push(fruit);
            }
        }

        public void OnEvent()
        {
            Event evt = Event.current;

            if (IsActive)
            {
                GUI.DrawTexture(Texture, Rect, Hovered ? Color.Green : Color.Lime);
            }

            if (!evt.IsMouse)
                return;

            Vector2 pos = evt.MousePosition;
            Hovered = Raylib.CheckCollisionPointRec(pos, Rect);

            if (evt.Type != EventType.MouseDown)
                return;

            if (evt.Button == MouseButton.Left && Hovered)
            {
                if (Fruits.TryDequeue(out Fruit? fruit))
                    fruit.Picked = true;
                evt.Use();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (Fruit fruit in Fruits)
                fruit.Dispose();

            Fruits.Clear();

            if (Texture.Id != 0 && --Count == 0)
            {
                Raylib.UnloadTexture(Texture);
                Texture = default;
            }
        }
    }
}
