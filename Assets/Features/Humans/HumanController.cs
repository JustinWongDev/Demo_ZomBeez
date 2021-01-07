using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : NavAgent
{
    [Header("Movement")]
    public float moveSpeed = 10.0f;
    public float minDistance = 0.01f;

    private Vector3 mousePos;

    private void Update()
    {
        //Left click: A* search
        if(Input.GetMouseButtonDown(1))
        {
            currentPath = AStarSearch(currentPath[currentPathIndex], findClosestWaypoint());
            currentPathIndex = 0;
        }

        //Move player
        if (currentPath.Count > 0)
        {
            //Move towards next node
            transform.position = Vector3.MoveTowards(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

            //Inc path index
            if(Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance)
            {
                if (currentPathIndex < currentPath.Count - 1)
                {
                    currentPathIndex++;
                }
            }

            //Store current node index
            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNode>().index;
        }
    }

    private int findClosestWaypoint()
    {
        //Convert mouse coordinates to world position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            mousePos = hit.point;
        }

        Debug.DrawLine(Camera.main.transform.position, mousePos, Color.red);

        float distance = Mathf.Infinity;
        int closestWaypoint = 0;

        //Find closest node to mouse position
        for (int i = 0; i < graphNodes.graphNodes.Length; i++)
        {
            if (Vector3.Distance(mousePos, graphNodes.graphNodes[i].transform.position) <= distance)
            {
                distance = Vector3.Distance(mousePos, graphNodes.graphNodes[i].transform.position);
                closestWaypoint = i;
            }
        }

        return closestWaypoint;
    }
}
