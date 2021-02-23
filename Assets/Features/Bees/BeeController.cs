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

    public void MoveToPos(Vector3 targetPos)
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

    public void OrbitPos(Vector3 targetPos)
    {
        targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        adjRotSpeed = Mathf.Min(BeeSettings.RotSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
        rb.AddRelativeForce(Vector3.forward * (BeeSettings.Speed * 20 * Time.deltaTime));
    }
}
