using System.Diagnostics;
using Hwdtech;

namespace SpaceBattle.Lib;
public class GameCommand : ICommand
{
    private Queue<ICommand> _q;
    private object _scope;
    private Dictionary<ICommand, Exception> _ExceptDict;
    private Stopwatch stopwatch;
    public GameCommand(Queue<ICommand> q, object scope, Dictionary<ICommand, Exception> ExeptDict)
    {
        _q = q;
        _scope = scope;
        _ExceptDict = ExeptDict;
        stopwatch = new();
    }
    public void Execute()
    {
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        var timeQuant = IoC.Resolve<int>("Game.TimeQuant");
        while (_q.Count > 0 && timeQuant >= 0)
        {
            stopwatch.Start();
            var cmd = _q.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                try{
                    IoC.Resolve<ICommand>("ExceptionHandler.Checker", cmd, e, _ExceptDict).Execute();
                }
                catch(Exception e2)
                {
                    IoC.Resolve<ICommand>("ExceptionHandler.Checker", cmd, e2, _ExceptDict).Execute();
                }
            }
            stopwatch.Stop();
            timeQuant -= (int)stopwatch.ElapsedMilliseconds;
        }
    }
}