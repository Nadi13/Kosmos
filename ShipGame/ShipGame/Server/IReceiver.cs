using ShipGame.Move;
namespace ShipGame.Server
{
    public interface IReceiver
    {
        public ICommand Receive();
        public bool IsEmpty();
    }
}
