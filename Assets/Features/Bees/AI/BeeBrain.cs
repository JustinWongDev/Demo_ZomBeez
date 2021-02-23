using System;
using UnityEngine;

public class BeeBrain : MonoBehaviour
{
    private BeeController bee => GetComponent<BeeController>();
    private BeeAIStates CurrentState => bee.CurrentAIState;

    private void Start()
    {
        SetNewState(new BeeIdle(bee));
    }

    private void Update()
    {
        bee.CurrentAIState.Tick();
    }

    public void SetNewState(BeeAIStates newState)
    {
        BeeAIStates temp = newState;
        newState.Initialise();
        bee.CurrentAIState = temp;
    }
}
