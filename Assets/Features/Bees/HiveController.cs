using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class HiveController : MonoBehaviour
{
    #region Static Singleton
    public static HiveController live;
    private void Awake()
    {
        live = this;
    }
    #endregion

    [Header("Bees")]
    [SerializeField] private int initCount = 5;
    [SerializeField] private int maxScoutCount = 5;
    [SerializeField] private int maxAttackerCount = 2;
    private List<BeeBrain> beeBrains = new List<BeeBrain>();
    private List<BeeController> workers = new List<BeeController>();
    private List<BeeController> scouts = new List<BeeController>();
    private List<BeeController> attackers = new List<BeeController>();

    private List<HumanController> activeHumans = new List<HumanController>();
    private List<HumanController> detectedHumans = new List<HumanController>();
    private HiveResources resources;

    private void Start()
    {
        resources = GetComponent<HiveResources>();
        
        SpawnBees();
    }

    private void Update()
    {
        SetScouts();

        PeriodicHumanSort();
    }

    public void RemoveHuman(HumanController deadHuman)
    {
        activeHumans.Remove(deadHuman);
        detectedHumans.Remove(deadHuman);
    }

    public void AddActiveHuman(HumanController newHuman) => activeHumans.Add(newHuman);
    

    public void AddDetectedHuman(HumanController newHuman) => detectedHumans.Add(newHuman);

    public List<HumanController> DetectedHumans => detectedHumans;
    public List<HumanController> ActiveHumans => activeHumans;

    void PeriodicHumanSort()
    {
        //Sort humans in decreasing resource amount
        detectedHumans.Sort(delegate (HumanController a, HumanController b)
        {
            return (b.Settings.Brains).CompareTo(a.Settings.Brains);
        });

        detectedHumans = detectedHumans.Distinct().ToList();
        //forageTimer = Time.time + forageTime;
        
        //Determine best-resource human
        if (detectedHumans.Count <= 0) // && Time.time > forageTimer
            return;

        if (detectedHumans.Count < 3)
        {
            InstructIdles(detectedHumans.Count);
        }
        else
        {
            InstructIdles(3);
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

    void InstructIdles(int humanTargetIndex)
    {
        SortHarvestTargets();
        
        List<GameObject> listTargets = new List<GameObject>();
        for (int i = 0; i < humanTargetIndex; i++)
        {
            listTargets.Add(detectedHumans[i].gameObject);
        }
        
        int segmentInt = Mathf.FloorToInt(workers.Count / humanTargetIndex);
        int numToInstruct = 0;

        for (int i = 0; i < humanTargetIndex; i++)
        {
            numToInstruct += segmentInt;

            for (int j = 0; j < numToInstruct; j++)
            {
                if (workers[j].CurrentAIState.GetType() == typeof(BeeIdle))
                {
                    workers[j].Target = listTargets[i];
                    listTargets[i].GetComponent<HumanController>().OnHumanDeath += workers[j].RemoveTarget;
                    workers[j].CurrentAIState = new BeeForage(workers[j]);
                    //workers[j].humanEmpty = false;
                }
            }
        }
    }

    private void SetScouts()
    {
        //Initialise scouts
        if (scouts.Count < maxScoutCount && GameManager.live.hasGameStarted())
        { 
            //Sort bees in order of scoutFit
            List<BeeController> potentialScouts = new List<BeeController>();
            workers.Sort(delegate (BeeController a, BeeController b)
            {
                return (b.scoutFit.CompareTo(a.scoutFit));
            });

            scouts.Add(workers[0]);
            workers.Remove(workers[0]);
            
            scouts[scouts.Count - 1].CurrentAIState = new BeeScout(scouts[scouts.Count - 1]);
        }    
    }

    void SpawnBees()
    {
        for (int i = 0; i < initCount; i++)
        {
            GameObject bee = ObjPool.SharedInstance.GetPooledObj();
            // if (BeeController == null)
            //     return;

            
            bee.transform.position = this.transform.position;
            bee.transform.rotation = this.transform.rotation;
            bee.SetActive(true);
        
            bee.GetComponent<BeeController>().Initialise(this, resources, gameObject);
            bee.GetComponent<BeeResources>().Initialise(this, bee.GetComponent<BeeController>());
            workers.Add(bee.GetComponent<BeeController>());   
        }
    }
}
