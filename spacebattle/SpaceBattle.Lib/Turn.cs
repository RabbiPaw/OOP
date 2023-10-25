namespace SpaceBattle.Lib
{
    public interface ITurnable
    {
        public int Anglle {get; set; }
        public int Turn {get;}
    }
    public class TurnCommand : ICommand{
        private readonly ITurnable turnable;
        public TurnCommand(ITurnable turnable)
        {
            this.turnable = turnable;
        }
        public void Execute(){
            turnable.Anglle += turnable.Turn;
        }

    }
}