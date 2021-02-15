using UnityEngine;

public class HumanObjective : HumanAIState
{
    public HumanObjective(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        if (!_human.Settings.GetHasJelly())
        {
            _move.SetTarget(_move.HiveLocation());
        }
        else
        {
            _move.SetTarget(_move.DepotLocation());
        }
    }
}
