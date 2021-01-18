using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum Pathfinding { None, Astar };

public class HumanController : NavAgent
    {
        //Stats
        [FormerlySerializedAs("settings")] [SerializeField]
        private HumanSettings _settings;
        public HumanSettings Settings {get {return _settings;}}

        private float _currentHealth;
        private float _currentArmour;
        private float _currentDamage;
        private float _currentResource;
        public float CurrentResource => _currentResource;

        private bool _isDead = false;
        public float awareRadius = 30.0f;
        private bool _isAware = false;
        
        //Items
        private HumanInventory _inventory;
        public HumanInventory Inventory => _inventory;
        
        //DFA
        private int newState = 0;
        private int currentState = -1;
        private int CurrentBehaviour { get; set; } = 0;

        private int[,] myDFA = new int [3, 5];
        private readonly int[,] dfaCiv = new int[,]
        {
            {1, 0, 0, 2, 2},            //seek
            {0, -1, -1, -1, -1 },       //ability
            {1, 2, 2, 2, 2 },           //flee
            {0, -1, -1, -1, -1 }        //steal
        };
        
        private readonly int[,] dfaKeeper = new int[,]
        {
            {0, -1, -1, -1, -1},            //seek
            {1, 3, 1, 1, 2 },               //ability
            {1, 3, 3, 1, 2 },               //flee
            {1, 3, 3, 1, 2 }                //steal
        };
        
        private readonly int[,] dfaSadist = new int[,]
        {
            {1, 0, 0, 1, 2},            //seek
            {1, 0, 0, 1, 2 },       //ability
            {1, 0, 0, 1, 2 },           //flee
            {0, -1, -1, -1, -1 }        //steal
        };

        [Header("Movement")]
        public Pathfinding currentPathing = Pathfinding.None;
        public float minDistance = 0.01f;
        public float acceleration = 5.0f;
        public float deceleration = 25.0f;
        private float currentSpeed = 0;
        private float maxSpeed;

        [Header("Animations")]
        public GameObject modelHolder;
        private HumanAnimController _animController;

        private static readonly int IsDead = Animator.StringToHash("_isDead");
        private static readonly int Property = Animator.StringToHash("Velocity Z");


        private void Update()
        {
            newState = DfaLogic();
            Behaviour();

            Pathing();

            MovePlayer();
        }

        #region Start up
        public void Initialise(HumanSO so)
        {
            //Instantiate model
            GameObject go = Instantiate(so.model, modelHolder.transform);
            GetComponent<Droppable>().Initialise();
            _animController = GetComponent<HumanAnimController>();
            _animController.Initialise();
            //Link _inventory
            _inventory = GetComponent<HumanInventory>();
            
            
            //Set stats
            _currentHealth = Settings.MaxHealth * Random.Range(0.8f, 1.2f);
            _currentArmour = Settings.MaxArmour * Random.Range(0.8f, 1.2f);
            _currentDamage = Settings.MaxDamage * Random.Range(0.8f, 1.2f);
            _currentResource = Settings.MaxResource * Random.Range(0.8f, 1.2f);
            
            maxSpeed = so.max_Speed * Random.Range(0.8f, 1.2f);

            //AI
            switch (so.currentType)
            {
                case HumanType.civilian:
                    newState = 0;
                    currentState = 0;
                    CurrentBehaviour = 0;
                    
                    myDFA = dfaCiv;
                    break;
                case HumanType.keeper:
                    newState = 3;
                    currentState = 3;
                    CurrentBehaviour = 3;
                    
                    myDFA = dfaKeeper;
                    break;
                case HumanType.sadist:
                    newState = 0;
                    currentState = 0;
                    CurrentBehaviour = 0;
                    
                    myDFA = dfaSadist;
                    break;
                default:
                    print("Error in initialising DFA");
                    break;
            }
        }
        
        #endregion

        private int DfaLogic()
        {
            if (_currentHealth <= (Settings.MaxHealth / 3))
            {
                return 3;
            }
            else if (_currentHealth <= (Settings.MaxHealth * 0.75f))
            {
                return 2;
            }
            else if (Awareness())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private bool Awareness()
        {
            return false;
        }
        
        private void Behaviour()
        {
            //New state logic
            if (newState != currentState)
            {
                if (myDFA[CurrentBehaviour, 0] == 1)
                {
                    CurrentBehaviour = myDFA[CurrentBehaviour, newState + 1];
                }

                currentState = newState;
            }
    
            //State logic 
            switch (CurrentBehaviour)
            {
                case 0:
                    Seek();
                    break;
                case 1:
                    Ability();
                    break;
                case 2:
                    Flee();
                    break;
                case 3:
                    Steal();
                    break;
                default:
                    print("Error: human behaviour");
                    break;
            }
        }

        #region Seek
        private int Seek()
        {
            print("seeKING");
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
        
        #endregion
        
        #region Ability
        private void Ability()
        {
            print("ABILITYING");
            //pred pray
        }

        #endregion
        
        #region Flee
        private void Flee()
        {
            print("FLEEING");
            //pred prey
        }

        #endregion
        
        #region Steal
        private int Steal()
        {
            int stealIndex = findClosestWaypointToSteal();
            
            if (stealIndex == -1)
            {
                return UnityEngine.Random.Range(0, graphNodes.graphNodes.Length);
            }

            return stealIndex;
        }
        
        private int findClosestWaypointToSteal()
        {
            //Locate Hive
            Hive hive = FindObjectOfType<Hive>();
            //Find which waypoint is closest to hive
            float distance = Mathf.Infinity;
            int closestWaypoint = -1;

            //Find closest node to site
            for (int i = 0; i < graphNodes.graphNodes.Length; i++)
            {
                if (Vector3.Distance(hive.transform.position, graphNodes.graphNodes[i].transform.position) <= distance)
                {
                    distance = Vector3.Distance(hive.transform.position, graphNodes.graphNodes[i].transform.position);
                    closestWaypoint = i;
                }
            }

            return closestWaypoint;
        }
        
        #endregion

        #region Navigation

        void Pathing()
        {
            //Auto random pathing
            switch (currentPathing)
            {
                case Pathfinding.None:
                    break;
                case Pathfinding.Astar:
                    if(currentNodeIndex == currentPath[currentPath.Count-1])
                    {
                        switch (CurrentBehaviour)
                        {
                            case 0:             //seek
                                currentPath = AStarSearch(currentPath[currentPathIndex], Seek());
                                currentPathIndex = 0;
                                break;
                            case 1:             //ability
                                break;
                            case 2:             //flee
                                break;
                            case 3:             //steal
                                currentPath = AStarSearch(currentPath[currentPathIndex], Steal());
                                currentPathIndex = 0;
                                break;
                        }


                    }
                    break;
            }
            
        }

        void MovePlayer()
        {
            //Move player
            if (currentPath.Count > 0 && !_isDead)
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
            _animController.Float_VelocityZ(currentSpeed);
        
        }
        #endregion
        
        public void LoseResource(int val)
        {
            _currentResource -= val;

            if (_currentResource <= 0)
            {
                _isDead = true;
                _animController.Bool_IsDead(true);
                Destroy(this.gameObject, 5.0f);
            }
        }

        public void TakeDamage(float dmg)
        {
        //Armour calc
            float val = _currentArmour - dmg;
            if (val >= 0)
            {
                _currentArmour -= dmg;
                return;
            }
            else if (val < 0)
            {
                _currentArmour = 0;
                
        //Health calc
                _currentHealth += val;
                
        //Death logic
                if (_currentHealth <= 0)
                {
                    _isDead = true;
                    _animController.Bool_IsDead(true);
                    Destroy(this.gameObject, 5.0f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isAware)
            {
                _isAware = other.GetComponent<Worker>();
            }


        }
    }

