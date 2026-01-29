using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public class Berry : GameObject, IDrawable, IDisposable
    {
        public static long count = 0;
        public static Texture2D Texture;

        private Vector2 Begin;
        private Vector2 Target => Shared.Core.Position;

        public int Layer => 2;
        public int Depth => 0;

        private float PathProgress;

        public Color Color;

        public BerryState State = BerryState.Idle;

        private bool _disposed;

        public override void Awake()
        {
            Begin = Position;
            PathProgress = 0.0f;

            if (count++ == 0)
                LoadTexture();
        }

        public void LoadTexture()
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

            Raylib.SetTextureFilter(Texture, TextureFilter.Point);
        }

        public override void Update()
        {
            switch (State)
            {
                // Berry is idle
                case BerryState.Idle:
                    break;

                // When player collects the berry
                // The berry will move to the core itself
                case BerryState.Picked:
                    HandlePicked();
                    break;

                // When its being carried, the carrier is resposable for updating the berries prosition
                case BerryState.Carried:
                    break;

                // The berry is collected
                case BerryState.Collected:
                    break;

                default:
                    break;
            }
        }

        public void HandlePicked()
        {
            float distance = Vector2.Distance(Begin, Target);
            PathProgress += TimeManager.Delta / distance * 300;
            if (PathProgress >= 1)
            {
                Collect();
                return;
            }

            Position = Vector2.Lerp(Begin, Target, PathProgress);
        }

        public void Draw()
        {
            Raylib.BeginMode2D(Shared.Camera);
            Raylib.DrawTextureV(Texture, Rect.Position, Color);
            Raylib.EndMode2D();
        }

        public void Pick()
        {
            State = BerryState.Picked;
        }

        public void Carry()
        {
            State = BerryState.Carried;
        }

        public void Collect()
        {
            State = BerryState.Collected;
            Shared.Core.Collect(this);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Manager.Destroy(this);
            if (Texture.Id != 0 && --count == 0)
            {
                Raylib.UnloadTexture(Texture);
                Texture = default;
            }
        }
    }

    public enum BerryState
    {
        Idle,
        Picked,
        Carried,
        Collected
    }
}
