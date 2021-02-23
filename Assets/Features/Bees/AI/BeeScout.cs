using UnityEngine;

public class BeeScout : BeeAIStates
{
    private float detectTimer = 0;
    private float scoutTimer = 0; 
    private Vector3 location = Vector3.zero;
    
    public BeeScout(BeeController bee) : base(bee)
    {
    }

    public override void Initialise()
    {
        
    }

    private void Timers()
    {
        scoutTimer += Time.deltaTime;
        detectTimer += Time.deltaTime;
    }

    public override void Tick()
    {
        Timers();
        
        if (!bResource.NewHuman)
        { 
            if (ReadyForNewScoutLocation()) NewScoutLocation();
            else bee.MoveToPos(location);
            
            if (detectTimer >= BeeSettings.DetectTime) 
            { 
                bResource.DetectNewHuman(); 
                detectTimer = 0.0f; 
            }
        }
        else
        {
            location = hive.transform.position;
            bee.MoveToPos(location);

            if (Vector3.Distance(bee.transform.position, hive.transform.position) < BeeSettings.TargetRadius)
            {
                //ADD BEE TO DRONE LIST
                //REMOVE BEE FROM SCOUT LIST
                hive.AddDetectedHuman(bResource.NewHuman);
                bResource.ResetScout();
            }
        }
    }

    private bool ReadyForNewScoutLocation()
    {
        if (Vector3.Distance(bee.transform.position, location) < BeeSettings.DetectionRad &&
            scoutTimer > BeeSettings.ScoutTime)
        {
            return true;
        }

        return false;
    }

    private void NewScoutLocation()
    {
        Vector3 newPos;
        newPos.x = hive.transform.position.x + Random.Range(-BeeSettings.ScoutRadiusX, BeeSettings.ScoutRadiusX); //left and right
        newPos.y = hive.transform.position.y + Random.Range(-BeeSettings.ScoutRadiusY, BeeSettings.ScoutRadiusY); //above and below
        newPos.z = hive.transform.position.z + Random.Range(0.0f, BeeSettings.ScoutRadiusZ); //cannot scout behind hiveController

        scoutTimer = 0;
        location = newPos;
    }
}
