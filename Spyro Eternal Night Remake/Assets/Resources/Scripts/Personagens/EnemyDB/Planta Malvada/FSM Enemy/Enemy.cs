using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region States

    public EnemyStateMachine StateMachine { get; private set; }

    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }


    [SerializeField]
    private EnemyData enemyData;
    #endregion

    public Animator Anim { get; set; }
    public Rigidbody RB { get; private set; }

    public Transform Target;

    #region Other Variables         
    public Vector3 CurrentVelocity { get; private set; }
    private Vector3 workspace;
    #endregion
    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "idle");
        ChaseState = new EnemyChaseState(this, StateMachine, enemyData, "chase");
        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "attack");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public void SetVelocity(Vector3 velocity)
    {
        RB.velocity = velocity;
        CurrentVelocity = velocity;
    }



    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();



    public bool CheckGround()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.5f);

        bool groundDetected = false;

        for (float i = -1; i <= 1; i++)
        {
            Vector2 rayDirection = Vector2.down;
            Vector2 rayPosition = new Vector2(rayOrigin.x + i * enemyData.raySpacing, rayOrigin.y);

            RaycastHit2D hit = Physics2D.Raycast(rayPosition, rayDirection, enemyData.rayCheck, enemyData.groundLayer);

            Debug.DrawRay(rayPosition, rayDirection * enemyData.rayCheck, hit.collider != null ? Color.cyan : Color.red);

            groundDetected |= hit.collider != null;
        }

        return groundDetected;
    }
}
