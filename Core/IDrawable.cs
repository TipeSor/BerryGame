namespace BerryEngine
{
    public interface IDrawable
    {
        int Layer { get; }
        int Depth { get; }

        void Draw();
    }
}
