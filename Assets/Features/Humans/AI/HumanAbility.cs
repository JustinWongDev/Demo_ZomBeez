using UnityEngine;

public class HumanAbility : HumanAIState
{
    public HumanAbility(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        Debug.Log("ABILITy");
    }
}
