namespace SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;

public class ExceptionChecker : ICommand
{
    private readonly Exception _exception;
    private readonly ICommand _cmd;

    private readonly Dictionary<ICommand, Exception> _ExceptionDict;
    public ExceptionChecker(ICommand cmd, Exception exception, Dictionary<ICommand, Exception> ExceptionDict)
    {
        _exception = exception;
        _cmd = cmd;
        _ExceptionDict = ExceptionDict;
    }

    public void Execute()
    {
        if (_ExceptionDict.ContainsKey(_cmd) && _ExceptionDict[_cmd] != null && _ExceptionDict[_cmd] == _exception)
        {
            IoC.Resolve<Exception>("ExceptionHandler.Handle", _cmd, _exception);
        }
        else
        {
            _ExceptionDict[_cmd] = _exception;
            throw _exception;
        }
    }
}