using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private float rotationSpeed = 5f;

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

                RotateTowardsPlayer();

                if (distanceToTarget <= enemyData.attackRange)
                {
                    stateMachine.ChangeState(enemy.AttackState);
                    Debug.Log("Atacando");
                }
                else if (distanceToTarget > enemyData.attackRange)
                {
                    ChasePlayer();
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
        if (enemyData.PodeAndar)
        {
            RotateTowardsPlayer();
            Vector3 direction = Vector3.Normalize(enemy.Target.position - enemy.transform.position);
            Vector3 velocity = direction * enemyData.speed;
            enemy.SetVelocity(velocity);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = Vector3.Normalize(enemy.Target.position - enemy.transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

}
