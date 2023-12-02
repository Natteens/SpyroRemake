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
    public EnemyDeadState DeadState { get; private set; }
    public EnemyHurtState HurtState { get; private set; }
    public EnemyJumpState JumpState { get; private set; }


    [SerializeField]
    private EnemyData enemyData;
    #endregion

    public Animator Anim;
    public Rigidbody RB { get; private set; }

    public Transform Target;
    public BoxCollider atk;

    #region Other Variables         
    public Vector3 CurrentVelocity { get; private set; }
    private Vector3 workspace;

    public bool FindPlayer = false;
    public bool GiveDamage = false;



    [SerializeField]
    private int rayCount = 5; // Número de raycasts
    [SerializeField]
    private float viewDistance = 5f; // Distância de visão
    [SerializeField]
    private float viewAngle = 60f; // Ângulo de visão

    #endregion
    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "idle");
        ChaseState = new EnemyChaseState(this, StateMachine, enemyData, "walk");
        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "atk");
        DeadState = new EnemyDeadState(this, StateMachine, enemyData, "desmaio");
        JumpState = new EnemyJumpState(this, StateMachine, enemyData, "pulo");
    }

    private void Start()
    {     
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

        CheckPlayerInSight();
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



    private void CheckPlayerInSight()
    {
        float halfViewAngle = viewAngle / 2f;
        Vector3 forward = transform.forward;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = -halfViewAngle + (i / (float)(rayCount - 1)) * viewAngle;

            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;
            Ray ray = new Ray(transform.position, direction);

            Debug.DrawRay(ray.origin, ray.direction * viewDistance, Color.yellow);

            if (Physics.Raycast(ray, out RaycastHit hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    FindPlayer = true;
                    Target = hit.transform;
                    GiveDamage = true;
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                    break;
                }
            }
        }
    }



}
