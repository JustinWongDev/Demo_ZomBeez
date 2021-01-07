using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    public GameObject[] graphNodes;

    private void Start()
    {
        IndexNodes();
    }

    void IndexNodes()
    { 
        for(int i = 0; i < graphNodes.Length; i++)
        {
            graphNodes[i].GetComponent<LinkedNode>().index = i;    
        }
    }
}
