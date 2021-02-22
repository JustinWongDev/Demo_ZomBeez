using UnityEngine;

public class HumanOffensive : HumanAIState
{
    private float attackTimer = 0.0f;
    private float abilityTimer  = 0.0f;
    private Worker attackTarget = null;
    
    public HumanOffensive(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
    }

    public void SetTarget(Worker targ)
    {
        attackTarget = targ;
    }
    
    public override void Tick()
    {
        Timers();

        if (abilityTimer >= _human.Settings.AbilityTime)
        {
            _human.UseAbility();
            abilityTimer = 0.0f;
            return;
        }

        if (attackTimer >= _human.Settings.AttackTime) //&& no ability/attack anims playing
        {
            if(Vector3.Distance(_humanBrain.transform.position, attackTarget.transform.position) > _human.Settings.Reach)
                _move.SetDestination(attackTarget.transform);
            else
            {
                attackTarget.takeDamage(_human.Settings.Damage);
                attackTimer = 0.0f;
            }
        }
    }

    private void Timers()
    {
        if (abilityTimer <= _human.Settings.AbilityTime)
        {
            abilityTimer += Time.deltaTime;
            return;
        }
        
        if (attackTimer <= _human.Settings.AttackTime)
        {
            attackTimer += Time.deltaTime;
        }
    }
}
