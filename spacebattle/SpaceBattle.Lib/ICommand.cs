namespace SpaceBattle.Lib;

public interface ICommand
{
    public void Execute();
    public void Check();
}