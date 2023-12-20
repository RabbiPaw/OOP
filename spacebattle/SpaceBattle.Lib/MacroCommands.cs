using Hwdtech;
namespace SpaceBattle.Lib;
public class MacroCommands : ICommand
{
    private readonly List<ICommand> _commands = new();

    public MacroCommands(string ListOfCommands){
    var commandsList = IoC.Resolve<string[]>(ListOfCommands);
        foreach (var command in commandsList)
        {
            _commands.Add(IoC.Resolve<ICommand>(command));
        }
    }
    public void Execute(){
        foreach (var command in _commands){
            command.Execute();
        }
    }
}