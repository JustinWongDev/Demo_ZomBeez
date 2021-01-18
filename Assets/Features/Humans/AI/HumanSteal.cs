﻿using UnityEngine;

public class HumanSteal : HumanAIState
{
    public HumanSteal(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        Debug.Log("STEAL");
        
        if (_humanController.AtDestination())
        {
            _humanController.Target = _humanController.HiveTrans();
        }
    }
    
}