using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Enemy
{
    //Drone Utility Variable
    private float attackOrFlee;
    [HideInInspector]
    public float scoutFit = 0;
    private const float MAX_SPEED = 200;
    private const int MAX_COLLECTION = 6;
    private const int MAX_CAPACITY = 20;
    private const int MAX_DAMAGE = 100;
    private const float MAX_HEALTH = 300;
    private const float MAX_DETECTION = 400;
    private const float MAX_TOTAL_FUEL = 200;

    //Movement & Rotation Variables
    [Header("Movement")]
    public float speed = 50.0f;
    public float rotationSpeed = 5.0f;
    private float adjRotSpeed;
    private Quaternion targetRotation;
    public GameObject target;
    public float targetRadius = 200f;

    //Boid Steering/Flocking Variables
    [Header("Boiding")]
    public float separationDistance = 25.0f;
    public float cohesionDistance = 50.0f;
    public float separationStrength = 250.0f;
    public float cohesionStrength = 25.0f;
    private Vector3 cohesionPos = new Vector3(0f, 0f, 0f);
    private int boidIndex = 0;

    //Drone Behaviour Variables
    [Header("General")]
    public GameObject hive;
    public Vector3 idlePosition;
    public float currentFuel;
    public float totalFuel;

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
        target = hive;
    }

    private void Update()
    {
        //MoveTowardsTarget(hive.transform.position);

        FSMController();
    }

    public void Initialise(GameObject hive, GameObject initTarget)
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
        throw new NotImplementedException();
    }

    private void Idle()
    {
        OrbitTarget(hive.transform.position);
    }
}
