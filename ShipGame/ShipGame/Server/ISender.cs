using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.Server
{
    public interface ISender
    {
        public void Send(ICommand command);
    }
}
