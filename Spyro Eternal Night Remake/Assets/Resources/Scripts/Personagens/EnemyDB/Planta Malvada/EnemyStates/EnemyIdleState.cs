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
            // Verifique se o jogador está dentro do raio de detecção
            Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, enemyData.DetectRay, enemyData.whatIsPlayer);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerTransform = collider.transform; // Armazene a referência ao transform do jogador
                    break;
                }
            }

            if (playerTransform != null)
            {   
              stateMachine.ChangeState(enemy.AttackState);    
            }

            Debug.Log("Idle");
        }
    }
}


