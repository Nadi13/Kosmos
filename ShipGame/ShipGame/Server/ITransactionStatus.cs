namespace ShipGame.Server
{
    public interface ITransactionStatus
    {
        public object getValue();
        public void SetTransactionValue(object newValue);
        public int GetId();
        public void SetId(int id);
        public bool IsNotEqualValues();
        public void Commited();
        public void Aborted();
    }
}