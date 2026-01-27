global using BerryEngine;
using System.Numerics;
using Raylib_cs;
namespace BerryGame
{
    public static class Shared
    {
        public static Player Player = null!;
        public static Core Core = null!;

        public static Camera2D Camera => Player.Camera;

        public static Vector2 ScreenSize = new(480, 480);
        public static Rectangle WorldRect = new(0, 0, 1024, 1024);

        public static readonly Random RNG = new();
    }

    public class Program
    {
        public static void Main()
        {
            int width = (int)Shared.ScreenSize.X;
            int height = (int)Shared.ScreenSize.Y;

            Manager.Init(width, height, "Berry game");
            Manager.OnCycle += static (_, _) => MouseContext.Update();

            Shared.Player = Manager.Create<Player>();
            Shared.Core = Manager.Create<Core>(Shared.WorldRect.Center, Vector2.One * 64);

            Manager.Create<BushSpawner>();

            Manager.Run();
        }
    }
}
