namespace ShipGame.Move
{
    public interface IMoveCommandStartable
    {
        IUObject Target 
        {
            get; 
        }
        IDictionary<string, object> Properties
        {
            get;
        }
    }
}
