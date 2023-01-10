using Hwdtech;
namespace ShipGame.DecisionTree
{
    public class TreeCreation : ICommand
    {
        private string file;
        public TreeCreation(string file)
        {
            this.file = file;
        }

        public void Execute()
        {
            var decision = IoC.Resolve<Dictionary<int, object>>("Create.Tree");
            try
            {
                using (StreamReader st = File.OpenText(file))
                {
                    string line;
                    while ((line = st.ReadLine()) != null)
                    {
                        var fill = line.Split().Select(c => Convert.ToInt32(c)).ToList();

                        foreach (var x in fill)
                        {
                            decision.TryAdd(x, new Dictionary<int, object>());
                            decision = (Dictionary<int, object>)decision[x];
                        }
                    }
                }
            }
            catch (FileNotFoundException x)
            {
                throw new FileNotFoundException(x.ToString());
            }
            catch (Exception x)
            {
                throw new Exception(x.ToString());
            }

        }
    }
}
