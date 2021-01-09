using System;
using UnityEngine;

public class Worker : Enemy
{
    //Stats
    public float damage = 20;


    private float attackOrFlee;
    [HideInInspector]
    public float scoutFit = 0;
    public float attackFit = 0;
    private const float MAX_SPEED = 80;
    private const int MAX_COLLECTION_TIME = 6;
    private const int MAX_CAPACITY = 5;
    private const int MAX_DAMAGE = 20;
    private const float MAX_HEALTH = 100;
    private const float MAX_DETECTION = 20;

    [Header("Movement")]
    public float speed = 50.0f;
    public float rotationSpeed = 5.0f;
    private float adjRotSpeed;
    private Quaternion targetRotation;
    public GameObject target;
    public float targetRadius = 200f;

    [Header("Boiding")]
    public float separationDistance = 25.0f;
    public float cohesionDistance = 50.0f;
    public float separationStrength = 250.0f;
    public float cohesionStrength = 25.0f;
    private Vector3 cohesionPos = new Vector3(0f, 0f, 0f);
    private int boidIndex = 0;

    [Header("Foraging")]
    public float collectionTime = 6.0f;
    private float collectionTimer = 0.0f;
    public int maxCapacity = 5;
    public int collectedResource = 0;
    public float forageRadius = 20.0f;

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
    //public float currentFuel;
    //public float totalFuel;

    [Header("Detection")]
    public float detectRadius = 20.0f;
    public float scoutRadiusX = 40.0f;
    public float scoutRadiusY = 5.0f;
    public float scoutRadiusZ = 70.0f;
    public float detectTimer = 0.25f;
    public float scoutTime = 5.0f;

    [Header("Foraging")]
    public bool humanEmpty = false;
    private int newHumanResource = 0;
    private HumanController newHuman = null;

    //Prey Variables
    private float outOfRangeRatio = 0.05f;
    private float fleeRadius = 300.0f;

    //Predator Variables
    private Vector3 tarVel;
    private Vector3 tarPrevPos;
    private Vector3 attackPos;
    private float distanceRatio = 0.05f;
    private int beamDamage = 100;

    GameManager gameManager;
    Rigidbody rb;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        FSMController();
    }

    private void Setup()
    {
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager = FindObjectOfType<GameManager>();

        rb = GetComponent<Rigidbody>();
        idlePosition = hive.transform.position;

        //Drone idly roam around mothership
        beeBehaviour = BeeBehaviours.Idle;
        target = hive.gameObject;

        //Randomise Stats
        collectionTime = MAX_COLLECTION_TIME * UnityEngine.Random.Range(0.5f, 1.0f);
        maxCapacity = (int)(MAX_CAPACITY * UnityEngine.Random.Range(0.5f, 1.0f));
        damage = MAX_DAMAGE * UnityEngine.Random.Range(0.5f, 1.0f);
        speed = MAX_SPEED * UnityEngine.Random.Range(0.5f, 1.0f);
        health = MAX_HEALTH * UnityEngine.Random.Range(0.5f, 1.0f);
        detectRadius = MAX_DETECTION * UnityEngine.Random.Range(0.5f, 1.0f);

        //Calculate scout and attack heuristics
        HeuristicAttack();
        HeuristicScout();
}

    private void HeuristicScout()
    {
        float speedScore = speed / MAX_SPEED;
        float detectScore = detectRadius / MAX_DETECTION;

        scoutFit = speedScore * 0.8f + detectScore * 0.2f;
    }    
    
    private void HeuristicAttack()
    {
        float healthScore = health / MAX_HEALTH;
        float damageScore = damage / MAX_DAMAGE;

        float carryScore = maxCapacity / MAX_CAPACITY;
        float collectScore = collectionTime / MAX_COLLECTION_TIME;

        attackFit = (healthScore * 0.2f) + (damage * 0.5f) - (carryScore * 0.2f) - (collectScore * 0.1f);
    }

    public void Initialise(Hive hive, GameObject initTarget)
    {
        this.hive = hive;
        this.target = initTarget;
    }

    /// <summary>
    /// Move towards Vector3 parameter
    /// </summary>
    /// <param name="targetPos"></param>
    private void MoveTowardsTarget(Vector3 targetPos)
    {
        //Rotate and move towards target if out of range
        if (Vector3.Distance(targetPos, transform.position) > targetRadius)
        {
            //Lerp Towards target
            targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            adjRotSpeed = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
            rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime);
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
        if (target.GetComponent<HumanController>().resource <= 0 || target == null)
        {
            humanEmpty = true;
        }

        //If full capacity or resource unavailable, return to hive, otherwise...
        if (collectedResource >= maxCapacity | humanEmpty)
        {
            MoveTowardsTarget(hive.transform.position);

            if (Vector3.Distance(transform.position, hive.transform.position) <= targetRadius)
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
    #endregion

    void CollectResource()
    {
        //Must be within foraging distance and human must have resource available
        if (Vector3.Distance(transform.position, target.transform.position) <= forageRadius &&
           target.GetComponent<HumanController>().resource > 0)
        {
            //Must spend time within radius before collecting resource
            if (Time.time > collectionTimer)
            {
                target.GetComponent<HumanController>().LoseResource(1);
                collectedResource += 1;

                collectionTimer = Time.time + collectionTime;
            }
        }
    }

    #region Scout
    private void Scout()
    {
        //If no new human found...
        if (!newHuman)
        {
            //If within range of selected scouting area, find new scouting area
            if (Vector3.Distance(transform.position, idlePosition) < detectRadius && Time.time > idleTimer)
            {
                //Gen new, random scouting area
                Vector3 newPos;
                newPos.x = hive.transform.position.x + UnityEngine.Random.Range(-scoutRadiusX, scoutRadiusX); //left and right
                newPos.y = hive.transform.position.y + UnityEngine.Random.Range(-scoutRadiusY, scoutRadiusY); //above and below
                newPos.z = hive.transform.position.z + UnityEngine.Random.Range(0.0f, scoutRadiusZ); //cannot scout behind hive

                idlePosition = newPos;

                idleTimer = Time.time + scoutTime;
            }
            else
            {
                MoveTowardsTarget(idlePosition);
                Debug.DrawLine(transform.position, idlePosition, Color.white);
            }

            //Every few seconds, check for new resources
            if (Time.time > detectTimer)
            {
                newHuman = DetectNewHuman();
                detectTimer = Time.time + detectTimer;
            }
        }
        //Human found, head back to hive
        else
        {
            target = hive.gameObject;
            Debug.DrawLine(transform.position, hive.gameObject.transform.position, Color.green);

            MoveTowardsTarget(target.transform.position);

            //Relay information to hive, then reset bee
            if (Vector3.Distance(transform.position, hive.transform.position) < targetRadius)
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
        //Go through all active humans
        for (int i = 0; i < hive.activeHumans.Count; i++)
        {
            //Check if human in detect range
            if (Vector3.Distance(transform.position, hive.activeHumans[i].transform.position) <= detectRadius)
            {
                //Find best human 
                if (hive.activeHumans[i].GetComponent<HumanController>().resource > newHumanResource)
                {
                    newHuman = hive.activeHumans[i];
                }
            }
        }

        if (hive.detectedHumans.Contains(newHuman))
        {
            return null;
        }
        else
        {
            return newHuman;
        }
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
        adjRotSpeed = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);
        rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime);
    }
    #endregion
}
