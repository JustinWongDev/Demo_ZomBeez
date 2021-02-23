using UnityEngine;

public class HumanObjective : HumanAIState
{
    public HumanObjective(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public override void Tick()
    {
        if (_human.Settings.GetHasJelly())
        {
            _move.SetDestination(_move.DepotLocation());
            _move.Moveable(false);

            if (_move.NearDestination(20.0f))
            {
                _animController.Trig_Deposit();
                _move.Moveable(false);

                if (_animController.IsAnimOnBlendTree())
                {
                    _move.Moveable(true);
                    _human.Settings.SetHasJelly(false);
                }
            }
            return;
        }

        if (_human.Settings.GetHasJelly() == false)
        {
            _move.SetDestination(_move.HiveLocation());

            if (_move.NearDestination(20.0f))
            {
                _animController.Trig_Collect();
                _move.Moveable(false);
                
                if (_animController.IsAnimOnBlendTree())
                {
                    _move.Moveable(true);
                    _human.Settings.SetHasJelly(true);
                }
            }
        }
    }
}
