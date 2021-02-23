using UnityEngine;

public class BeeForage : BeeAIStates
{
    private float collectionTimer = 0.0f;
    
    public BeeForage(BeeController bee) : base(bee)
    {
    }

    public override void Tick()
    {
        Timers();        
        
        if (AbleToForage())
        {
            bee.Target.GetComponent<HumanController>().LoseBrains(1);
            bResource.AddResource(1);

            collectionTimer = 0.0f;
        }
    }

    private void Timers()
    {
        collectionTimer += Time.deltaTime;
    }

    private bool AbleToForage()
    {
        if (Vector3.Distance(bee.transform.position, bee.Target.transform.position) <= 
            BeeSettings.ForageRadius &&
            bee.Target.GetComponent<HumanController>().Settings.Brains > 0 &&
            collectionTimer >= BeeSettings.CollectionTime)
        {
            return true;
        }

        return false;
    }
}
