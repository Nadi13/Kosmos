namespace ShipGame.Builder
{
    public interface IBuilder
    {
        IBuilder addAnything(string param, params object[] args);
        object Build();
    }
}
