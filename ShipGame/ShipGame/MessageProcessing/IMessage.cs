namespace ShipGame.MessageProcessing
{
    public interface IMessage
        {
            public string Gameid { get; }
            public string UObjectid { get; }
            public string Cmd { get; }
            public IDictionary<string, object> Args { get; }
        }
}
