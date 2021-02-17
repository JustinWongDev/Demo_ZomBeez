using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveResources : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private GameObject modelParent = null;
    [SerializeField] private  Material empty = null;
    [SerializeField] private  Material full = null;
    
    [SerializeField] private float forageTime = 10.0f;
    [SerializeField] private float jellyTime = 10.0f;
    [SerializeField] private int resourceToJellyRatio = 10;
    [SerializeField] private int initJelly = 3;
    
    private float forageTimer = 0;
    private int collectedBrains = 0;
    private int jellyAmount = 0;
    private float jellyTimer = 0;

    private void Start()
    {
        jellyAmount = initJelly;
    }
    
    private void Update()
    {
        ProduceJelly();

        DisplayJelly();
    }
    
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
        if (collectedBrains >= resourceToJellyRatio)
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

    public void GainBrains(int val)
    {
        collectedBrains += val;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>() && jellyAmount > 0)
        {
            LoseJelly();
            other.GetComponent<HumanController>().Settings.SetHasJelly(true);
        }
    }
}
