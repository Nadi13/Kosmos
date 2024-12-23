namespace ShipGame.Server
{
    public class TransactionStatus : ITransactionStatus
    {
        public TransactionStatus(object value, object transactionValue, int id){
            this.value = value;
            this.transactionValue = transactionValue;
            this.id = id;
        }

        private int id;
        private object transactionValue;
        private object value;

        public int GetId()
        {
            return id;
        }
        public void SetId(int id){
            this.id=id;
        }
        public void SetTransactionValue(object newValue){
            this.transactionValue=newValue;
        }

        public object getValue()
        {
            return value;
        }
        public bool IsNotEqualValues()
        {
            return !transactionValue.Equals(value);
        }

        public void Commited(){
            value = transactionValue;
        }
        public void Aborted(){
            transactionValue = value;
        }
    }
}
