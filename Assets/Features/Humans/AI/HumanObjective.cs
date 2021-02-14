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
        if (!_humanController.HasJelly)
        {
            _humanController.SetTarget(_humanController.HiveLocation());
        }
        else
        {
            _humanController.SetTarget(_humanController.DepotLocation());
        }
    }
}
