using UnityEngine;

public class HumanDrop : HumanAIState
{
    public HumanDrop(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        Debug.Log("DROP");
    }
    
}
