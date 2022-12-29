namespace ShipGame.Move
{
    public interface IQueue<T>
    {
        void Push(T elem);
        T Pop();
    }
}
