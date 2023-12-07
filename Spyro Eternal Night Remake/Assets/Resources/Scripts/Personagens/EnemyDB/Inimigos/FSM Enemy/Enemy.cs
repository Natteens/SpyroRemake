using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour, Damage
{
    #region States

    public EnemyStateMachine StateMachine { get; private set; }

    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemySPAttackState SuperAttackState { get; private set; }
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

    public VisualEffect VFX;

    public SkinnedMeshRenderer skMesh;
    public Material skMaterial;

    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    #region Vida
    //VIDA
    public float maxHealth;
    public float currentHealth;
    //ATK
    public bool podeATK = true;
    #endregion

    #endregion

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "idle");
        ChaseState = new EnemyChaseState(this, StateMachine, enemyData, "walk");
        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "atk");
        DeadState = new EnemyDeadState(this, StateMachine, enemyData, "dead");
        JumpState = new EnemyJumpState(this, StateMachine, enemyData, "pulo");
        if (enemyData.PodeAndar)
        {
            SuperAttackState = new EnemySPAttackState(this, StateMachine, enemyData, "sAtk");
           // Debug.Log(gameObject.name + "TENHO SUPER ATK");
        }
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        StateMachine.Initialize(IdleState);

        skMaterial = skMesh.material;
        maxHealth = enemyData.vidaMax;
        currentHealth = maxHealth;
        isDead = false;

    }

    private void Update()
    {

       CurrentVelocity = RB.velocity;
       StateMachine.CurrentState.LogicUpdate();
    
       if (transform.position.y <= -30)
       {
           Destruido();
       }
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();       
         CheckPlayer();
    }

    public void SetVelocity(Vector3 velocity)
    {
        RB.velocity = velocity;
        CurrentVelocity = velocity;
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public void CheckPlayer()
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
        if (!isDead)
        {
            damage = enemyData.Attack;
            currentHealth -= damage;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isDead = true; 
            } 
        }
    }

    public void Destruido()
    {
        Destroy(gameObject);
    }

    public IEnumerator DissolveCo()
    {
        float counter = 0;
        VFX.Play();

        while (skMaterial.GetFloat("_DissolveAmount") < 1f)
        {
            counter += dissolveRate;
            skMaterial.SetFloat("_DissolveAmount", counter);
        }

        yield return new WaitForSeconds(refreshRate);

        if (skMaterial.GetFloat("_DissolveAmount") >= 1f)
        {
            Destruido();
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
