using UnityEngine;

public class BeeAttack : BeeAIStates
{
    private float attackTimer = 0.0f;
    
    public BeeAttack(BeeController bee) : base(bee)
    {
    }

    public override void Initialise()
    {
        bee.Target = hive.HumansToAttack[0].gameObject;
    }

    public override void Tick()
    {
        Timers();
        
        bee.Target = hive.HumansToAttack[0].gameObject;
        bee.OrbitPos(bee.Target.transform.position);
        
        if (attackTimer >= BeeSettings.AttackTime)
        {
            bee.Target.GetComponent<HumanController>()?.ReceiveDamage(BeeSettings.Damage);
            attackTimer = 0.0f;
        }
    }

    private void Timers()
    {
        attackTimer += Time.deltaTime;
    }
}
