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
    private InputAction dash; // Nova InputAction para o dash
    #endregion

    #region Configurações
    // ------------------------------------------------------\\ 
    [Header("Configurações do Jogador")]
    [Space(10)]

    [Header("Movimentação")]
    [Space(5)]

    [Range(0, 10)]
    [SerializeField] private float maxSpeed = 5f;
    [Range(0, 10)]
    [SerializeField] private float movementForce = 1f;
    [Range(0, 10)]
    [SerializeField] private float runSpeed = 10f;

    // ------------------------------------------------------\\ 

    [Header("Pulo")]
    [Space(5)]

    [Range(0, 10)]
    [SerializeField] private int maxJumps = 2;
    [Range(0, 10)]
    [SerializeField] private float jumpForce = 5f;

    // ------------------------------------------------------\\ 

    [Header("Dash")]
    [Space(5)]

    [Range(0, 10)]
    [SerializeField] private float dashForce = 10f;
    [Range(0, 10)]
    [SerializeField] private float dashDuration = 0.2f;
    [Range(0, 10)]
    [SerializeField] private float dashCooldown = 1f;

    // ------------------------------------------------------\\ 

    [Header("Vôo")]
    [Space(5)]

    [Range(0, 10)]
    [SerializeField] private float glideForce = 1f;
    [Range(0, 10)]
    [SerializeField] private float glideDrag = 2f;

    // ------------------------------------------------------\\ 

    [Header("Manipulação do Tempo")]
    [Space(5)]

    [Range(0, 10)]
    [SerializeField] private float slowTimeScale = 0.5f;

    // ------------------------------------------------------\\ 

    [Header("Configurações Adicionais")]
    [Space(10)]
    [SerializeField] private Camera playerCamera;
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

        dash = playerActionsAsset.Player.Dash; // Configuração da ação de dash
        dash.performed += ctx => TryDash(); // Iniciar o dash quando a ação for realizada

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
        // Se estiver no estado de dash, não aplicar outras forças
        if (isDashing)
            return;

        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

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

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (planando)
        {
            rb.velocity -= rb.velocity * glideDrag * Time.fixedDeltaTime;
        }

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private void TryDash()
    {
        // Verificar se o dash está em recarga
        if (Time.time < dashCooldownTimer)
            return;

        // Aplicar a força do dash na direção do movimento atual
        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f)
        {
            Vector3 dashDirection = rb.velocity.normalized;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

            // Iniciar o tempo de recarga do dash
            dashCooldownTimer = Time.time + dashCooldown;

            // Iniciar a duração do dash
            StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCooldown()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void OnJumpStarted(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            forceDirection += Vector3.up * jumpForce;
            remainingJumps = maxJumps - 1;
        }
        else if (remainingJumps > 0)
        {
            forceDirection += Vector3.up * jumpForce;
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
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            return true;
        else
            return false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
