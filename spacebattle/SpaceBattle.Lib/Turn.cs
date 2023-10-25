namespace SpaceBattle.Lib
{
    public interface ITurnable
    {
        public Angles Anglle {get; set; }
        public Angles Turn {get;}
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