using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    internal record Rock : IEventHandler, IDisposable
    {
        public static long Count = 0;
        public static Texture2D Texture;

        public Rectangle Rect;

        public Vector2 Position => Rect.Position;
        public Vector2 Center => Rect.Center;
        public Vector2 Size => Rect.Size;

        private Vector2 Target;

        public bool IsActive;
        public bool IsHeld;
        private bool _disposed;

        private static Random rng = new Random();

        public Rock(Vector2 center)
        {
            Vector2 size = new Vector2(32, 32);

            Rect = new Rectangle(
                center - (size / 2.0f),
                size
            );

            IsActive = true;
            IsHeld = false;

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
                    Raylib.ClearBackground(Color.Gray);
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
            if (IsHeld)
                Rect.Position = Vector2.Lerp(Center, Target, TimeManager.Delta * 5f) - Size / 2.0f;
        }

        public void OnEvent()
        {
            Event evt = Event.current;

            if (!IsActive)
                return;

            
            GUI.DrawTexture(Texture, Rect, Color.White);
            GUI.DrawOutline(Rect, 2, Color.SkyBlue);
            if (IsHeld)
                GUI.DrawDot(Target, 2, Color.Beige);

            if (!evt.IsMouse)
                return;

            Vector2 pos = evt.MousePosition;
            bool hovered = Raylib.CheckCollisionPointRec(pos, Rect);

            if (evt.Type == EventType.MouseUp && hovered)
            {
                IsHeld = false;
                evt.Use();
            }

            if (evt.Type == EventType.MouseDrag && IsHeld)
            {
                Target = evt.MousePosition;
                evt.Use();
                return;
            }

            if (evt.Type == EventType.MouseDown && hovered)
            {
                IsHeld = true;
                Target = evt.MousePosition;
                evt.Use();
                return;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (Texture.Id != 0 && --Count == 0)
            {
                Raylib.UnloadTexture(Texture);
                Texture = default;
            }
        }
    }
}
