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

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Command.MacroCommand.SomeMacroCommand",(object[] args)=>{
            return new string[]{"Move","Rotate"};}).Execute();

     
        }
        [Fact]
        public void MacroCommand_Command_Init_Correctly()
        {
            var Obj = new Mock<IUObject>();
            var Move =new Mock<ICommand>();
            var Rotate =new Mock<ICommand>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Move",(object[] args)=>{return Move.Object;}).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Rotate",(object[] args)=>{return Rotate.Object;}).Execute();
            
            var macroCommand=new CreatMacroCommand("Command.MacroCommand.SomeMacroCommand");
            macroCommand.Execute();
            var Command = new MacroCommand(macroCommand);
            Command.Execute();
            Move.Verify(mc => mc.Execute(), Times.Once());
            Rotate.Verify(cfc => cfc.Execute(), Times.Once());
        }
        [Fact]
        public void MacroCommand_Disability_To_Execute_SubCommand_Caueses_Exeption()
        {
            var Move =new Mock<ICommand>();
            var Rotate =new Mock<ICommand>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Move",(object[] args)=>{return Move.Object;}).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Rotate",(object[] args)=>{return Rotate.Object;}).Execute();
            Move.Setup(f=>f.Execute()).Throws(new Exception()).Verifiable();

            var macroCommand=new CreatMacroCommand("Command.MacroCommand.SomeMacroCommand");
            macroCommand.Execute();
            var Command = new MacroCommand(macroCommand);

            Assert.Throws<Exception>(Command.Execute);
        }
    }