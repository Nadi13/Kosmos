namespace ShipGame.Move
{
    public interface IMoveCommandEndable
    {
        ICommand MoveCommand 
        {
            get;
        } 
        IUObject Item
        {
            get; 
        }
        IQueue<ICommand> Obj
        {
            get;
        }
    }
}
