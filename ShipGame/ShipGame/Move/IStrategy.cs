namespace ShipGame.Move
{
    public interface IStrategy
    {
        public object RunStrategy(params object[] args);
    }
}
