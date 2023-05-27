namespace ShipGame.Move
{
    public interface IUObject
    {
        IDictionary<string, object> scope { get; set; }
        object this[string key] { get; set; }
        void SetProperty(string key, object value);
        object GetProperty(string key);
        
    }
}
