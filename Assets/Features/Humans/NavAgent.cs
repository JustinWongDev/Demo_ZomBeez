using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : MonoBehaviour
{
    [Header("Nav Variables")]
    public WaypointGraph graphNodes;
    public List<int> openList = new List<int>();
    public List<int> closedList = new List<int>();
    public List<int> currentPath = new List<int>();
    public List<int> greedyPaintList = new List<int>();
    public int currentPathIndex = 0;
    public int currentNodeIndex = 0;

    public Dictionary<int, int> cameFrom = new Dictionary<int, int>();

    private void Start()
    {
        //Find waypoint graph
        graphNodes = GameObject.FindObjectOfType<WaypointGraph>().GetComponent<WaypointGraph>();

        //Add initial node index 
        //currentPath.Add(currentNodeIndex);
    }

    /// <summary>
    /// AStar search between two nodes
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<int> AStarSearch(int start, int goal)
    {
        //Clear all lists
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        //Add root node
        openList.Add(start);

        //Setup heuristics
        float gScore = 0;
        float fScore = gScore + Heuristics(start, goal);

        //Start algorithm
        while (openList.Count > 0)
        {
            //Find openList node with lowest fScore 
            int currentNode = bestOpenListFScore(start, goal);

            //If goal reached, reconstruct path and return
            if (currentNode == goal)
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            //Move currentNode to closedList
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //For each linked node to the currentNode...
            for (int i = 0; i < graphNodes.graphNodes[currentNode].GetComponent<LinkedNode>().linkedNodeIndexes.Length; i++)
            {
                int thisNeighbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNode>().linkedNodeIndexes[i];

                //Ignore closed, neighbour nodes
                if(!closedList.Contains(thisNeighbourNode))
                {
                    //Dist between current and neighbour nodes
                    float tentativeGScore = Heuristics(start, currentNode) + Heuristics(currentNode, thisNeighbourNode);

                    //Check if openList or new GScore is more sensible
                    if (!openList.Contains(thisNeighbourNode) || tentativeGScore < gScore)
                    {
                        openList.Add(thisNeighbourNode);
                    }

                    //Add to dictionary, this neighbour came from this parent node
                    if (!cameFrom.ContainsKey(thisNeighbourNode))
                    {
                        cameFrom.Add(thisNeighbourNode, currentNode);
                    }

                    //Adjust heuristics
                    gScore = tentativeGScore;
                    fScore = Heuristics(start, thisNeighbourNode) + Heuristics(thisNeighbourNode, goal);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Calculate distance between two nodes
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public float Heuristics(int a, int b)
    {
        return Vector3.Distance(graphNodes.graphNodes[a].transform.position, graphNodes.graphNodes[b].transform.position);
    }

    /// <summary>
    /// Find open node with best FScore
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public int bestOpenListFScore(int start, int goal)
    {
        int bestIndex = 0;

        for (int i = 0; i < openList.Count; i++)
        {
            if ((Heuristics(openList[i], start) + Heuristics(openList[i], goal) < (Heuristics(openList[bestIndex], start) + Heuristics(openList[bestIndex], goal))))
            {
                bestIndex = i;
            }
        }

        int bestNode = openList[bestIndex];
        return bestNode;
    }

    /// <summary>
    /// When goal node reached, reconstruct path
    /// </summary>
    /// <param name="cameFrom"></param>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    private List<int> ReconstructPath(Dictionary<int, int> cameFrom, int currentNode)
    {
        List<int> finalPath = new List<int>();

        finalPath.Add(currentNode);

        while (cameFrom.ContainsKey(currentNode))
        {
            currentNode = cameFrom[currentNode];
            finalPath.Add(currentNode);
        }

        finalPath.Reverse();

        return finalPath;
    }
}
