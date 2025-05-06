using ShipGame.Move;

namespace ShipGame.GameState
{
    public class SetPositionCommand : ICommand
    {
        private IUObject obj;
        private Vector position;

        public SetPositionCommand(IUObject obj, Vector pos)
        {
            this.obj = obj;
            this.position = pos;
        }
        public void Execute()
        {
            obj.SetProperty("position", position);
        }
    }
}
