using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : NavAgent
{
    [Header("Stats")]
    public float currentHealth;
    public float maxHealth = MAX_HEALTH;
    public int resource = MAX_RESOURCE;

    private const float MAX_HEALTH = 100.0f;
    private const int MAX_RESOURCE = 20;
    private const float MAX_SPEED = 20.0f;

    private bool isDead = false;

    [Header("Movement")]
    public Pathfinding currentPathing = Pathfinding.none;
    public enum Pathfinding { none, astar };
    public float maxSpeed = MAX_SPEED;
    public float minDistance = 0.01f;
    public float acceleration = 5.0f;
    public float deceleration = 25.0f;
    private float currentSpeed = 0;

    [Header("Drop")]
    public float dropHeight = 20.0f;
    private Vector3 mousePos;
    private Vector3 lastPos;
    private bool isDropped = false;
    private bool isGrounded = false;

    [Header("Animations")]
    public Animator animator;


    private void Update()
    {
        //Hold and drop 
        HoldAndDrop();

        //Start moving
        CheckToStartMoving();

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

    private void HoldAndDrop()
    {
        if (!isDropped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDropped = true;
                currentPath.Clear();
                currentNodeIndex = findClosestWaypoint();
                currentPath.Add(currentNodeIndex);
            }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //If ray hits...
            if (Physics.Raycast(ray, out hit))
            {
                //print(hit.collider.name);

                //Hover over ray hit
                Vector3 pos = hit.point;
                pos.y += dropHeight;
                transform.position = pos;
                lastPos = pos;

                //Draw line indicating drop location
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
            else
            {
                //Hover over last ray hit
                transform.position = lastPos;

                Vector3 pos = lastPos;
                pos.y -= dropHeight;

                //Draw line indicating drop location
                Debug.DrawLine(transform.position, pos, Color.green);
            }
        }
        else
        {
            isDropped = true;
            animator.SetTrigger("isDropped");
        }


        //Drop on click
    }


    public void LoseResource(int val)
    {
        resource -= val;

        if (resource <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            Destroy(this.gameObject, 5.0f);
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            Destroy(this.gameObject, 5.0f);
        }
    }

    void MovePlayer()
    {
        //Move player
        if (currentPath.Count > 0 && !isDead)
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

    void CheckToStartMoving()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("2D Blend Tree"))
        {
            currentPathing = Pathfinding.astar;
        }



        //if (animator.GetAnimatorTransitionInfo(0).IsName("TransToBlend"))
        //{
        //    print("Movement started");
        //    currentPathing = Pathfinding.astar;
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.gameObject.tag == "Floor")
        {
            isGrounded = true;
            animator.SetTrigger("isGrounded");

        }
    }
}
