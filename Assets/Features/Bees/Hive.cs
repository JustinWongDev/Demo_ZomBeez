using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hive : MonoBehaviour
{
    [Header("Bees")]
    public int initCount = 5;
    public int maxScoutCount = 5;
    public int maxAttackerCount = 2;

    [Header("Resource")]
    public float forageTime = 10.0f;
    private float forageTimer;
    public int collectedResource = 0;

    [Header("Models")]
    public GameObject beePrefab;

    [Header("Bees")]
    public List<Worker> workers = new List<Worker>();
    public List<Worker> scouts = new List<Worker>();
    public List<Worker> attackers = new List<Worker>();

    [Header("Humans")]
    public List<HumanController> activeHumans = new List<HumanController>();
    public List<HumanController> detectedHumans = new List<HumanController>();

    private void Start()
    {
        SpawnBees();
    }

    private void Update()
    {
        SetScouts();

        PeriodicHumanSort();
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
                return (b.resource).CompareTo(a.resource);
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
            if (detectedHumans[0].resource > 3)
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
}
