using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib;

public class MacroCommandTests
    {
        public MacroCommandTests()
        {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Command.MacroCommands.SomeCommand",(object[] args)=>{
            return new string[]{"FirstCommand","SecondCommand"};}).Execute();

     
        }
        [Fact]
        public void MacroCommand_Command_Init_Correctly()
        {
            var firstCommand=new Mock<ICommand>();
            var secondCommand=new Mock<ICommand>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","FirstCommand",(object[] args)=>{return firstCommand.Object;}).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","SecondCommand",(object[] args)=>{return secondCommand.Object;}).Execute();
            
            var macroCommand=new MacroCommands("Command.MacroCommands.SomeCommand");
            macroCommand.Execute();

            firstCommand.Verify(fc => fc.Execute(), Times.Once());
            secondCommand.Verify(sc => sc.Execute(), Times.Once());
        }
        [Fact]
        public void MacroCommand_Disability_To_Execute_SubCommand_Caueses_Exeption()
        {
            var firstCommand=new Mock<ICommand>();
            var secondCommand=new Mock<ICommand>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","FirstCommand",(object[] args)=>{return firstCommand.Object;}).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","SecondCommand",(object[] args)=>{return secondCommand.Object;}).Execute();
            firstCommand.Setup(f=>f.Execute()).Throws(new Exception()).Verifiable();

            var macroCommand=new MacroCommands("Command.MacroCommands.SomeCommand");

            Assert.Throws<Exception>(macroCommand.Execute);
        }
    }