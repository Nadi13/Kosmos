using Hwdtech;
using ShipGame.Move;

namespace ShipGame.Server
{
    public class UObject : IUObject
    {
        public UObject(Dictionary<string, ITransactionStatus> uObjectProperties){
            this.uObjectProperties = uObjectProperties;
        }
        Dictionary<string, ITransactionStatus> uObjectProperties;

        public void SetProperty(string key, object value)
        {
            if (uObjectProperties.ContainsKey(key)){
                if (IoC.Resolve<bool>("TransactionManager.GetTransactionStatus", uObjectProperties[key].GetId())){
                        uObjectProperties[key].Commited();
                    }
                uObjectProperties[key].SetTransactionValue(value);
                uObjectProperties[key].SetId(IoC.Resolve<int>("TransactionManager.GetCurrentTransactionID"));
            }
            else
            {
                uObjectProperties.Add(key, new TransactionStatus(new object(), value, IoC.Resolve<int>("TransactionManager.GetCurrentTransactionID")));
            }
        }

        public object GetProperty(string key)
        {
            if (uObjectProperties.ContainsKey(key))
            {
                if (uObjectProperties[key].IsNotEqualValues()){
                    if (IoC.Resolve<bool>("TransactionManager.GetTransactionStatus", uObjectProperties[key].GetId())){
                        uObjectProperties[key].Commited();
                    }
                    else
                    {
                        uObjectProperties[key].Aborted();
                    }
                }
                return uObjectProperties[key].getValue();
            }
            else{
                throw new KeyNotFoundException();
            }
        }
    }
}