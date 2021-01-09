using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{
    [Header("Variables")]
    public int initCount = 5;

    [Header("Models")]
    public GameObject beePrefab;

    [Header("Humans")]
    //public HumanController[] activeHumans;
    //public HumanController[] foundHumans;

    public List<HumanController> activeHumans = new List<HumanController>();
    public List<HumanController> detectedHumans = new List<HumanController>();

    private void Start()
    {
        SpawnBees();
    }

    void SpawnBees()
    {
        for (int i = 0; i < initCount; i++)
        {
            //USE OBJECT POOLING
            GameObject go = Instantiate(beePrefab);
            go.transform.position = transform.position;
            go.GetComponent<Worker>().Initialise(this, gameObject);

        }
    }
}
