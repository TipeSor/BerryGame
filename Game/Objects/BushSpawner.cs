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
            float dx = Shared.RNG.NextSingle() * 512;
            float dy = Shared.RNG.NextSingle() * 512;

            Bush bush = Manager.Create<Bush>(128 + dx, 128 + dy, 64, 32);
            bush.BerryLimit = 5;
            Bushes.Add(bush);
        }
    }
}
