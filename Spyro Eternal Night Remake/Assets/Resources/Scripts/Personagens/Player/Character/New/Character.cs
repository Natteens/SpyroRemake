using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class Character : MonoBehaviour
{
    #region InputActions
    public PlayerInputActions playerActionsAsset;
    private InputAction move;
    private InputAction glide;
    private InputAction slowTime;
    private InputAction fireAttack;
    private InputAction attack;
    private InputAction dash;
    private InputAction furyAttack;
    #endregion

    #region Configurações
    // ------------------------------------------------------\\ 
    [Header("Configurações do Jogador")]
    [Space(10)]


    [Header("Gastos de skills")]
    [Space(10)]
    [Tooltip("CUSTO DE MANA PARA O ATAQUE DE FOGO!")]
    [SerializeField] private byte manaCost;
    [SerializeField] private byte dashManaCost;

    [Header("Movimentação")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private byte speed = 5;
    [SerializeField] private byte RunSpeed = 10;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [HideInInspector]
    public bool canMove = true;
    private float turnSmoothVelocity;
    // ------------------------------------------------------\\ 

    [Header("Pulo")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private byte maxJumps = 2;
    [Range(-1000, 1000)]
    [SerializeField] private byte jumpHeight = 3;
    [SerializeField] private float jumpStartTime;
    [SerializeField] private float jumpDuration = 0.5f;
    // ------------------------------------------------------\\ 

    [Header("Dash")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private byte dashForce = 10;
    [Range(0, 100)]
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float turnSmoothTimeDash = 0.1f;
    [SerializeField] private float dashCooldownTime = 2.0f; 
    private float lastDashTime;
    // ------------------------------------------------------\\ 

    [Header("Vôo")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private byte glideDrag = 2;

    // ------------------------------------------------------\\ 

    [Header("Manipulação do Tempo")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private float slowTimeScale = 0.5f;
    [SerializeField] private Volume volume;
    [HideInInspector]
    [SerializeField] private Vignette vignette;

    // ------------------------------------------------------\\ 

    [Header("Golpes")]
    [Space(10)]
    [SerializeField] private short dano;
    [SerializeField] private float vfxDuration = 0.5f;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private int currentComboStep = 0;
    [SerializeField] private float comboTimer = 0;
    [SerializeField] private float furyAttackDuration = 5f;
    [SerializeField] private VisualEffect[] VFX;
    [SerializeField] private GameObject[] damageColliders;
    [SerializeField] private LayerMask Target;
    [SerializeField] private List<Cristais> cristaisColididos = new List<Cristais>();

    private ComboState comboState = ComboState.None;
    private bool isAttacking = false;
    private bool isUsingFuryAttack = false;
    private int currentVFXIndex;
   
    private enum ComboState
    {
        None,
        FirstAttack,
        SecondAttack,
        ThirdAttack
    }
   // ------------------------------------------------------\\ 
    [Header("Configurações Adicionais")]
    [Space(10)]

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform cameraTransform;

    // ------------------------------------------------------\\ 

    #endregion

    #region Privates

    private float verticalVelocity = 0f;
    private bool isJumping = false;
    private bool isRunning = false;
    private bool planando = false;
    private bool isDashing = false;
    private bool isUsingFireBreath = false;
    private byte remainingJumps;
    private CharacterController character;
    private Animator animator;
    private Vector3 direction;
    private Vector3 dashDirection;
    private Status status;
    private Transform objVFX;
    #endregion

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        status = GetComponent<Status>();
        playerActionsAsset = new PlayerInputActions();
        animator = this.GetComponent<Animator>();
        remainingJumps = maxJumps;
        volume.profile.TryGet(out vignette);
        objVFX = VFX[4].transform;
    }

    private void OnEnable()
    {
        playerActionsAsset = new PlayerInputActions();

        move = playerActionsAsset.Player.Move;

        glide = playerActionsAsset.Player.Glide;
        glide.performed += ctx => planando = true;
        glide.canceled += ctx => planando = false;

        slowTime = playerActionsAsset.Player.SlowTime;
        slowTime.performed += ctx => ToggleSlowTime();

        fireAttack = playerActionsAsset.Player.FireAttack;
        fireAttack.performed += ctx => StartFlameAttack();
        fireAttack.canceled += ctx => StopFlameAttack();

        attack = playerActionsAsset.Player.Attack;
        attack.started += ctx => Attack();
        attack.canceled += ctx => Attack();

        dash = playerActionsAsset.Player.Dash;
        dash.performed += ctx => TryDash();

        playerActionsAsset.Player.Jump.started += OnJumpStarted;
        playerActionsAsset.Player.Jump.canceled += OnJumpCanceled;
        playerActionsAsset.Player.Run.performed += ToggleRun;

        furyAttack = playerActionsAsset.Player.FuryAttack;
        furyAttack.performed += ctx => FuryAttack();

        playerActionsAsset.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Disable();
    }

    private void Update()
    {
       if (isUsingFuryAttack)
        {     
            Vector3 oppositeDirection = objVFX.position - cameraTransform.position;
            oppositeDirection.y = 0f;          
            if (oppositeDirection != Vector3.zero)
            {
                objVFX.rotation = Quaternion.LookRotation(-oppositeDirection);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        Movement();
        if (isJumping)
            HandleJump();

        Gravity();
        if (comboState != ComboState.None)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer >= 2)
            {
                comboState = ComboState.None;
                Debug.Log("Combo Reset!");
            }
        }

    }

    #region Move
    private void Movement()
    {
        if (!canMove)
            return;

        Vector2 moveInput = move.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            character.Move(moveDir.normalized * (isRunning ? RunSpeed : speed) * Time.deltaTime);
        }
        else
        {
            Vector3 moveDir = Vector3.zero;
            character.Move(moveDir);
        }

        if (planando)
        {
            verticalVelocity = -glideDrag;
        }
    }

    #endregion
    #region Dash
    private void TryDash()
    {
        if (isDashing)
            return;

        Vector2 moveInput = move.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (direction.magnitude > 0.1f)
        {
            dashDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        }
        else
        {
            float cameraYRotation = cameraTransform.eulerAngles.y;
            dashDirection = Quaternion.Euler(0f, cameraYRotation, 0f) * Vector3.forward;
        }

        dashDirection.y = 0f;
        dashDirection.Normalize();

        if (status.currentMana >= dashManaCost)
        {
            if (Time.time - lastDashTime >= dashCooldownTime)
            {
                status.UseMana(dashManaCost);
                float targetAngle = Mathf.Atan2(dashDirection.x, dashDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                isDashing = true;
                lastDashTime = Time.time;
                StartCoroutine(DashCooldown());
            }
            else
            {
                Debug.Log("Dash ainda está em cooldown!");
            }
        }
        else
        {
            Debug.Log("Sem mana suficiente para o dash!");
        }
    }

    private IEnumerator DashCooldown()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            // Aplicar a velocidade do dash na direção do dash
            float dashSpeed = dashForce * Time.deltaTime;
            character.Move(dashDirection * dashSpeed);

            yield return null;
        }

        isDashing = false;
    }

    #endregion
    #region Jump
    private void OnJumpStarted(InputAction.CallbackContext obj)
    {
        jumpStartTime = Time.time;

        if (canMove && (character.isGrounded || remainingJumps > 0))
        {
            isJumping = true;
            verticalVelocity = jumpHeight;
            remainingJumps--;
        }
        else if (!planando)
        {
            planando = true;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext obj)
    {
        if (planando)
        {
            planando = false;
        }
        jumpStartTime = 0;
    }

    private void HandleJump()
    {
        float elapsedJumpTime = Time.time - jumpStartTime;

        if (elapsedJumpTime >= jumpDuration)
        {
            isJumping = false;
        }
        else
        {
            verticalVelocity = Mathf.Lerp(jumpHeight, 0f, elapsedJumpTime / jumpDuration);
        }
    }

    #endregion
    #region Running
    private void ToggleRun(InputAction.CallbackContext obj)
    {
        isRunning = obj.ReadValueAsButton();
    }

    #endregion

    #region FlameAttack
    private void StartFlameAttack()
    {
        if (status.currentMana >= manaCost)
        {
            isUsingFireBreath = true;
            VFX[0].Play();
            damageColliders[0].SetActive(true);
            StartCoroutine(ContinuousFlameAttack());
        }
    }
    private void StopFlameAttack()
    {
        isUsingFireBreath = false;
        VFX[0].Stop();
        damageColliders[0].SetActive(false);
        StopCoroutine(ContinuousFlameAttack());
    }

    private IEnumerator ContinuousFlameAttack()
    {
        float manaCostPerSecond = manaCost * Time.deltaTime;
        float accumulatedManaCost = 0f;

        while (isUsingFireBreath)
        {
            accumulatedManaCost += manaCostPerSecond;
            dano = 5;
            
            if (accumulatedManaCost >= 1f)
            {
                int roundedManaCost = Mathf.CeilToInt(accumulatedManaCost);
                if (status.currentMana >= roundedManaCost)
                {
                    status.UseMana(roundedManaCost);
                    accumulatedManaCost -= roundedManaCost;
                    CheckCollisions();
                }
                else
                {
                    isUsingFireBreath = false;
                    VFX[0].Stop();
                    damageColliders[0].SetActive(false);
                }
            }

            yield return null;
        }
    }

    #endregion
    #region Attack
    private void Attack()
    {
        if (canAttack)
        {
            if (!isUsingFireBreath && !isAttacking)
            {
                switch (comboState)
                {
                    case ComboState.None:
                        comboState = ComboState.FirstAttack;
                        Debug.Log("Ataque #1");
                        VFX[1].Play();
                        dano = 10;
                        currentVFXIndex = 1;
                        damageColliders[1].SetActive(true);
                        comboTimer = 0;
                        break;
                    case ComboState.FirstAttack:
                        comboState = ComboState.SecondAttack;
                        Debug.Log("Ataque #2");
                        VFX[2].Play();
                        dano = 20;
                        currentVFXIndex = 2;
                        damageColliders[2].SetActive(true);
                        comboTimer = 0;
                        break;
                    case ComboState.SecondAttack:
                        comboState = ComboState.ThirdAttack;
                        Debug.Log("Ataque #3");
                        VFX[3].Play();
                        dano = 30;
                        currentVFXIndex = 3;
                        damageColliders[3].SetActive(true);
                        comboTimer = 0;
                        break;
                    case ComboState.ThirdAttack:
                        comboState = ComboState.None;
                        Debug.Log("Combo Reset!");
                        break;
                } 
                CheckCollisions();
                StartCoroutine(WaitForVFXToEnd(vfxDuration));
                canAttack = false;
                StartCoroutine(AttackCooldown());
            }
        }
        else
        {
            isAttacking = false;
        }
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    private IEnumerator WaitForVFXToEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        damageColliders[currentVFXIndex].SetActive(false);
        VFX[1].Stop();
        VFX[2].Stop();
        VFX[3].Stop();
    }

    #endregion
    #region FuryAttack
    private void FuryAttack()
    {
        if (!isUsingFuryAttack && status.currentFuryEnergy >= 100)
        {
            status.currentFuryEnergy -= 100;
            StartCoroutine(ExecuteFuryAttack());
        }
    }
    private IEnumerator ExecuteFuryAttack()
    {
        canMove = false;
        VFX[4].Play();
        dano = 50;
        damageColliders[4].SetActive(true);
        isUsingFuryAttack = true;
        CheckCollisions();
        isRunning = false;
        isUsingFireBreath = false;
        isAttacking = false;
        yield return new WaitForSeconds(furyAttackDuration);


        isUsingFuryAttack = false;
        damageColliders[4].SetActive(false);
        VFX[4].Stop();
        canMove = true;
    }
    #endregion
    #region SlowTime
    private void ToggleSlowTime()
    {
        Debug.Log("Slow Time atual: " + (Time.timeScale));
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            vignette.intensity.Override(0.4f);
        }
        else
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
            vignette.intensity.Override(0f);
        }
    }

    #endregion

    #region Damage
    private void CheckCollisions()
    {
        foreach (GameObject damageCollider in damageColliders)
        {
            if (damageCollider.activeSelf)
            {
                Collider[] colliders = damageCollider.GetComponents<Collider>();

                foreach (Collider collider in colliders)
                {
                    Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, Target);

                    foreach (Collider hitCollider in hitColliders)
                    {
                       
                        if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Cristal"))
                        {
                            Cristais cristal = hitCollider.gameObject.GetComponent<Cristais>();

                            if (cristal != null && !cristaisColididos.Contains(cristal))
                            {
                                cristal.TakeDamage(dano);
                                cristaisColididos.Add(cristal);
                            }
                        }
                    }
                }
            }
        }

       
        cristaisColididos.Clear();
    }


    #endregion

    #region Gravidade
    private void Gravity()
    {
        if (character.isGrounded)
        {
            verticalVelocity = -0.05f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveVector = new Vector3(0f, verticalVelocity, 0f);
        character.Move(moveVector * Time.deltaTime);

        if (character.isGrounded)
        {
            remainingJumps = maxJumps;
            jumpStartTime = 0;
        }
    } 
    #endregion
}
