using ICommand = ShipGame.Move.ICommand;

namespace ShipGame.Server
{
    public class SendCommand: ICommand
    {
        private ISender sndr;
        private ICommand cmd;
        public SendCommand(ISender sndr, ICommand cmd)
        {
            this.sndr = sndr;
            this.cmd = cmd;
        }
        public void Execute()
        {
            sndr.Send(cmd);
        }
    }
}
