using UnityEngine;

public class BeeFlee : BeeAIStates
{
    public BeeFlee(BeeController bee) : base(bee)
    {
    }
    
    public override void Initialise()
    {
        bee.Target = hive.gameObject;
    }
    
    public override void Tick()
    {
        bee.OrbitPos(bee.Target.transform.position);
    }
}
