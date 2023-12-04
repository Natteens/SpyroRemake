using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySPAttackState : EnemyState
{
    private float attackCooldown = 2f;

    public EnemySPAttackState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
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

        if (!isExitingState)
        {
            if (enemy.Target != null)
            {
                float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.Target.position);

                if (distanceToTarget <= enemyData.attackRange)
                {
                    stateMachine.ChangeState(enemy.SuperAttackState);
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
