using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Enemy
{
    //Stats
    public float collectionTime = 6.0f;
    public int capacity = 5;
    public float damage = 20;

    private float attackOrFlee;
    [HideInInspector]
    public float scoutFit = 0;
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

    [Header("General")]
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

    public enum BeeBehaviours
    { 
        Idle,
        Scout, 
        Forage, 
        Attack,
        Flee
    }
    public BeeBehaviours beeBehaviour;

    private void Start()
    {
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager = FindObjectOfType<GameManager>();

        rb = GetComponent<Rigidbody>();
        idlePosition = hive.transform.position;

        //Drone idly roam around mothership
        beeBehaviour = BeeBehaviours.Idle;
        target = hive.gameObject;

        //Queue scout for when game starts
        GameManager.live.gameStartDel += SetToScout;
    }

    private void Update()
    {
        FSMController();
    }

    public void Initialise(Hive hive, GameObject initTarget)
    {
        this.hive = hive;
        this.target = initTarget;
    }

    private void SetToScout()
    {
        beeBehaviour = BeeBehaviours.Scout;
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
            //Consume fuel
            //currentFuel -= Time.deltaTime;

            //Lerp Towards target
            targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            adjRotSpeed = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);

            //if (currentFuel <= 0)
            //{
            //    rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime / 3);
            //}
            //else
            //{
            //    rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime);
            //}
            rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime);
        }
    }

    private void OrbitTarget(Vector3 targetPos)
    {
        //Rotate and move towards target if out of range
        //if (Vector3.Distance(targetPos, transform.position) > targetRadius)
        //{
            //Consume fuel
            //currentFuel -= Time.deltaTime;

            //Lerp Towards target
            targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            adjRotSpeed = Mathf.Min(rotationSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, adjRotSpeed);

            //if (currentFuel <= 0)
            //{
            //    rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime / 3);
            //}
            //else
            //{
            //    rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime);
            //}
            rb.AddRelativeForce(Vector3.forward * speed * 20 * Time.deltaTime);
        //}
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

    private void Flee()
    {
        throw new NotImplementedException();
    }

    private void Attack()
    {
        throw new NotImplementedException();
    }

    private void Forage()
    {
        throw new NotImplementedException();
    }

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

    private void Idle()
    {
        OrbitTarget(hive.transform.position);
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
}
