using UnityEngine;

public class HumanObjective : HumanAIState
{
    public HumanObjective(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    private float _collectTimer = 0.0f;
    
    public override void Tick()
    {
        if (!_human.Settings.GetHasJelly())
        {
            if(!_move.AtDestination())
                _move.SetTarget(_move.HiveLocation());
            else
            {
                _collectTimer += Time.deltaTime;
                if (_collectTimer >= _human.Settings.CollectTime)
                {
                    _human.Settings.SetHasJelly(true);
                }
            }
        }
        else
        {
            if(!_move.AtDestination())
                _move.SetTarget(_move.DepotLocation());
        }
    }
}
