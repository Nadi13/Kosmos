using ICommand = ShipGame.Move.ICommand;
using Hwdtech;

namespace ShipGame.Game
{
    public class SetScopeCommand: ICommand
    {
        string id;
        GameCommand GameCommand;
        object GameScope;
        public SetScopeCommand(string id, object GameScope, Queue<ICommand> queue)
        {
            this.id = id;
            this.GameCommand = new GameCommand(id, queue);
            this.GameScope = GameScope;
        }
        public void Execute()
        {
            var InitialScope = IoC.Resolve<object>("ThreadScope.Current", id);
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", this.GameScope).Execute();

            GameCommand.Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", InitialScope).Execute();
        }
    }
}
