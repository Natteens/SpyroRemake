using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public float detectionRange = 5f; // Ajuste conforme necessário

    public EnemyIdleState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(Vector3.zero);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (enemy.Target != null)
            {
                float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.Target.position);

                if (distanceToTarget <= detectionRange)
                {
                    stateMachine.ChangeState(enemy.ChaseState);
                }
            }
        }
    }
}
