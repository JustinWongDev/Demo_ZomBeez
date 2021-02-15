using UnityEngine;

public class HumanDefensive : HumanAIState
{
    public HumanDefensive(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        //if too many bees around human (heuristic) -> find furthest corner to flee
        //if not too many bees around human (heuristic) -> look for items 
    }
}
