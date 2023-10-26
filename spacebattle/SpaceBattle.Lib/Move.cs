namespace SpaceBattle.Lib;

public interface IMovableTurnable
{
    public Vector Position { get; set; }
    public Vector Velocity { get; }
    public MoveAbility MoveAbility { get;}
    public Angles Angle {get; set; }
    public Angles Turn {get;}
    public TurnAbility TurnAbility{get;}
}

public class MoveTurnCommand : ICommand
{
    private readonly IMovableTurnable movable_or_turnable;
    public MoveTurnCommand(IMovableTurnable movable_or_turnable)
    {
        this.movable_or_turnable = movable_or_turnable;
    }
    public void Execute()
    {   
        if ( movable_or_turnable.Position != null && movable_or_turnable.Velocity != null){
            movable_or_turnable.Position += movable_or_turnable.Velocity;
        }
        if ( movable_or_turnable.Angle != null && movable_or_turnable.Turn != null){
            movable_or_turnable.Angle += movable_or_turnable.Turn;
        }
    }
    public void Check(){
        if ( movable_or_turnable.Position != null && movable_or_turnable.Velocity != null){
            Vector.IsNotNull(movable_or_turnable.Position);
            Vector.IsNotNull(movable_or_turnable.Velocity);
        }
        if ( movable_or_turnable.Angle != null && movable_or_turnable.Turn != null){
            Angles.UncorrectInput(movable_or_turnable.Angle);
            Angles.UncorrectInput(movable_or_turnable.Turn);
        }
    }
    public void MoveAbilityCheck(){
        if (movable_or_turnable.MoveAbility == new MoveAbility(false)){
            throw new System.Exception();
        }
    }
    public void TurnAbilityCheck(){
        if (movable_or_turnable.TurnAbility == new TurnAbility(false)){
            throw new Exception();
        }
    }
}