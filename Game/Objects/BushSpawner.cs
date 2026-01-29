namespace BerryGame
{
    public class BushSpawner : GameObject
    {
        private List<Bush> Bushes = [];
        public int BushLimit = 3;

        public override void Update()
        {
            Bushes = [.. Bushes.Where(static b => !b.Destroyed)];

            if (Bushes.Count < BushLimit)
                CreateBush();
        }

        public void CreateBush()
        {
            float width = Shared.WorldRect.Width;
            float height = Shared.WorldRect.Height;

            float ox = 128;
            float oy = 128;

            float dx = (Shared.RNG.NextSingle() * (width - ox)) + (ox / 2.0f);
            float dy = (Shared.RNG.NextSingle() * (height - oy)) + (oy / 2.0f);

            Bush bush = Manager.Create<Bush>(dx, dy, 64, 32);
            bush.BerryLimit = Shared.RNG.Next(2, 5);
            Bushes.Add(bush);
        }
    }
}
