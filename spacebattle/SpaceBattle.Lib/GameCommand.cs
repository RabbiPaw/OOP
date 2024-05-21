namespace SpaceBattle.Lib;

using System.Diagnostics;
using System.Security;
using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib;


class GameCommand: ICommand {
    private Queue<ICommand> _q;
    private object scope;
    public GameCommand(Queue<ICommand> q, object scope) {
        _q = q;
        _scope = scope;
    }
    public void Execute() {
        Stopwatch stopwatch= new Stopwatch();
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        var timeQuant = IoC.Resolve<int>("Game.TimeQuant");
        while (q.Count > 0 && timeQuant >= 0) {
            stopwatch.Start();
            var cmd = q.Take();
            try {
                cmd.Execute();
            } catch (Exception e) {
                IoC.Resolve<ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
            } 
            stopwatch.Stop();
            timeQuant -= stopwatch.ElapsedMilliseconds;
        }
    }
}