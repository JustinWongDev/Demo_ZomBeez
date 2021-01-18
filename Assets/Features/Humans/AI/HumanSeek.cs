using UnityEngine;

public class HumanSeek : HumanAIState
{
    public HumanSeek(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {        
        Debug.Log("SEEK");
        
        if (_humanController.AtDestination())
        {
            _humanController.Target = _humanController.ClosestActiveForageSite();
        }
    }
}
