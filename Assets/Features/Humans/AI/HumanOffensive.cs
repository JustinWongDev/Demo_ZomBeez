using UnityEngine;

public class HumanOffensive : HumanAIState
{
    private float attackTimer = 0.0f;
    private float abilityTimer  = 0.0f;
    
    public HumanOffensive(HumanBrain humanBrain) : base(humanBrain)
    {
    }

    public override void LeaveState()
    {
        
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
            _human.Attack();
            abilityTimer = 0.0f;
        }
    }

    private void Timers()
    {
        if (abilityTimer <= _human.Settings.AbilityTime)
        {
            abilityTimer += Time.deltaTime;
        }
        
        if (attackTimer <= _human.Settings.AttackTime)
        {
            attackTimer += Time.deltaTime;
        }
    }
}
