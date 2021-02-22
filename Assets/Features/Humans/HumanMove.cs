using UnityEngine;

public enum Pathfinding { None, Astar };

public class HumanMove : NavAgent
{
    private Pathfinding _currentPathing = Pathfinding.None;
    private Transform _destination = null;
    private HumanController _controller = null;
    private HumanAnimController _animController = null;

    public Pathfinding CurrentPathing => _currentPathing;

    private void Start()
    {
        SetGraphNodes();

        _destination = HiveLocation();
        
        _controller = GetComponent<HumanController>();
        _animController = GetComponent<HumanAnimController>();
    }

    private void Update()
    {
        Pathing();

        Move();
    }
    
    public void SetDestination(Transform newTarget)
    {
        _destination = newTarget;
    }

    public void SetPathFinding(Pathfinding val) => _currentPathing = val;
    
    void Pathing()
    {
        //Auto random pathing
        switch (CurrentPathing)
        {
            case Pathfinding.None:
                break;
            case Pathfinding.Astar:
                if(AtDestination())
                {
                    currentPath = AStarSearch(currentPath[currentPathIndex], FindClosestWaypoint(_destination));
                    currentPathIndex = 0;
                }
                break;
        }
    }

    public int FindClosestWaypoint(Transform target)
    {
        if (!target)
        {
            Debug.Log("No target transform");
            return -1;
        }

        //Find which waypoint is closest to target
        float distance = Mathf.Infinity;
        int closestWaypoint = -1;

        //Find closest node to target
        for (int i = 0; i < graphNodes.graphNodes.Length; i++)
        {
            if (Vector3.Distance(target.transform.position, graphNodes.graphNodes[i].transform.position) <= distance)
            {
                distance = Vector3.Distance(target.transform.position, graphNodes.graphNodes[i].transform.position);
                closestWaypoint = i;
            }
        }

        return closestWaypoint;
    }
    
    public Transform HiveLocation()
    {
        return FindObjectOfType<HiveController>().transform;
    }

    public Transform DepotLocation()
    {
        return FindObjectOfType<Depot>().transform;
    }

    public Transform ClosestActiveCache()
    {
        //Store all forage sites
        ForageSite [] allSites = FindObjectsOfType<ForageSite>();

        //Find which active, forage site is closest 
        ForageSite closestSite = null;
        float closestDist = Mathf.Infinity;
         
        foreach (ForageSite site in allSites)
        {
            float tempDist = Vector3.Distance(transform.position, site.transform.position);
             
            if (tempDist < closestDist && 
                site.currentItem)
            {
                closestSite = site;
                closestDist = tempDist;
            }
        }

        return closestSite.transform;
    }
    
    public bool AtDestination()
    {
        if (currentPath.Count > 0)
        {
            //bool value = currentNodeIndex == currentPath[currentPath.Count - 1];
            return currentNodeIndex == currentPath[currentPath.Count - 1];
        }
        else
        {
            return false;
        }
    }

    public bool NearDestination(float distance)
    {
        return Vector3.Distance(transform.position, _destination.transform.position) < distance;
    }

    void Move()
    {
        if (AbleToMove())
        {
            var targetPos = graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position;

            //Accelerate
            if (_controller.Settings.GetCurrentSpeed() < _controller.Settings.MaxSpeed)
                _controller.Settings.AddCurrentSpeed(_controller.Settings.MaxSpeed * Time.deltaTime * _controller.Settings.Acceleration);
            //Update anim variables
            _animController.Float_VelocityZ(_controller.Settings.GetCurrentSpeed());

            //Move towards next node
            Transform currentPos = this.transform;
            currentPos.position = Vector3.MoveTowards(currentPos.position, targetPos, _controller.Settings.GetCurrentSpeed() * Time.deltaTime);

            //Face direction
            FaceDirection(targetPos, currentPos);

            //Inc path index
            if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= _controller.Settings.MinDistance)
            {
                if (currentPathIndex < currentPath.Count - 1)
                {
                    currentPathIndex++;
                }
            }

            //Store current node index
            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNode>().index;
        }
    
    
        //Decelerate
        if (_controller.Settings.GetCurrentSpeed() > 0 && CurrentPathing == Pathfinding.None)
            _controller.Settings.AddCurrentSpeed(-_controller.Settings.MaxSpeed * Time.deltaTime * _controller.Settings.Deceleration);
        //Update anim variables
        _animController.Float_VelocityZ(_controller.Settings.GetCurrentSpeed());
    }
    private bool AbleToMove()
    {
        if (currentPath.Count > 0 && !_controller.Settings.GetIsDead() && !GetComponent<Droppable>())
            return true;
        return false;
    }
    private void FaceDirection(Vector3 targetPos, Transform currentPos)
    {
        var targetDir = targetPos - currentPos.position;
        var step = _controller.Settings.GetCurrentSpeed() * Time.deltaTime;
        var newDir = Vector3.RotateTowards(currentPos.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    public void Moveable(bool val)
    {
        SetPathFinding(val ? Pathfinding.None : Pathfinding.Astar);
    }
}

