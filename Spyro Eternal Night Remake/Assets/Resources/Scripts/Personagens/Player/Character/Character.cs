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
    private PlayerInputActions playerActionsAsset;
    private InputAction move;
    private InputAction glide;
    private InputAction slowTime;
    private InputAction attack;
    private InputAction dash; 
    #endregion

    #region Configurações
    // ------------------------------------------------------\\ 
    [Header("Configurações do Jogador")]
    [Space(10)]

    [Header("Movimentação")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private float maxSpeed = 5f;
    [Range(0, 100)]
    [SerializeField] private float movementForce = 1f;
    [Range(0, 100)]
    [SerializeField] private float runSpeed = 10f;
    [Range(0, 1000)]
    [SerializeField] private float rotationSpeed;

    // ------------------------------------------------------\\ 

    [Header("Pulo")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private int maxJumps = 2;
    [Range(0, 100)]
    [SerializeField] private float jumpForce = 5f;

    // ------------------------------------------------------\\ 

    [Header("Dash")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private float dashForce = 10f;
    [Range(0, 100)]
    [SerializeField] private float dashDuration = 0.2f;
    [Range(0, 100)]
    [SerializeField] private float dashCooldown = 1f;

    // ------------------------------------------------------\\ 

    [Header("Vôo")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private float glideForce = 1f;
    [Range(0, 100)]
    [SerializeField] private float glideDrag = 2f;

    // ------------------------------------------------------\\ 

    [Header("Manipulação do Tempo")]
    [Space(5)]

    [Range(0, 100)]
    [SerializeField] private float slowTimeScale = 0.5f;

    // ------------------------------------------------------\\ 

    [Header("Configurações Adicionais")]
    [Space(10)]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Volume volume;
    [SerializeField] private Vignette vignette;
    [SerializeField] private VisualEffect flamethrower;
    // ------------------------------------------------------\\ 
    #endregion

    #region Privates

    private bool isRunning = false;
    private bool planando = false;
    private bool isDashing = false;
    private int remainingJumps;
    private float originalMaxSpeed;
    private float dashCooldownTimer = 0f;
    private Vector3 forceDirection = Vector3.zero;
    private Rigidbody rb;
    private Animator animator;
   

    #endregion

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new PlayerInputActions();
        animator = this.GetComponent<Animator>();
        originalMaxSpeed = maxSpeed;
        remainingJumps = maxJumps;
        volume.profile.TryGet(out vignette);
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
        attack = playerActionsAsset.Player.Attack;
        attack.started += ctx => DoAttack();
        attack.canceled += ctx => StopAttack();

        dash = playerActionsAsset.Player.Dash; 
        dash.performed += ctx => TryDash(); 

        playerActionsAsset.Player.Jump.started += OnJumpStarted;
        playerActionsAsset.Player.Jump.canceled += OnJumpCanceled;
        playerActionsAsset.Player.Run.performed += ToggleRun;

        playerActionsAsset.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Disable();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        Vector2 moveInput = move.ReadValue<Vector2>();
        Vector3 moveDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        moveDirection.y = 0; // Mantém a direção no plano horizontal.

        if (isRunning)
        {
            maxSpeed = runSpeed;
        }
        else
        {
            maxSpeed = originalMaxSpeed;
        }

        if (planando)
        {
            rb.AddForce(Vector3.up * glideForce, ForceMode.Force);
        }

        rb.AddForce(moveDirection.normalized * movementForce, ForceMode.Impulse);

        if (planando)
        {
            rb.velocity -= rb.velocity * glideDrag * Time.fixedDeltaTime;
        }

        if (rb.velocity.y < 0f)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
        }

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;

        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
    private void TryDash()
    {
       
        if (Time.time < dashCooldownTimer)
            return;

       
        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f)
        {
            Vector3 dashDirection = rb.velocity.normalized;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

          
            dashCooldownTimer = Time.time + dashCooldown;

            
            StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCooldown()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private void OnJumpStarted(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            remainingJumps = maxJumps - 1;
        }
        else if (remainingJumps > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
    }

    private void ToggleRun(InputAction.CallbackContext obj)
    {
        isRunning = obj.ReadValueAsButton();
    }

    private void DoAttack()
    {
        flamethrower.Play();
    }

    private void StopAttack()
    {
        flamethrower.Stop();
    }

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

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            return true;
        else
            return false;
    }

   
}
