using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HumanBrain : MonoBehaviour
{
    [SerializeField]
    private float firstHealthThreshold = 0.80f;
    [SerializeField]
    private float secondHealthThreshold = 0.50f;
    
    private int _potentialState = 0;
    private int _currentState = -1;
    private int _currentBehaviour { get; set; } = 0;

    private int[,] myDFA = new int [3, 4];
    private readonly int[,] _dfaCiv = new int[,]
    {
        {1, 0, 2, 2},        //objective
        {0, -1,-1,-1},       //offense
        {1, 0, 2, 2},        //defense
    };

    private readonly int[,] _dfaKeeper = new int[,]
    {
        {1, 0, 1, 2},     
        {1, 0, 1, 2},        
        {1, 0, 1, 2},        
    };
    
    private readonly int[,] _dfaSadist = new int[,]
    {
        {1, 0, 1, 1},        
        {1, 1, 1, 1},       
        {0, -1, -1, -1},       
    };
    
    private HumanController _controller = null;
    private HumanSO so = null;
    private HumanAIState _currentAIState = null;
    private bool isDropped => !GetComponent<Droppable>();
    
    public int CurrentBehaviour => _currentBehaviour;
    public HumanAIState CurrentAIState => _currentAIState;

    private void Update()
    {
        _potentialState = DfaLogic();
        if (_potentialState == 3)
        {
            return;
        }
        
        BehaviourSwitching();
        _currentAIState = new HumanObjective(this);
        _currentAIState.Tick();
    }

    public void Initialise(HumanSO val)
    {
        this.so = val;

        switch (val.currentType)
        {
            case HumanType.civilian:
                _potentialState = 0;
                _currentState = 0;
                _currentBehaviour = 0;

                myDFA = _dfaCiv;
                break;
            case HumanType.keeper:
                _potentialState = 0;
                _currentState = 0;
                _currentBehaviour = 0;

                myDFA = _dfaKeeper;
                break;
            case HumanType.sadist:
                _potentialState = 1;
                _currentState = 1;
                _currentBehaviour = 1;

                myDFA = _dfaSadist;
                break;
            default:
                print("Error in initialising DFA");
                break;
        }
        
        _controller = GetComponent<HumanController>();
        
        BehaviourSwitching();
    }
    
    private int DfaLogic()
    {
        if (!isDropped)
            return 3;
        
        if (_controller.Settings.HealthPercentCheck(secondHealthThreshold))
            return 2;
        else if (_controller.Settings.HealthPercentCheck(firstHealthThreshold))
            return 1;
        else
            return 0;
    }

    private void BehaviourSwitching()
    {
        //Check for new state
        if (_potentialState == _currentState)
            return;
        
        if (myDFA[_currentBehaviour, 0] == 1)
        {
            _currentBehaviour = myDFA[_currentBehaviour, _potentialState + 1];
            
            //Switch behaviour
            switch (_currentBehaviour)
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
            }
        }
        
        _currentState = _potentialState;
    }

    private void SetState(HumanAIState state)
    {
        _currentAIState?.LeaveState();

        _currentAIState = state;
    }
}
