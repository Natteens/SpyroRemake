using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.RB.mass = 10;
        enemy.RB.isKinematic = true;

        enemy.isDead = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.isDead)
        {
            ISDEAD();
        }
    }


    public void ISDEAD()
    {
        enemy.VFX[1].Play();
    }
}

