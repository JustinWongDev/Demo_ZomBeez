using UnityEngine;

public abstract class HumanAIState
{
    protected HumanBrain _humanBrain;
    protected Transform _transform;

    protected HumanController _humanController;
    protected HumanAnimController _animController;

    public abstract void Tick();
    public abstract void LeaveState();

    public HumanAIState(HumanBrain humanBrain)
    {
        this._humanBrain = humanBrain;
        this._transform = humanBrain.transform;
        this._humanController = humanBrain.GetComponent<HumanController>();
        this._animController = humanBrain.GetComponent<HumanAnimController>();
    }
}
