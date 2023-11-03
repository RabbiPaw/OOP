

namespace SpaceBattle.Lib
{
    public interface ITurnable
    {
        public int Angle {get; set; }
        public int Turn {get;}
   }

    public class TurnCommand : ICommand
    {
        private readonly ITurnable turnable;
        public TurnCommand(ITurnable turnable)
        {
            this.turnable = turnable;
        }
        public void Execute()
        {   
            turnable.Angle=(int)(8F / 360 *(turnable.Angle + turnable.Turn)% 8F *360/ 8F);
        }
    }

}
