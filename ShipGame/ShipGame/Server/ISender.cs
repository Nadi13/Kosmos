using ShipGame.Move;

namespace ShipGame.Server
{
    public interface ISender
    {
        public void Send(ICommand command);
    }
}
