using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private float atkCD = 1.5f;

    private bool useSuperAtk;

    public EnemyIdleState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(Vector3.zero);
        enemy.StartCoroutine(resetCD());
        useSuperAtk = Random.Range(0, 2) == 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState && enemy.Target != null)
        {
            float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.Target.position);
            if (distanceToTarget > enemyData.attackRange)
            {
                stateMachine.ChangeState(enemy.ChaseState);
            }
            else if (distanceToTarget <= enemyData.attackRange && enemy.podeATK)
            {
                if (enemyData.PodeAndar)
                {
                    if (useSuperAtk)
                    {
                        stateMachine.ChangeState(enemy.SuperAttackState);
                    }
                    else
                    {
                        stateMachine.ChangeState(enemy.AttackState);
                    }
                }
                else
                {
                    stateMachine.ChangeState(enemy.AttackState);
                }
            }
        }
    }

    private IEnumerator resetCD()
    {
        yield return new WaitForSeconds(atkCD);
        enemy.podeATK = true;
    }
}
