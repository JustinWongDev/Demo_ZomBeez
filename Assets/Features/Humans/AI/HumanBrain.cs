using System;
using UnityEngine;

public class HumanBrain : MonoBehaviour
{
    //DFA
    private int newState = 0;
    private int currentState = -1;
    private int currentBehaviour { get; set; } = 0;

    private int[,] myDFA = new int [3, 5];
    private readonly int[,] dfaCiv = new int[,]
    {
        {1, 0, 0, 2, 2 },        //objective
        {0, -1,-1,-1,-1 },       //offense
        {1, 2, 2, 2, 2 },        //defense
    };

    private readonly int[,] dfaKeeper = new int[,]
    {
        {0, -1, -1, -1, -1},     
        {1, 3, 1, 1, 2 },        
        {1, 3, 3, 1, 2 },        
    };
    
    private readonly int[,] dfaSadist = new int[,]
    {
        {1, 0, 0, 1, 2},        
        {1, 0, 0, 1, 2 },       
        {1, 0, 0, 1, 2 },       
    };
    
    private HumanController _controller;
    private HumanSO so;
    private HumanAIState currentAIState;
    private bool isDropped => !GetComponent<Droppable>();
    
    public int CurrentBehaviour => currentBehaviour;
    public HumanAIState CurrentAIState => currentAIState;

    private void Start()
    {
        _controller = GetComponent<HumanController>();
        Behaviour();
    }

    private void Update()
    {
        newState = DfaLogic();
        if (newState == 3)
            return;
        Behaviour();
        currentAIState.Tick();
    }

    public void Initialise(HumanSO val)
    {
        this.so = val;

        switch (val.currentType)
        {
            case HumanType.civilian:
                newState = 0;
                currentState = 0;
                currentBehaviour = 0;

                myDFA = dfaCiv;
                break;
            case HumanType.keeper:
                newState = 0;
                currentState = 0;
                currentBehaviour = 0;

                myDFA = dfaKeeper;
                break;
            case HumanType.sadist:
                newState = 1;
                currentState = 1;
                currentBehaviour = 1;

                myDFA = dfaSadist;
                break;
            default:
                print("Error in initialising DFA");
                break;
        }
    }
    
    private int DfaLogic()
    {
        if (!isDropped)
            return 3;
        
        if (_controller.Settings.HealthPercentCheck(0.30f))
            return 2;
        else if (_controller.Settings.HealthPercentCheck(0.75f))
            return 1;
        else
            return 0;
    }

    private void Behaviour()
    {
        //Check for new state
        if (newState != currentState)
        {
            if (myDFA[currentBehaviour, 0] == 1)
            {
                currentBehaviour = myDFA[currentBehaviour, newState + 1];
                
                //Switch behaviour
                switch (currentBehaviour)
                {
                    case 0:
                        SetState(new HumanObjective(this));
                        break;
                    case 1:
                        SetState(new HumanOffensive(this));
                        break;
                    case 2:
                        SetState(new HumanDefensive(this));
                        break;
                    case 3:
                        SetState(new HumanDrop(this));
                        break;
                    default:
                        print("Error: human behaviour");
                        break;
                }
            }

            currentState = newState;
        }
    }

    private void SetState(HumanAIState state)
    {
        currentAIState?.LeaveState();
        
        if(currentAIState != null)
            Debug.Log(currentAIState.ToString());
        
        currentAIState = state;
    }
}
