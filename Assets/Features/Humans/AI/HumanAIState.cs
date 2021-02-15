using UnityEngine;

public abstract class HumanAIState
{
    protected HumanBrain _humanBrain;
    protected Transform _transform;

    protected HumanController _controller;
    protected HumanMove _move;
    protected HumanAnimController _animController;

    public abstract void Tick();
    public abstract void LeaveState();

    public HumanAIState(HumanBrain humanBrain)
    {
        this._humanBrain = humanBrain;
        this._transform = humanBrain.transform;
        this._controller = humanBrain.GetComponent<HumanController>();
        this._animController = humanBrain.GetComponent<HumanAnimController>();
        this._move = humanBrain.GetComponent<HumanMove>();
    }
}
