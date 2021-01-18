using UnityEngine;

public class HumanFlee : HumanAIState
{
    public HumanFlee(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        Debug.Log("FLEE");
    }
}
