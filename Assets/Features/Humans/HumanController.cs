using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : NavAgent
{
    [Header("Stats")]
    public int resource = 20;

    [Header("Movement")]
    public Pathfinding currentPathing = Pathfinding.none;
    public enum Pathfinding { none, astar };
    public float maxSpeed = 10.0f;
    public float minDistance = 0.01f;
    public float acceleration = 5.0f;
    public float deceleration = 25.0f;
    private float currentSpeed = 0;

    private Vector3 mousePos;

    public Animator animator;

    private void Update()
    {
        //Left click: A* search
        //if(Input.GetMouseButtonDown(1))
        //{
        //    currentPath = AStarSearch(currentPath[currentPathIndex], findClosestWaypoint());
        //    currentPathIndex = 0;
        //}

        //Auto random pathing
        switch (currentPathing)
        {
            case Pathfinding.none:
                break;
            case Pathfinding.astar:
                if(currentNodeIndex == currentPath[currentPath.Count-1])
                {
                    currentPath = AStarSearch(currentPath[currentPathIndex], UnityEngine.Random.Range(0, graphNodes.graphNodes.Length));
                    currentPathIndex = 0;
                }
                break;
        }

        MovePlayer();
    }

    void MovePlayer()
    {
        //Move player
        if (currentPath.Count > 0)
        {
            Vector3 targetPos = graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position;

            //Set speed
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += maxSpeed * Time.deltaTime * acceleration;
            }

            //Move towards next node
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

            //Face direction
            Vector3 targetDir = targetPos - transform.position;
            float step = currentSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            //Inc path index
            if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance)
            {
                if (currentPathIndex < currentPath.Count - 1)
                {
                    currentPathIndex++;
                }
            }

            //Store current node index
            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNode>().index;
        }
        
        
        //Set speed
        if (currentSpeed > 0 && currentPathing == Pathfinding.none)
        {
            currentSpeed -= maxSpeed * Time.deltaTime * deceleration;
        }

        //Update anim variables
        animator.SetFloat("Velocity Z", currentSpeed);
        
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
