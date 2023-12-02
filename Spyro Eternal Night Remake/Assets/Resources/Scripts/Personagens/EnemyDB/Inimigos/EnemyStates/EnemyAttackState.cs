using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{

    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
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
                    stateMachine.ChangeState(enemy.ChaseState);
                }
                else if (enemy.CanAttack())
                {                   
                    stateMachine.ChangeState(enemy.AttackState);
                    enemy.attackCooldownTimer = enemy.attackCooldown;
                }
            }
            else
            {
                stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemy.timerCooldown();
    }
}
