using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool : MonoBehaviour
{
    [SerializeField] private GameObject objectToPool = null;
    [SerializeField] private int amountToPool = 0;
    
    public static ObjPool SharedInstance;
    public List<GameObject> pooledObjs;
    
    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjs = new List<GameObject>();
        GameObject temp;
        for (int i = 0; i < amountToPool; i++)
        {
            temp = Instantiate(objectToPool);
            temp.SetActive(false);
            pooledObjs.Add(temp);
        }
    }

    public GameObject GetPooledObj()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjs[i].activeInHierarchy)
                return pooledObjs[i];
        }

        return null;
    }
}
