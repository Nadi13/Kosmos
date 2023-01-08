namespace ShipGame.Move
{
    public interface IMoveCommandEndable
    {
        ICommand MoveCommand 
        {
            get;
        } 
        IUObject ObjMove
        {
            get; 
        }
        IQueue<ICommand> Obj
        {
            get;
        }
    }
}
