using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public class Program
    {
        public static List<IEventHandler> handlers = [];
        public static Stack<IEventHandler> handlersToRemove = [];
        public static Stack<IEventHandler> handlersToAdd = [];

        public static Player Player = null!;

        public static void Main()
        {
            Rectangle WorldRect =
                new Rectangle(
                    x: 0,
                    y: 0,
                    width: 1024,
                    height: 1024
                );

            Vector2 ScreenSize = new Vector2(480, 480);

            Raylib.InitWindow((int)ScreenSize.X, (int)ScreenSize.Y, "Berry Game");
            Raylib.InitAudioDevice();

            Player = new Player(WorldRect, ScreenSize);
            World world = new World(Player, ScreenSize);
            Core core = new Core(WorldRect.Center);
            Bush bush = new Bush(core, Vector2.One * 256);
            Rock rock = new Rock(Vector2.One * 640);

            handlers = [world, Player, core, bush, rock];

            while (!Raylib.WindowShouldClose())
            {
                while (handlersToAdd.TryPop(out IEventHandler? handler))
                {
                    handlers.Add(handler);
                }

                while (handlersToRemove.TryPop(out IEventHandler? handler))
                {
                    handlers.Remove(handler);
                }

                // time
                {
                    TimeManager.Update();
                    {
                        foreach (IEventHandler hander in handlers)
                        {
                            if (hander is Player p) p.Update();
                            if (hander is Bush b) b.Update();
                            if (hander is Fruit f) f.Update();
                            if (hander is Rock r) r.Update();
                        }
                    }
                    if (TimeManager.ShouldRunFixedUpdate())
                    {
                        TimeManager.FixedUpdate();
                    }
                }

                // event
                {
                    // layout
                    {
                        Event.current = new Event
                        {
                            Type = EventType.Layout
                        };
                        CallEvent();
                    }

                    // repaint
                    {
                        Event.current = new Event
                        {
                            Type = EventType.Repaint
                        };
                        CallEvent();
                    }

                    // TODO: keyboard

                    // mouse 
                    {
                        Vector2 delta = Raylib.GetMouseDelta();
                        Vector2 position = Raylib.GetMousePosition() + Player.Position - Player.camera.Offset / 2.0f;
                        if (delta != Vector2.Zero)
                        {
                            Event.current = new Event
                            {
                                Type = Raylib.IsMouseButtonDown(MouseButton.Left)
                                    ? EventType.MouseDrag
                                    : EventType.MouseMove,
                                MousePosition = position,
                                Delta = delta
                            };

                            CallMouseEvent();
                        }
                        for (int i = 0; i <= 6; i++)
                        {
                            MouseButton button = (MouseButton)i;

                            if (Raylib.IsMouseButtonPressed(button))
                            {
                                Event.current = new Event
                                {
                                    Type = EventType.MouseDown,
                                    MousePosition = position,
                                    Button = button
                                };
                                CallMouseEvent();
                            }

                            if (Raylib.IsMouseButtonReleased(button))
                            {
                                Event.current = new Event
                                {
                                    Type = EventType.MouseUp,
                                    MousePosition = position,
                                    Button = button
                                };
                                CallMouseEvent();
                            }
                        }
                    }

                    Raylib.BeginDrawing();
                    Raylib.BeginMode2D(Player.camera);
                    InteractionQueue.Flush();
                    Raylib.EndMode2D();
                    Raylib.EndDrawing();

                    void CallEvent()
                    {
                        foreach (IEventHandler hander in handlers)
                        {
                            if (Event.current.Type == EventType.Repaint)
                            {
                                GUI.Begin(hander);
                            }
                            hander.OnEvent();
                        }
                    }

                    static void CallMouseEvent()
                    {
                        if (Event.mouseOwner != null)
                        {
                            if (Event.current.Type == EventType.MouseUp)
                            {
                                Event.mouseOwner.OnEvent();
                                if (Event.current.Type == EventType.Used)
                                {
                                    Event.mouseOwner = null;
                                    return;
                                }
                            }
                            else if (Event.current.Type == EventType.MouseDrag)
                            {
                                Event.mouseOwner.OnEvent();
                                if (Event.current.Type == EventType.Used)
                                {
                                    return;
                                }
                                Event.mouseOwner = null;
                            }
                        }

                        foreach (InteractionEntry e in InteractionQueue.Entries.OrderByDescending(static e => e.Z))
                        {
                            EventType lastType = Event.current.Type;

                            e.Handler.OnEvent();
                            if (Event.current.Type == EventType.Used)
                            {
                                if (lastType is EventType.MouseDown)
                                {
                                    Event.mouseOwner = e.Handler;
                                }
                                break;
                            }
                        }
                    }
                }
            }


            foreach (IEventHandler hander in handlers)
            {
                if (hander is IDisposable d)
                    d.Dispose();
            }

            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}
