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
    private List<HumanController> humansToAttack = new List<HumanController>();
    private HiveResources hResources;

    public void AddActiveHuman(HumanController newHuman) => activeHumans.Add(newHuman);
    public void AddDetectedHuman(HumanController newHuman) => detectedHumans.Add(newHuman);
    public List<HumanController> DetectedHumans => detectedHumans;
    public List<HumanController> ActiveHumans => activeHumans;
    public List<HumanController> HumansToAttack => humansToAttack;
    
    private void Start()
    {
        hResources = GetComponent<HiveResources>();

        SpawnBees();
    }

    private void Update()
    {
        SetScouts();

        PeriodicHumanSort();
        
        SortTargetsAndSetAttackers();
    }

    public HumanController ClosestDetectedHuman(Vector3 beePos)
    {
        float lowestDist = Mathf.Infinity;
        HumanController closestHuman = null;

        for (int i = 0; i < detectedHumans.Count; i++)
        {
            float dist = Vector3.Distance(beePos, detectedHumans[i].transform.position);
            
            if (dist < lowestDist)
            {
                lowestDist = dist;
                closestHuman = detectedHumans[i];
            }
        }
        return closestHuman;
    }

    public void RemoveHuman(HumanController deadHuman)
    {
        activeHumans.Remove(deadHuman);
        detectedHumans.Remove(deadHuman);
    }

    void PeriodicHumanSort()
    {
        //Sort humans in decreasing resource amount
        detectedHumans.Sort(delegate (HumanController a, HumanController b)
        {
            return (b.Settings.Brains).CompareTo(a.Settings.Brains);
        });

        detectedHumans = detectedHumans.Distinct().ToList();

        if (detectedHumans.Count <= 0) // && Time.time > forageTimer
            return;

        if (detectedHumans.Count < 3) InstructIdles(detectedHumans.Count);
        else InstructIdles(3);
    }

    void SortHarvestTargets()
    {
        if (detectedHumans.Count == 0) return;
        
        foreach (HumanController human in detectedHumans)
        {
            if (!human.enabled) detectedHumans.Remove(human);
        }

        detectedHumans.Sort((h1,h2)=>h1.Settings.Brains.CompareTo(h2.Settings.Brains));
    }

    void InstructIdles(int humanTargetIndex)
    {
        SortHarvestTargets();
        
        List<GameObject> listTargets = new List<GameObject>();
        for (int i = 0; i < humanTargetIndex; i++)
            listTargets.Add(detectedHumans[i].gameObject);

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
        if (scouts.Count < maxScoutCount && GameManager.live.hasGameStarted())
        {
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
    
    private void SortTargetsAndSetAttackers()
    {
        if (humansToAttack.Count <= 0)
            return;
        
        RemoveNullValues();
        
        humansToAttack.Sort((p1,p2)=>p1.GetComponent<HumanMove>().DistanceToDepot().
            CompareTo(p2.GetComponent<HumanMove>().DistanceToDepot()));
        
        if (attackers.Count < maxAttackerCount && GameManager.live.hasGameStarted())
        {
            List<BeeController> potentialAttackers = new List<BeeController>();
            workers.Sort(delegate (BeeController a, BeeController b)
            {
                return (b.attackFit.CompareTo(a.attackFit));
            });

            attackers.Add(workers[0]);
            workers.Remove(workers[0]);
            
            attackers[attackers.Count - 1].CurrentAIState = new BeeAttack(attackers[attackers.Count - 1]);
        } 
    }

    private void RemoveNullValues()
    {
        for(var i = humansToAttack.Count - 1; i > -1; i--)
        {
            if (humansToAttack[i] == null)
                humansToAttack.RemoveAt(i);
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
        
            bee.GetComponent<BeeController>().Initialise(this, hResources, gameObject);
            bee.GetComponent<BeeResources>().Initialise(this, bee.GetComponent<BeeController>());
            workers.Add(bee.GetComponent<BeeController>());   
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HumanController>())
        {
            humansToAttack.Add((other.GetComponent<HumanController>()));
        }
    }
}
