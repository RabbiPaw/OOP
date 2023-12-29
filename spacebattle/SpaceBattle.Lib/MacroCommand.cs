using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceBattle.Lib;

public class MacroCommand : ICommand
{
    private readonly CreatMacroCommand _cmd;
    public MacroCommand(CreatMacroCommand cmd){
        this._cmd = cmd;
    }
     public void Execute(){
        foreach(var c in _cmd.cmds){
        c.Execute();}
     }      
}
