using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Raylib_cs;

namespace BerryEngine
{
    public static class Manager
    {
        internal static List<GameObject> objects = [];

        internal static Queue<GameObject> objectsToAdd = [];
        internal static Queue<GameObject> objectsToRemove = [];

        internal static RenderTexture2D RenderTexture;
        public static event EventHandler? OnCycle = null;

        public static T Create<T>()
            where T : GameObject, new()
            => Create<T>(new Rectangle(0, 0, 0, 0));

        public static T Create<T>(float x, float y, float width, float height)
            where T : GameObject, new()
            => Create<T>(new Rectangle(x, y, width, height));

        public static T Create<T>(Vector2 position, Vector2 size)
            where T : GameObject, new()
            => Create<T>(new Rectangle(position - (size / 2.0f), size));

        public static T Create<T>(Rectangle rect)
            where T : GameObject, new()
        {
            T obj = new() { Rect = rect };
            obj.BaseAwake();
            objectsToAdd.Enqueue(obj);
            return obj;
        }

        public static bool TryFind<T>([NotNullWhen(true)] out GameObject? obj)
        {
            for (int i = 0; i < objects.Count; i++)
                if (objects[i] is T) { obj = objects[i]; return true; }

            obj = null;
            return false;
        }

        public static bool TryFindAll<T>(out GameObject[] obj)
        {
            obj = [.. objects.Where(static o => o is T)];
            return obj.Length > 0;
        }

        internal static void Destroy(GameObject obj)
        {
            obj.Destroyed = true;
            obj.SetActive(false);
            objectsToRemove.Enqueue(obj);
        }

        public static void Init(int width, int height, string title)
        {
            Raylib.InitWindow(width, height, title);
            Raylib.InitAudioDevice();
            RenderTexture = Raylib.LoadRenderTexture(width, height);
        }

        private static void CleanUp()
        {
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        public static void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                OnCycle?.Invoke(null, EventArgs.Empty);
                AddObjects();
                UpdateObjects();
                Raylib.BeginDrawing();
                DrawObjects();
                HandleEvents();
                Raylib.EndDrawing();
                RemoveObjects();
            }
            CleanUp();
        }

        private static void AddObjects()
        {
            while (objectsToAdd.TryDequeue(out GameObject? obj))
            {
                objects.Add(obj);
                obj.SetActive(true);
                obj.BaseStart();
            }
        }

        private static void RemoveObjects()
        {
            while (objectsToRemove.TryDequeue(out GameObject? obj))
            {
                objects.Remove(obj);
                if (obj is IDisposable d) d.Dispose();
            }

            if (Event.MouseOwner?.Destroyed ?? false)
                Event.MouseOwner = null;
        }

        private static void UpdateObjects()
        {
            TimeManager.Update();
            bool runFixed = TimeManager.ShouldRunFixedUpdate();
            if (runFixed) TimeManager.FixedUpdate();

            foreach (GameObject obj in objects)
            {
                if (!obj.IsActive) continue;
                obj.Update();
                if (runFixed) obj.FixedUpdate();
            }
        }

        private static void DrawObjects()
        {
            IOrderedEnumerable<IDrawable> drawables
                = objects
                    .Where(static o => o.IsActive)
                    .OfType<IDrawable>()
                    .OrderBy(static d => d.Layer)
                    .ThenBy(static d => d.Depth);

            foreach (IDrawable drawable in drawables)
            {
                drawable.Draw();
            }
        }

        private static void HandleEvents()
        {
            HandleLayoutEvent();
            HandleRepaintEvent();
            HandleMouseEvents();
            Raylib.BeginTextureMode(RenderTexture);
            Raylib.ClearBackground(Color.Blank);
            InteractionQueue.Flush();
            Raylib.EndTextureMode();

            float width = RenderTexture.Texture.Width;
            float height = RenderTexture.Texture.Height;
            Raylib.DrawTextureRec(
                texture: RenderTexture.Texture,
                source: new Rectangle(
                    x: 0,
                    y: height,
                    width: width,
                    height: -height),
                position: Vector2.Zero,
                tint: Color.White);
        }

        private static void HandleMouseEvents()
        {
            foreach (Event evt in PoolMouseEvents())
            {
                Event.Current = evt;

                IEnumerable<IGUIHandler> handlers =
                    InteractionQueue.Entries
                                    .OrderByDescending(static e => e.Z)
                                    .Select(static e => e.Handler)
                                    .OfType<IGUIHandler>();

                foreach (IGUIHandler handler in handlers)
                {
                    EventType lastType = Event.Current.Type;

                    handler.OnGUI();

                    if (Event.Current.Type == EventType.Used)
                    {
                        if (lastType == EventType.KeyDown)
                            Event.MouseOwner = (GameObject)handler;
                        break;
                    }
                }
            }
        }

        private static void HandleLayoutEvent()
        {
            GUILayout.Reset();
            Event.Current = new Event { Type = EventType.Layout };
            foreach (GameObject obj in objects)
            {
                if (obj is not IGUIHandler handler)
                    continue;

                GUILayout.Begin(obj);
                handler.OnGUI();
                GUILayout.End();
            }
        }

        private static void HandleRepaintEvent()
        {
            Event.Current = new Event { Type = EventType.Repaint };
            foreach (GameObject obj in objects)
            {
                if (obj is not IGUIHandler handler)
                    continue;

                GUILayout.Begin(obj);
                GUI.Begin(obj);
                handler.OnGUI();
                GUI.End();
                GUILayout.End();
            }
        }

        private static IEnumerable<Event> PoolMouseEvents()
        {
            {
                Vector2 delta = Raylib.GetMouseDelta();
                Vector2 position = Raylib.GetMousePosition();
                if (delta != Vector2.Zero)
                {
                    yield return new Event
                    {
                        Type = Raylib.IsMouseButtonDown(MouseButton.Left)
                            ? EventType.MouseDrag
                            : EventType.MouseMove,
                        MousePosition = position,
                        Delta = delta
                    };
                }
                for (int i = 0; i <= 6; i++)
                {
                    MouseButton button = (MouseButton)i;

                    if (Raylib.IsMouseButtonPressed(button))
                    {
                        yield return new Event
                        {
                            Type = EventType.MouseDown,
                            MousePosition = position,
                            Button = button
                        };
                    }

                    if (Raylib.IsMouseButtonReleased(button))
                    {
                        yield return new Event
                        {
                            Type = EventType.MouseUp,
                            MousePosition = position,
                            Button = button
                        };
                    }
                }
            }
        }
    }
}
