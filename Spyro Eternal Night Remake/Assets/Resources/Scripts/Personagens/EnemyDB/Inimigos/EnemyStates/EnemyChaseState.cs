using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{

    public EnemyChaseState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (enemy.Target != null)
            {
                float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.Target.position);

                if (distanceToTarget > enemyData.attackRange)
                {
                    ChasePlayer();
                }
                else
                {
                    stateMachine.ChangeState(enemy.AttackState);
                    Debug.Log("Atacando");
                }
            }
            else
            {
                stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = Vector3.Normalize(enemy.Target.position - enemy.transform.position);
        Vector3 velocity = direction * enemyData.speed;
        enemy.SetVelocity(velocity);
    }

}
