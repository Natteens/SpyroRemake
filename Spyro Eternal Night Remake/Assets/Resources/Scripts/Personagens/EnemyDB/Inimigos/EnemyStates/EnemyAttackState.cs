using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public bool atacando = false;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        var pS = enemy.Target.GetComponent<Status>();

        if (enemy.GiveDamage && !atacando)
        {
            atacando = true;
            pS.TakeDamage(enemyData.Attack);
            Debug.Log("Ataquei");
            stateMachine.ChangeState(enemy.IdleState);
        }
       else
        {
            stateMachine.ChangeState(enemy.IdleState);
        }
        

    }

    
   
}