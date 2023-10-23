using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(Vector3.zero);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState)
        {
           
            Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, enemyData.DetectRay, enemyData.whatIsPlayer);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    stateMachine.ChangeState(enemy.ChaseState);
                }
            }

            Debug.Log("Idle");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stateMachine.ChangeState(enemy.ChaseState);
        }
    }
}
