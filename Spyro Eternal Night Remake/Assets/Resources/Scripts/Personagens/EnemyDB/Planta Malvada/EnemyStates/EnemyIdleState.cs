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
        playerTransform = null; // Reinicialize o transform do jogador ao entrar no estado idle
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
                // Verifique se há obstruções entre o inimigo e o jogador
                Vector3 directionToPlayer = playerTransform.position - enemy.transform.position;

                if (!Physics.Raycast(enemy.transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, enemyData.whatIsPlayer))
                {
                    stateMachine.ChangeState(enemy.ChaseState);
                }
            }

            Debug.Log("Idle");
        }
    }
}


