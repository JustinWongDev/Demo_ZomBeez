using UnityEngine;

public class BeeIdle : BeeAIStates
{
    public BeeIdle(BeeController bee) : base(bee)
    {
    }

    public override void Tick()
    {
        bee.OrbitTarget(hive.transform.position);
    }
}
