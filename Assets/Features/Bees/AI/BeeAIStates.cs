using UnityEngine;

public abstract class BeeAIStates
{
    protected BeeController bee;
    protected BeeBrain brain;
    protected BeeResources bResource;
    protected HiveController hive;
    protected HiveResources hResource;
    
    public abstract void Tick();

    public BeeAIStates(BeeController bee)
    {
        this.bee = bee;
        hive = GameObject.FindObjectOfType<HiveController>();
        hResource = hive.GetComponent<HiveResources>();
        brain = this.bee.GetComponent<BeeBrain>();
        bResource = this.bee.GetComponent<BeeResources>();
    }
}
