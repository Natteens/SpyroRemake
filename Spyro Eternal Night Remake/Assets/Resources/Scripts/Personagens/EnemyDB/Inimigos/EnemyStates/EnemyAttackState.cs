using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float attackCooldown = 1.5f;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();
        enemy.podeATK = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (enemy.currentHealth <= 0f)
        {
            stateMachine.ChangeState(enemy.DeadState);
        }
        else if (!isExitingState)
        {
            if (enemy.Target != null)
             {
                 float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.Target.position);

                 if (distanceToTarget <= enemyData.attackRange)
                 {
                     stateMachine.ChangeState(enemy.AttackState);
                     enemy.StartCoroutine(CD());
                 }
                 else if (distanceToTarget > enemyData.attackRange)
                 {
                     stateMachine.ChangeState(enemy.ChaseState);
                 }
             }
             else
             {
                 stateMachine.ChangeState(enemy.IdleState);
             }
        }
    }

    private IEnumerator CD()
    {
        yield return new WaitForSeconds(attackCooldown);
        stateMachine.ChangeState(enemy.IdleState);
    }
}
