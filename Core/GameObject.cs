using System.Numerics;
using Raylib_cs;

namespace BerryEngine
{
    public abstract class GameObject
    {
        public Rectangle Rect;

        public Vector2 Position
        {
            get => Rect.Center;
            set => Rect.Position = value - (Rect.Size / 2.0f);
        }

        public Vector2 Size
        {
            get => Rect.Size;
            set => Rect.Size = value;
        }

        public bool DidAwake { get; private set; }
        public bool DidStart { get; private set; }

        public bool IsActive { get; private set; }
        internal bool Destroyed;


        internal GameObject() { }

        internal void BaseAwake() { if (DidAwake) return; DidAwake = true; Awake(); }
        internal void BaseStart() { if (DidStart) return; DidStart = true; Start(); }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }

        public void SetActive(bool value)
            => IsActive = value;
    }
}
