using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public class Bush : GameObject, IDrawable, IDisposable
    {
        public static long Count = 0;
        public static Texture2D Texture;

        private readonly Queue<Berry> Berries = [];
        public int BerryLimit;

        public bool Hovered;
        private bool _disposed;

        public int Layer => 1;
        public int Depth => Hovered ? 1 : 0;

        public BushState State;
        public float Progress = 0.0f;

        private Vector2 FinalSize = new(64, 32);
        private Vector2 FinalPosition;

        public override void Awake()
        {
            State = BushState.Spawning;
            FinalPosition = Position;

            if (Count++ == 0)
                LoadTexture();
        }

        public void LoadTexture()
        {
            int width = (int)Size.X;
            int height = (int)Size.Y;

            RenderTexture2D rtex = Raylib.LoadRenderTexture(width, height);

            Raylib.BeginTextureMode(rtex);
            Raylib.ClearBackground(Color.White);
            Raylib.EndTextureMode();

            Image img = Raylib.LoadImageFromTexture(rtex.Texture);
            Texture = Raylib.LoadTextureFromImage(img);

            Raylib.UnloadImage(img);
            Raylib.UnloadRenderTexture(rtex);
        }

        public override void Update()
        {
            if (State is BushState.Spawning or BushState.Despawning)
            {
                Progress += TimeManager.Delta;
                if (Progress >= 1.0f)
                {
                    NextState();
                }
            }

            if (State == BushState.Spawning)
            {
                Size = Vector2.Lerp(Vector2.Zero, FinalSize, Progress);
                Position = FinalPosition - (Vector2.UnitY * ((Size.Y - FinalSize.Y) / 2.0f));
            }

            else if (State == BushState.Despawning)
            {
                Size = Vector2.Lerp(FinalSize, Vector2.Zero, Progress);
                Position = FinalPosition - (Vector2.UnitY * ((Size.Y - FinalSize.Y) / 2.0f));
            }

            else if (State == BushState.Idle)
            {
                if (Berries.Count == 0)
                {
                    NextState();
                    return;
                }

                Hovered = MouseContext.CheckOverlap(this) && !MouseContext.IsUsed;
                if (Hovered)
                {
                    if (MouseContext.IsButtonDown(MouseButton.Left) &&
                        TryGetBerry(out Berry? b))
                    {
                        b.Pick();
                        MouseContext.Use();
                    }
                }
            }
        }

        public void NextState()
        {
            Progress = 0.0f;
            switch (State)
            {
                case BushState.Spawning:
                    State = BushState.Idle;
                    SpawnBerries();
                    break;
                case BushState.Idle:
                    State = BushState.Despawning;
                    break;
                case BushState.Despawning:
                    Manager.Destroy(this);
                    break;
                default:
                    break;
            }
        }

        public void SpawnBerries()
        {
            while (Berries.Count < BerryLimit)
            {
                float dx = Shared.RNG.NextSingle() * Size.X;
                float dy = Shared.RNG.NextSingle() * Size.Y;

                float saturation = (Shared.RNG.NextSingle() * 0.3f) + 0.7f;
                float value = (Shared.RNG.NextSingle() * 0.7f) + 0.3f;

                Color color = Color.FromHSV(0f, saturation, value);

                Berry berry = Manager.Create<Berry>(Rect.Position + new Vector2(dx, dy), Vector2.One * 8);
                berry.Color = color;

                Berries.Enqueue(berry);
            }
        }

        public bool TryGetBerry([NotNullWhen(true)] out Berry? berry)
            => Berries.TryDequeue(out berry);

        public void Draw()
        {
            Raylib.BeginMode2D(Shared.Camera);
            Raylib.DrawTexturePro(
                texture: Texture,
                source: new Rectangle(Vector2.Zero, Texture.Dimensions),
                dest: Rect,
                origin: Vector2.Zero,
                rotation: 0f,
                tint: Hovered ? Color.Green : Color.Lime);
            Raylib.DrawRectangleLinesEx(Rect, 2, Color.SkyBlue);
            Raylib.EndMode2D();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (Berry berry in Berries)
                Manager.Destroy(berry);

            Berries.Clear();

            if (Texture.Id != 0 && --Count == 0)
            {
                Raylib.UnloadTexture(Texture);
                Texture = default;
            }
        }
    }

    public enum BushState
    {
        Spawning,
        Idle,
        Despawning
    }
}
