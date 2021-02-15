using System;
using UnityEngine;

public class Worker : Enemy
{
    [Header("General")]
    public BeeBehaviours beeBehaviour;
    public enum BeeBehaviours
    {
        Idle,
        Scout,
        Forage,
        Attack,
        Flee
    }
    public Hive hive;
    private Vector3 idlePosition;
    private float idleTimer = 0;
    
    //Stats
    private int damage;
    private float attackOrFlee;
    [HideInInspector]
    public float scoutFit = 0;
    public float attackFit = 0;

    [Header("Scouting")]
    public float detectTimer = 0.25f;
    
    [Header("Movement")]
    private float adjRotSpeed;
    private Quaternion targetRotation;
    public GameObject target;

    [Header("Boiding")]
    private Vector3 cohesionPos = new Vector3(0f, 0f, 0f);
    //private int boidIndex = 0;

    [Header("Foraging")] 
    public int collectedResource = 0;
    private float collectionTimer = 0;
    public bool humanEmpty = false;
    private int newHumanResource = 0;
    private HumanController newHuman = null;
    public HumanController NewHuman => newHuman;

    //Prey Variables
    //private float outOfRangeRatio = 0.05f;
    //private float fleeRadius = 300.0f;

    //Predator Variables
    private Vector3 tarVel;
    private Vector3 tarPrevPos;
    private Vector3 attackPos;
    //private float distanceRatio = 0.05f;
    //private int beamDamage = 100;

    GameManager gameManager => FindObjectOfType<GameManager>();
    Rigidbody rb => GetComponent<Rigidbody>();

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        FSMController();
    }

    #region Start up
    private void Setup()
    {
        idlePosition = hive.transform.position;

        //Drone idly roam around mothership
        beeBehaviour = BeeBehaviours.Idle;
        target = hive.gameObject;

        //Randomise Stats
        damage = BeeSettings.Damage;
        health = BeeSettings.Health;
        collectionTimer = BeeSettings.CollectionTime;
        
        //Calculate scout and attack heuristics
        HeuristicAttack();
        HeuristicScout();
}

    //OUTDATED WITH BEESETTINGS
     private void HeuristicScout()
     {
         float speedScore = BeeSettings.Speed / BeeSettings.Speed;
         float detectScore = BeeSettings.DetectionRad / BeeSettings.DetectionRad;
    
         scoutFit = speedScore * 0.8f + detectScore * 0.2f;
     }    
    
     private void HeuristicAttack()
     {
         float healthScore = health / BeeSettings.Health;
         float damageScore = damage / BeeSettings.Damage;
    
         float carryScore = BeeSettings.Capacity / BeeSettings.Capacity;
         float collectScore = BeeSettings.CollectionTime / BeeSettings.CollectionTime;
    
         attackFit = (healthScore * 0.2f) + (damage * 0.5f) - (carryScore * 0.2f) - (collectScore * 0.1f);
     }

    public void Initialise(Hive hive, GameObject initTarget)
    {
        this.hive = hive;
        this.target = initTarget;
    }
#endregion

/// <summary>
    /// Move towards Vector3 parameter
    /// </summary>
    /// <param name="targetPos"></param>
    private void MoveTowardsTarget(Vector3 targetPos)
    {
        //Rotate and move towards target if out of range
        if (Vector3.Distance(targetPos, transform.position) > BeeSettings.TargetRadius)
        {
            //Lerp Towards target
            targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            adjRotSpeed = Mathf.Min(BeeSettings.RotSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
            rb.AddRelativeForce(Vector3.forward * BeeSettings.Speed * 20 * Time.deltaTime);
        }
    }

    /// <summary>
    /// Directs FSM to correct method calls, based on bee's current behaviour 
    /// </summary>
    private void FSMController()
    {
        switch (beeBehaviour)
        {
            case BeeBehaviours.Idle:
                Idle();
                break;
            case BeeBehaviours.Scout:
                Scout();
                break;
            case BeeBehaviours.Forage:
                Forage();
                break;
            case BeeBehaviours.Attack:
                Attack();
                break;
            case BeeBehaviours.Flee:
                Flee();
                break;
        }
    }

    #region Flee
    private void Flee()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Attack
    private void Attack()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Forage
    private void Forage()
    {
        //Check if resource available or target alive
        if (target.GetComponent<HumanController>().Settings.Brains <= 0 || target == null)
        {
            humanEmpty = true;
        }

        //If full capacity or resource unavailable, return to hive, otherwise...
        if (collectedResource >= BeeSettings.Capacity | humanEmpty)
        {
            MoveTowardsTarget(hive.transform.position);

            if (Vector3.Distance(transform.position, hive.transform.position) <= BeeSettings.TargetRadius)
            {
                hive.collectedResource += collectedResource;
                collectedResource = 0;
                beeBehaviour = BeeBehaviours.Idle;
            }
        }
        //Move to target
        else
        {
            MoveTowardsTarget(target.transform.position);
        }

        CollectResource();
    }

    void CollectResource()
    {
        //Must be within foraging distance and human must have resource available
        if (Vector3.Distance(transform.position, target.transform.position) <= BeeSettings.ForageRadius &&
           target.GetComponent<HumanController>().Settings.Brains > 0)
        {
            //Must spend time within radius before collecting resource
            if (Time.time > collectionTimer)
            {
                target.GetComponent<HumanController>().LoseBrains(1);
                collectedResource += 1;

                collectionTimer = Time.time + BeeSettings.CollectionTime;
            }
        }
    }

    #endregion

    #region Scout
    private void Scout()
    {
        //If no new human found...
        if (!newHuman)
        {
            //If within range of selected scouting area, find new scouting area
            if (Vector3.Distance(transform.position, idlePosition) < BeeSettings.DetectionRad && Time.time > idleTimer)
            {
                //Gen new, random scouting area
                Vector3 newPos;
                newPos.x = hive.transform.position.x + UnityEngine.Random.Range(-BeeSettings.ScoutRadiusX, BeeSettings.ScoutRadiusX); //left and right
                newPos.y = hive.transform.position.y + UnityEngine.Random.Range(-BeeSettings.ScoutRadiusY, BeeSettings.ScoutRadiusY); //above and below
                newPos.z = hive.transform.position.z + UnityEngine.Random.Range(0.0f, BeeSettings.ScoutRadiusZ); //cannot scout behind hive

                idlePosition = newPos;

                idleTimer = Time.time + BeeSettings.ScoutTime;
            }
            else
            {
                MoveTowardsTarget(idlePosition);
                //Debug.DrawLine(transform.position, idlePosition, Color.white);
            }

            //Every few seconds, check for new resources
            if (Time.time > detectTimer)
            {
                newHuman = DetectNewHuman();
                detectTimer = Time.time + BeeSettings.DetectTime;
            }
        }
        //Human found, head back to hive
        else
        {
            target = hive.gameObject;
            //Debug.DrawLine(transform.position, hive.gameObject.transform.position, Color.green);

            MoveTowardsTarget(target.transform.position);

            //Relay information to hive, then reset bee
            if (Vector3.Distance(transform.position, hive.transform.position) < BeeSettings.TargetRadius)
            {
                //ADD BEE TO DRONE LIST
                //REMOVE BEE FROM SCOUT LIST

                hive.detectedHumans.Add(newHuman);

                newHumanResource = 0;
                newHuman = null;

                beeBehaviour = BeeBehaviours.Scout;
            }
        }
    }

    private HumanController DetectNewHuman()
    {
        //Go through all active humans, within detection radius, dropped, and not already detected 
        foreach (HumanController human in hive.activeHumans)
        {
            if (Vector3.Distance(transform.position, human.transform.position) <= BeeSettings.DetectionRad &&
                !human.GetComponent<Droppable>() &&
                !hive.detectedHumans.Contains(human))
            {
                newHuman = human;
                return newHuman;
            }
        }

        //if criteria not met...
        return null;

        // //Go through all active humans
        // for (int i = 0; i < hive.activeHumans.Count; i++)
        // {
        //     //Check if human in detect range
        //     if (Vector3.Distance(transform.position, hive.activeHumans[i].transform.position) <= BeeSettings.DetectionRad &&
        //         !hive.activeHumans[i].GetComponent<Droppable>())
        //     {
        //         //Find best human 
        //         if (hive.activeHumans[i].GetComponent<HumanMove>().CurrentBrains > newHumanResource)
        //         {
        //             newHuman = hive.activeHumans[i];
        //         }
        //     }
        // }
        //
        // if (hive.detectedHumans.Contains(newHuman))
        // {
        //     return null;
        // }
        // else
        // {
        //     return newHuman;
        // }


    }
#endregion

    #region Idle
    private void Idle()
    {
        OrbitTarget(hive.transform.position);
    }

    private void OrbitTarget(Vector3 targetPos)
    {
        targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        adjRotSpeed = Mathf.Min(BeeSettings.RotSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
        rb.AddRelativeForce(Vector3.forward * BeeSettings.Speed * 20 * Time.deltaTime);
    }
    #endregion
}
