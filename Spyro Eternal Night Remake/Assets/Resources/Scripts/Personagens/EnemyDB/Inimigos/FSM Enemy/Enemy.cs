using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour, Damage
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

    #region Other Variables         
    public Vector3 CurrentVelocity { get; private set; }
    private Vector3 workspace;
    [SerializeField] private GameObject[] damageColliders;

    public bool FindPlayer = false;
    public bool GiveDamage = false;
    public bool isDead = false;

    public VisualEffect[] VFX;

    #region Vida
    //VIDA
    public float maxHealth = 100f;
    public float currentHealth;
    //ATK
    public bool hasAttacked = false;
    public float attackTimer = 0f;
    public float attackCooldown = 1f;
    public float attackCooldownTimer = 0f;
    #endregion

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

        currentHealth = maxHealth;
        isDead = false;
    }

    private void Update()
    {
        if (!isDead)
        {
            CurrentVelocity = RB.velocity;
            StateMachine.CurrentState.LogicUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            StateMachine.CurrentState.PhysicsUpdate();
            CheckPlayer();
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        RB.velocity = velocity;
        CurrentVelocity = velocity;
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    private void CheckPlayer()
    {
        float halfViewAngle = enemyData.viewAngle / 2f;
        Vector3 forward = transform.forward;     

        for (int i = 0; i < enemyData.rayCount; i++)
        {
            float currentAngle = -halfViewAngle + (i / (float)(enemyData.rayCount - 1)) * enemyData.viewAngle;

            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;
            Vector3 rayOrigin = transform.position + Vector3.up * enemyData.rayHeight;

            Ray ray = new Ray(rayOrigin, direction);

            Debug.DrawRay(ray.origin, ray.direction * enemyData.viewDistance, Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, enemyData.viewDistance, enemyData.whatIsPlayer))
            {
                Debug.Log($"Raycast hit {hit.collider.name}");

                if (hit.collider.CompareTag("Player"))
                {
                    FindPlayer = true;
                    Target = hit.transform;
                    GiveDamage = true;
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                    break; 
                }
                else
                {
                    FindPlayer = false;
                    Target = null;
                    GiveDamage = false;
                }
            }
            else
            {
                FindPlayer = false;
                Target = null;
                GiveDamage = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        damage = enemyData.Attack;
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        StateMachine.ChangeState(DeadState);
    }

    public bool CanAttack()
    {
        return attackCooldownTimer <= 0f;
    }

    public void timerCooldown()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
          var pS = other.GetComponent<Status>();
          if (pS != null)
          {
              pS.TakeDamage(enemyData.Attack);
              Debug.Log("Ataque ao jogador");
          }       
        }
    }

}
