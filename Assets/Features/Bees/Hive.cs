using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{
    [Header("Variables")]
    public int initCount = 5;

    [Header("Models")]
    public GameObject beePrefab;

    private void Start()
    {
        GameManager.live.gameStartDel += SpawnBees;
    }

    void SpawnBees()
    {
        for (int i = 0; i < initCount; i++)
        {
            //USE OBJECT POOLING
            GameObject go = Instantiate(beePrefab);
            go.transform.position = this.transform.position;
            go.GetComponent<Worker>().Initialise(FindObjectOfType<Hive>().gameObject, go.GetComponent<Worker>().hive.gameObject);

        }
    }
}
