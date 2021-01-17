using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hive : MonoBehaviour
{
    #region Static Singleton
    public static Hive live;
    private void Awake()
    {
        live = this;
    }
    #endregion

    [Header("Bees")]
    public int initCount = 5;
    public int maxScoutCount = 5;
    public int maxAttackerCount = 2;

    [Header("Resource")]
    public float forageTime = 10.0f;
    private float forageTimer;
    public int collectedResource = 0;
    public int initJelly = 3;
    private int jellyAmount = 0;
    public float jellyTime = 10.0f;
    public int resourceToJellyRatio = 10;
    private float jellyTimer = 0;

    [Header("Models")]
    public GameObject beePrefab;
    public GameObject modelParent;
    public Material empty;
    public Material full;

        [Header("Bees")]
    public List<Worker> workers = new List<Worker>();
    public List<Worker> scouts = new List<Worker>();
    public List<Worker> attackers = new List<Worker>();

    [Header("Humans")]
    public List<HumanController> activeHumans = new List<HumanController>();
    public List<HumanController> detectedHumans = new List<HumanController>();

    private void Start()
    {
        jellyAmount = initJelly;
        
        SpawnBees();
    }

    private void Update()
    {
        SetScouts();

        PeriodicHumanSort();
        
        ProduceJelly();

        DisplayJelly();
    }

    public void AddActiveHuman(HumanController newHuman)
    {
        activeHumans.Add(newHuman);
    }

    #region Resource Foraging Management
    void PeriodicHumanSort()
    {
        //Determine best-resource human
        if (detectedHumans.Count > 0) // && Time.time > forageTimer
        {
            //Sort humans in decreasing resource amount
            detectedHumans.Sort(delegate (HumanController a, HumanController b)
            {
                return (b.CurrentResource).CompareTo(a.CurrentResource);
            });

            detectedHumans = detectedHumans.Distinct().ToList();
            forageTimer = Time.time + forageTime;
            InstructIdles();
        }
    }
    void InstructIdles()
    {
        if (detectedHumans.Count > 0)
        {
            if (detectedHumans[0].CurrentResource > 3)
            {
                //Direct all idle workers
                foreach (Worker worker in workers)
                {
                    if (worker.beeBehaviour == Worker.BeeBehaviours.Idle)
                    {
                        worker.beeBehaviour = Worker.BeeBehaviours.Forage;
                        worker.target = detectedHumans[0].gameObject;
                        worker.humanEmpty = false;
                    }
                }
            }
        }
    }
    #endregion
    
    void DisplayJelly()
    {
        Transform[] segments = modelParent.GetComponentsInChildren<Transform>();
        
        if (jellyAmount > 0)
        {
            foreach (Transform t in segments)
            {
                if(t.gameObject.GetComponent<MeshRenderer>())
                    t.gameObject.GetComponent<MeshRenderer>().material = full;
            }
        }
        else
        {
            foreach (Transform t in segments)
            {                
                if(t.gameObject.GetComponent<MeshRenderer>())
                    t.gameObject.GetComponent<MeshRenderer>().material = empty;
            }
        }
    }

    void ProduceJelly()
    {
        if (collectedResource >= resourceToJellyRatio)
        {
            if (jellyTimer >= jellyTime)
            {
                jellyAmount++;
                jellyTimer = 0;
            }
            else
            {
                jellyTimer += Time.deltaTime;
            }
        }
    }

    void LoseJelly()
    {
        if(jellyAmount > 0)
        {
            jellyAmount--;
        }
    }

    private void SetScouts()
    {
        //Initialise scouts
        if (scouts.Count < maxScoutCount && GameManager.live.gameStart)
        { 
            //Sort bees in order of scoutFit
            List<Worker> potentialScouts = new List<Worker>();
            workers.Sort(delegate (Worker a, Worker b)
            {
                return (b.scoutFit.CompareTo(a.scoutFit));
            });

            scouts.Add(workers[0]);
            workers.Remove(workers[0]);

            scouts[scouts.Count - 1].beeBehaviour = Worker.BeeBehaviours.Scout;
        }    
    }

    void SpawnBees()
    {
        for (int i = 0; i < initCount; i++)
        {
            //USE OBJECT POOLING
            GameObject go = Instantiate(beePrefab);
            go.transform.position = transform.position;
            go.GetComponent<Worker>().Initialise(this, gameObject);
            workers.Add(go.GetComponent<Worker>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>() && jellyAmount > 0)
        {
            LoseJelly();
            GameManager.live.jellyObtained++;
            GameObject o;
            (o = other.gameObject).GetComponent<HumanController>().TakeDamage(1000.0f);
            Destroy(o, 2.0f);
        }
    }
}
