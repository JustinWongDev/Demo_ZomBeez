using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForageSite : MonoBehaviour
{
    [Header("Variables")]
    public float timeToSpawn = 10.0f;
    private float spawnTimer = 0;

    [Header("Items")] 
    public ItemSO currentItem = null;
    public ItemSO[] allItems;

    [Header("Model")] 
    public MeshRenderer mesh;
    public Material empty;
    public Material full;
    
    //periodically spawn random item
    //items:
    //  weapon: inc damage
    //  salve:  heal
    //  smoke   bomb: bees flee
    //  helmet: stop bee foraging
    //  boots:  speed
    //  vest:   armour 

    private void Start()
    {
        mesh.material = empty;
    }

    private void Update()
    {
        Timer();
    }
    
    private void SetAvailableItem()
    {
        currentItem = allItems[UnityEngine.Random.Range(0, allItems.Length - 1)];
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //When in proximity, give human item
        if (other.gameObject.GetComponent<HumanController>())
        {
            other.gameObject.GetComponent<HumanController>().Inventory.items.Add(currentItem);
            currentItem = null;
            mesh.material = empty;
            spawnTimer = Time.time + timeToSpawn;
        }
    }

    private void Timer()
    {
        if (currentItem || !GameManager.live.gameStart)
            return;

        if (Time.time > spawnTimer)
        {
            SetAvailableItem();
            
            spawnTimer = Time.time + timeToSpawn;
            mesh.material = full;
        }
    }
    
}
