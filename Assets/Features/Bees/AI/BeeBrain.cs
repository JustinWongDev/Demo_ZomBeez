using System;
using UnityEngine;

public class BeeBrain : MonoBehaviour
{
    private BeeController beeController;
    private BeeAIStates BeeAIState => beeController.CurrentAIState;

    private void Start()
    {
        beeController = GetComponent<BeeController>();
        SetNewState(new BeeIdle(beeController));
    }

    private void Update()
    {
        beeController.CurrentAIState.Tick();
    }

    public void SetNewState(BeeAIStates newState)
    {
        beeController.CurrentAIState = newState;
    }
}
