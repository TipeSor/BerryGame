namespace BerryGame
{
    internal static class InteractionQueue
    {
        internal static readonly List<InteractionEntry> Entries = [];

        public static void Submit(InteractionEntry entry)
        {
            Entries.Add(entry);
        }

        public static void Flush()
        {
            Entries.Sort(static (a, b) => a.Z.CompareTo(b.Z));

            foreach (InteractionEntry cmd in Entries)
                cmd.Draw();

            Entries.Clear();
        }
    }
}
