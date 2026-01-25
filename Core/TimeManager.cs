using Raylib_cs;

namespace BerryGame
{
    public static class TimeManager
    {
        public static double Time { get; private set; }
        public static double FixedTime { get; private set; }

        public static float Delta { get; private set; }
        public static float FixedDelta { get; private set; }

        public static float UnscaledDelta { get; private set; }
        public static float FixedUnscaledDelta { get; private set; }

        public static float TimeScale { get; set; } = 1.0f;

        public static long FrameCount { get; private set; }
        public static long FixedFrameCount { get; private set; }

        private static double accumulator;
        public static float FixedStep { get; } = 1f / 60f;

        internal static void Update()
        {
            double now = Raylib.GetTime();
            UnscaledDelta = (float)(now - Time);
            Time = now;

            Delta = UnscaledDelta * TimeScale;

            accumulator += UnscaledDelta;

            FrameCount++;
        }

        internal static void FixedUpdate()
        {
            accumulator -= FixedStep;

            FixedUnscaledDelta = FixedStep;
            FixedDelta = FixedStep * TimeScale;

            FixedTime += FixedStep;
            FixedFrameCount++;
        }

        internal static bool ShouldRunFixedUpdate()
        {
            return accumulator >= FixedStep;
        }
    }
}
