using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Camera;

namespace Features.Humans
{
    public class HumanController : NavAgent
    {
        [Header("Items")]
        public List<ItemSO> items = new List<ItemSO>();
        
        //Stats
        private float currentHealth;
        private float maxHealth;
        private float currentArmour;
        private float maxArmour;
        private float maxDamage;
        [HideInInspector]
        public int resource;
        private bool isDead = false;
        private Dfa myDfa;

        [Header("Movement")]
        public Pathfinding currentPathing = Pathfinding.None;
        public enum Pathfinding { None, Astar };
        public float minDistance = 0.01f;
        public float acceleration = 5.0f;
        public float deceleration = 25.0f;
        private float currentSpeed = 0;
        private float maxSpeed;

        [Header("Drop")]
        public float dropHeight = 20.0f;
        private Vector3 mousePos;
        private Vector3 lastPos;
        private bool isDropped = false;
        private bool isGrounded = false;
        public bool IsGround => isGrounded;

        [Header("Animations")]
        public Animator animator;
        public GameObject modelHolder;

        
        private static readonly int IsDropped = Animator.StringToHash("isDropped");
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int Property = Animator.StringToHash("Velocity Z");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");


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
                case Pathfinding.None:
                    break;
                case Pathfinding.Astar:
                    if(currentNodeIndex == currentPath[currentPath.Count-1])
                    {
                        currentPath = AStarSearch(currentPath[currentPathIndex], Forage());
                        currentPathIndex = 0;
                    }
                    break;
            }

            MovePlayer();
        }

        public void Initialise(HumanSO so)
        {
            //Instantiate model
            GameObject go = Instantiate(so.model, modelHolder.transform);
            //Link animator
            animator = go.GetComponent<Animator>();
            
            //Set stats
            maxHealth = so.max_Health * Random.Range(0.8f, 1.2f);
            currentHealth = maxHealth;
            maxArmour = so.max_Armour * Random.Range(0.8f, 1.2f);
            currentArmour = maxArmour;
            maxSpeed = so.max_Speed * Random.Range(0.8f, 1.2f);
            resource = (int)(so.max_Resource * Random.Range(0.5f, 1.2f));
            maxDamage = so.max_Damage * Random.Range(0.8f, 1.2f);
            
            //AI
            myDfa = so.dfa;
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
                Ray ray = main.ScreenPointToRay(Input.mousePosition);

                //If ray hits...
                var transformPosition = transform.position;
                if (Physics.Raycast(ray, out hit))
                {
                    //print(hit.collider.name);

                    //Hover over ray hit
                    Vector3 pos = hit.point;
                    pos.y += dropHeight;
                    transform.position = pos;
                    lastPos = pos;

                    //Draw line indicating drop location
                    Debug.DrawLine(transformPosition, hit.point, Color.green);
                }
                else
                {
                    //Hover over last ray hit
                    transform.position = lastPos;

                    Vector3 pos = lastPos;
                    pos.y -= dropHeight;

                    //Draw line indicating drop location
                    Debug.DrawLine(transformPosition, pos, Color.green);
                }
            }
            else
            {
                isDropped = true;
                animator.SetTrigger(IsDropped);
            }
        }


        public void LoseResource(int val)
        {
            resource -= val;

            if (resource <= 0)
            {
                isDead = true;
                animator.SetBool(IsDead, true);
                Destroy(this.gameObject, 5.0f);
            }
        }

        public void TakeDamage(float dmg)
        {
        //Armour calc
            float val = currentArmour - dmg;
            if (val >= 0)
            {
                currentArmour -= dmg;
                return;
            }
            else if (val < 0)
            {
                currentArmour = 0;
                
        //Health calc
                currentHealth += val;
                
        //Death logic
                if (currentHealth <= 0)
                {
                    isDead = true;
                    animator.SetBool(IsDead, true);
                    Destroy(this.gameObject, 5.0f);
                }
            }
        }

        void MovePlayer()
        {
            //Move player
            if (currentPath.Count > 0 && !isDead)
            {
                var targetPos = graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position;

                //Set speed
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += maxSpeed * Time.deltaTime * acceleration;
                }

                //Move towards next node
                Transform transform1;
                (transform1 = transform).position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

                //Face direction
                var targetDir = targetPos - transform1.position;
                var step = currentSpeed * Time.deltaTime;
                var newDir = Vector3.RotateTowards(transform1.forward, targetDir, step, 0.0f);
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
            if (currentSpeed > 0 && currentPathing == Pathfinding.None)
            {
                currentSpeed -= maxSpeed * Time.deltaTime * deceleration;
            }

            //Update anim variables
            animator.SetFloat(Property, currentSpeed);
        
        }

        private int Forage()
        {
            int forageIndex = findClosestWaypointToForage();
            
            if (forageIndex == -1)
            {
                return UnityEngine.Random.Range(0, graphNodes.graphNodes.Length);
            }

            return forageIndex;
        }

        private int findClosestWaypointToForage()
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

            //find which waypoint is closest to active, forage site
            float distance = Mathf.Infinity;
            int closestWaypoint = -1;

            //Find closest node to site
            for (int i = 0; i < graphNodes.graphNodes.Length; i++)
            {
                if (Vector3.Distance(closestSite.transform.position, graphNodes.graphNodes[i].transform.position) <= distance)
                {
                    distance = Vector3.Distance(closestSite.transform.position, graphNodes.graphNodes[i].transform.position);
                    closestWaypoint = i;
                }
            }

            return closestWaypoint;
        }

        private int findClosestWaypoint()
        {
            //Convert mouse coordinates to world position
            if (!(main is null))
            {
                Ray ray = main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    mousePos = hit.point;
                }
            }

            Debug.DrawLine(main.transform.position, mousePos, Color.red);

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
                currentPathing = Pathfinding.Astar;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.transform.gameObject.CompareTag("Floor"))
            {
                isGrounded = true;
                animator.SetTrigger(IsGrounded);
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            }
        }
    }
}
