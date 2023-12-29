using Hwdtech;

namespace SpaceBattle.Lib;

public class CreatMacroCommand : ICommand
{
    private readonly string _CommandsNames;
    public List<ICommand> cmds = new();

    public CreatMacroCommand(string commands) {
        this._CommandsNames = commands;
    }
    public void Execute(){
        var NamesOfArray = IoC.Resolve<string[]>(_CommandsNames);
        foreach ( var cmd in NamesOfArray)
        {
            cmds.Add(IoC.Resolve<ICommand>(cmd));
        }
    }
}