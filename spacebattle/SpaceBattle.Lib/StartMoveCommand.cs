using Hwdtech;

namespace SpaceBattle.Lib;
public class StartMoveCommand : ICommand
{
        private readonly IMoveCommandStartable _startable;

        public StartMoveCommand(IMoveCommandStartable startable) => _startable = startable;

        public void Execute(){
        
        _startable.Properties.ToList().ForEach(a => IoC.Resolve<ICommand>("Properties.Set", _startable.Target, a.Key, a.Value).Execute());
        var cmd = IoC.Resolve<ICommand>("Operations.Movement", _startable.Target);
        IoC.Resolve<ICommand>("Properties.Set", _startable.Target, "Operations.Movement", cmd).Execute();
        IoC.Resolve<IQueue>("Game.Queue").Put(cmd);
        }       
}
