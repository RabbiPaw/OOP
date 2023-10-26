

namespace SpaceBattle.Lib
{
    public interface ITurnable
    {
        public Angles Angle {get; set; }
        public Angles Turn {get;}
        public TurnAbility TurnAbility{get;}
    }
    public class TurnCommand : ICommand{
        private readonly ITurnable turnable;
        public TurnCommand(ITurnable turnable)
        {
            this.turnable = turnable;
        }
        public void Execute(){
            turnable.Angle += turnable.Turn;
        }
        public void Check(){
            Angles.UncorrectInput(turnable.Angle);
            Angles.UncorrectInput(turnable.Turn);
        }
        public void TurnAbilityCheck(){
            if (turnable.TurnAbility == new TurnAbility(false))
            {
            throw new Exception();
            }
        }

    }
}