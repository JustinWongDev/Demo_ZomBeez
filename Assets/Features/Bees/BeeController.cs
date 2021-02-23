using UnityEngine;

public class BeeController : Enemy
{
    [Header("General")]
    private HiveController hive = null;
    private HiveResources resourceH = null;
    private BeeBrain brain = null;
    private BeeResources resourceB = null;
    private Rigidbody rb => GetComponent<Rigidbody>();
    
    private int damage = 0;
    private float attackOrFlee = 0;
    public float scoutFit = 0;
    public float attackFit = 0;
    
    private float adjRotSpeed;
    private Quaternion targetRotation;
    private GameObject target;
    
    private Vector3 cohesionPos = new Vector3(0f, 0f, 0f);
    //private int boidIndex = 0;

    //Prey Variables
    //private float outOfRangeRatio = 0.05f;
    //private float fleeRadius = 300.0f;

    //Predator Variables
    private Vector3 tarVel;
    private Vector3 tarPrevPos;
    private Vector3 attackPos;
    //private float distanceRatio = 0.05f;
    //private int beamDamage = 100;

    private BeeAIStates _currentAIState;

    public BeeAIStates CurrentAIState
    {
        get => _currentAIState;
        set
        {
            if (value != _currentAIState)
            {
                _currentAIState = value;
            }
        }
    }

    public void RemoveTarget() => target = null;

    public GameObject Target
    {
        get => target;
        set
        { 
            target = value;
        }
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

    public void Initialise(HiveController hiveController, HiveResources resources, GameObject initTarget)
    {
        this.hive = hiveController;
        this.resourceH = resources;
        this.target = initTarget;
        this.brain = GetComponent<BeeBrain>();
        this.resourceB = GetComponent<BeeResources>();

        //Drone idly roam around mothership
        target = hiveController.gameObject;

        //Randomise Stats
        damage = (int)(BeeSettings.Damage * UnityEngine.Random.Range(0.8f, 1.2f));
        health = (int)(BeeSettings.Health * UnityEngine.Random.Range(0.8f, 1.2f));
        transform.rotation = Random.rotation;

        //Calculate scout and attack heuristics
        HeuristicAttack();
        HeuristicScout();
    }

    public void MoveTowardsTarget(Vector3 targetPos)
    {
        //Rotate and move towards target if out of range
        if (Vector3.Distance(targetPos, transform.position) > BeeSettings.TargetRadius)
        {
            //Lerp Towards target
            targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            adjRotSpeed = Mathf.Min(BeeSettings.RotSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
            rb.AddRelativeForce(Vector3.forward * (BeeSettings.Speed * 20 * Time.deltaTime));
        }
    }

    public void OrbitTarget(Vector3 targetPos)
    {
        targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        adjRotSpeed = Mathf.Min(BeeSettings.RotSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
        rb.AddRelativeForce(Vector3.forward * (BeeSettings.Speed * 20 * Time.deltaTime));
    }
    

    
    
    // private void Forage()
    // {
    //     //Check if resourceH available or target alive
    //     if (target == null)
    //     {
    //         beeBehaviour = BeeBehaviour.Idle;
    //         return;
    //     }
    //
    //     //If full capacity or resourceH unavailable, return to hive, otherwise...
    //     if (collectedResource >= BeeSettings.Capacity)
    //     {
    //         MoveTowardsTarget(hive.transform.position);
    //
    //         if (Vector3.Distance(transform.position, hive.transform.position) <= BeeSettings.TargetRadius)
    //         {
    //             resourceH.GainBrains(collectedResource);
    //             collectedResource = 0;
    //             beeBehaviour = BeeBehaviour.Idle;
    //         }
    //     }
    //     //Move to target
    //     else
    //     {
    //         MoveTowardsTarget(target.transform.position);
    //     }
    //
    //     CollectResource();
    // }


    // private void Scout()
    // {
    //     //If no new human found...
    //     if (!newHuman)
    //     {
    //         //If within range of selected scouting area, find new scouting area
    //         if (Vector3.Distance(transform.position, idlePosition) < BeeSettings.DetectionRad && Time.time > idleTimer)
    //         {
    //             //Gen new, random scouting area
    //             Vector3 newPos;
    //             newPos.x = hive.transform.position.x + UnityEngine.Random.Range(-BeeSettings.ScoutRadiusX, BeeSettings.ScoutRadiusX); //left and right
    //             newPos.y = hive.transform.position.y + UnityEngine.Random.Range(-BeeSettings.ScoutRadiusY, BeeSettings.ScoutRadiusY); //above and below
    //             newPos.z = hive.transform.position.z + UnityEngine.Random.Range(0.0f, BeeSettings.ScoutRadiusZ); //cannot scout behind hive
    //
    //             idlePosition = newPos;
    //
    //             idleTimer = Time.time + BeeSettings.ScoutTime;
    //         }
    //         else
    //         {
    //             MoveTowardsTarget(idlePosition);
    //             //Debug.DrawLine(transform.position, idlePosition, Color.white);
    //         }
    //
    //         //Every few seconds, check for new resourceB
    //         if (Time.time > detectTimer)
    //         {
    //             newHuman = DetectNewHuman();
    //             detectTimer = Time.time + BeeSettings.DetectTime;
    //         }
    //     }
    //     //Human found, head back to hive
    //     else
    //     {
    //         target = hive.gameObject;
    //         //Debug.DrawLine(transform.position, hive.gameObject.transform.position, Color.green);
    //
    //         MoveTowardsTarget(target.transform.position);
    //
    //         //Relay information to hive, then reset BeeController
    //         if (Vector3.Distance(transform.position, hive.transform.position) < BeeSettings.TargetRadius)
    //         {
    //             //ADD BEE TO DRONE LIST
    //             //REMOVE BEE FROM SCOUT LIST
    //
    //             hive.AddDetectedHuman(newHuman);
    //
    //             newHumanResource = 0;
    //             newHuman = null;
    //
    //             beeBehaviour = BeeBehaviour.Scut;
    //         }
    //     }
    // }
}
