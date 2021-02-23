using System;
using UnityEngine;

public class BeeResources : MonoBehaviour
{
    private HiveController hive = null;
    private BeeController bee = null;
    private BeeBrain brain = null;

    private int collectedResource = 0;
    private bool humanEmpty = false;
    private int newHumanResource = 0;
    private HumanController newHuman = null;
    public HumanController NewHuman => newHuman;

    public void Initialise(HiveController hive, BeeController bee)
    {
        this.hive = hive; 
        this.bee = bee;
        this.brain = bee.GetComponent<BeeBrain>();
    }

    public void ResetScout()
    {
        newHumanResource = 0;
        newHuman = null;
        brain.SetNewState(new BeeIdle(this.GetComponent<BeeController>()));
    }

    public void DetectNewHuman()
    {
        foreach (HumanController human in hive.ActiveHumans)
        {
            if (Vector3.Distance(transform.position, human.transform.position) <= BeeSettings.DetectionRad &&
                !human.GetComponent<Droppable>() &&
                !hive.DetectedHumans.Contains(human))
            {
                newHuman = human;
            }
        }
    }

    public void AddResource(int resourceCollected) => collectedResource += resourceCollected;
    
}
