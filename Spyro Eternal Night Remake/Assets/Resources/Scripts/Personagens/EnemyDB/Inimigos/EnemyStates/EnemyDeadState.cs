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

        ISDEAD();
    }


    public void ISDEAD()
    {
        enemy.VFX.Play();
    //    enemy.Destruido();
        if (enemyData.PodeAndar)
        {
            enemy.Anim.SetBool("dead", true);
            enemy.StartCoroutine(enemy.DissolveCo());
        }
    }
}

