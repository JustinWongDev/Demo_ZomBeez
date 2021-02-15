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
        
        if (_move.AtDestination())
        {
            _move.SetTarget(_move.ClosestActiveForageSite());
        }
    }
}
