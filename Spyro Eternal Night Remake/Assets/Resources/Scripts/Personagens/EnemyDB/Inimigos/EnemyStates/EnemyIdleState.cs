using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Transform playerTransform;

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
           
            if (playerTransform != null && !enemyData.PodeAndar)
            {   
              stateMachine.ChangeState(enemy.AttackState);    
            }
            else if (playerTransform != null && enemyData.PodeAndar)
            {
              stateMachine.ChangeState(enemy.ChaseState);
            }
        }
    }
}


