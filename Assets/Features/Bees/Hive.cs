using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class Hive : MonoBehaviour
{
    #region Static Singleton
    public static Hive live;
    private void Awake()
    {
        live = this;
    }
    #endregion

    [Header("Models")]
    [SerializeField] private GameObject modelParent = null;
    [SerializeField] private  Material empty = null;
    [SerializeField] private  Material full = null;
    
    [Header("Bees")]
    [SerializeField] private int initCount = 5;
    [SerializeField] private int maxScoutCount = 5;
    [SerializeField] private int maxAttackerCount = 2;
    private List<Worker> workers = new List<Worker>();
    private List<Worker> scouts = new List<Worker>();
    private List<Worker> attackers = new List<Worker>();

    [Header("Brains")]
    public float forageTime = 10.0f;
    private float forageTimer;
    public int collectedResource = 0;
    public int initJelly = 3;
    private int jellyAmount = 0;
    public float jellyTime = 10.0f;
    public int resourceToJellyRatio = 10;
    private float jellyTimer = 0;

    private List<HumanController> activeHumans = new List<HumanController>();
    private List<HumanController> detectedHumans = new List<HumanController>();

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

    public void AddDetectedHuman(HumanController newHuman)
    {
        detectedHumans.Add(newHuman);
    }

    public List<HumanController> DetectedHumans => detectedHumans;
    public List<HumanController> ActiveHumans => activeHumans;

    #region Brains Foraging Management
    void PeriodicHumanSort()
    {
        //Determine best-resource human
        if (detectedHumans.Count > 0) // && Time.time > forageTimer
        {
            //Sort humans in decreasing resource amount
            detectedHumans.Sort(delegate (HumanController a, HumanController b)
            {
                return (b.Settings.Brains).CompareTo(a.Settings.Brains);
            });

            detectedHumans = detectedHumans.Distinct().ToList();
            //forageTimer = Time.time + forageTime;
            InstructIdles();
        }
    }

    void SortHarvestTargets()
    {
        if (detectedHumans.Count == 0)
            return;
        
        foreach (HumanController human in detectedHumans)
        {
            if (!human.enabled)
                detectedHumans.Remove(human);
        }

        detectedHumans.Sort((h1,h2)=>h1.Settings.Brains.CompareTo(h2.Settings.Brains));
    }

    void InstructIdles()
    {
        SortHarvestTargets();
        
        if(detectedHumans[0].Settings.Brains < 3) return;
        
        foreach (Worker bee in workers)
        {
            if (bee.beeBehaviour == Worker.BeeBehaviours.Idle)
            {
                bee.beeBehaviour = Worker.BeeBehaviours.Forage;
                bee.target = detectedHumans[0]?.gameObject;
                bee.humanEmpty = false;
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

    public void LoseJelly()
    {
        if(jellyAmount > 0)
        {
            jellyAmount--;
        }
    }

    private void SetScouts()
    {
        //Initialise scouts
        if (scouts.Count < maxScoutCount && GameManager.live.hasGameStarted())
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
            GameObject bee = ObjPool.SharedInstance.GetPooledObj();
            // if (bee == null)
            //     return;

            
            bee.transform.position = this.transform.position;
            bee.transform.rotation = this.transform.rotation;
            bee.SetActive(true);
        
            bee.GetComponent<Worker>().Initialise(this, gameObject);
            workers.Add(bee.GetComponent<Worker>());   
        }
    }
    //INSTANTIATION
        // for (int i = 0; i < initCount; i++)
        // {
        //     //USE OBJECT POOLING
        //     GameObject go = Instantiate(beePrefab);
        //     go.transform.position = transform.position;
        //     go.GetComponent<Worker>().Initialise(this, gameObject);
        //     workers.Add(go.GetComponent<Worker>());
        // }

        private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>() && jellyAmount > 0)
        {
            LoseJelly();
            other.GetComponent<HumanController>().Settings.SetHasJelly(true);
            // GameManager.live.jellyObtained++;
            // GameObject o;
            // (o = other.gameObject).GetComponent<HumanMove>().TakeDamage(1000.0f);
            // Destroy(o, 2.0f);
        }
    }
}
