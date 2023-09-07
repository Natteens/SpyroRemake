using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Character : MonoBehaviour
{
    private PlayerInputActions playerActionsAsset;
    private InputAction move;
    private InputAction glide; // Ação para o glide
    private InputAction slowTime; // Ação para o SlowTime

    [SerializeField] private float movementForce = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float glideForce = 1f;
    [SerializeField] private float glideDrag = 2f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private int maxJumps = 2;

    private bool planando = false;
    private Rigidbody rb;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;
    private Animator animator;

    private int remainingJumps;
    private bool isRunning = false;
    private float originalMaxSpeed;

    [SerializeField] private float slowTimeScale = 0.5f; // Ajuste a escala de tempo desejada para o SlowTime

    public Volume volume; // Componente de volume
    private Vignette vignette; // Efeito de vinheta

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new PlayerInputActions();
        animator = this.GetComponent<Animator>();
        originalMaxSpeed = maxSpeed;
        remainingJumps = maxJumps;


      //  volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    private void OnEnable()
    {
        playerActionsAsset = new PlayerInputActions();
        move = playerActionsAsset.Player.Move;
        glide = playerActionsAsset.Player.Glide; // Configuração da ação de glide
        glide.performed += ctx => planando = true; // Iniciar o glide quando a ação for realizada
        glide.canceled += ctx => planando = false; // Parar o glide quando a ação for cancelada

        slowTime = playerActionsAsset.Player.SlowTime; // Configuração da ação de SlowTime
        slowTime.performed += ctx => ToggleSlowTime(); // Iniciar ou parar o SlowTime quando a ação for realizada

        playerActionsAsset.Player.Jump.started += OnJumpStarted;
        playerActionsAsset.Player.Jump.canceled += OnJumpCanceled;
        playerActionsAsset.Player.Run.performed += ToggleRun;
        playerActionsAsset.Player.Attack.started += DoAttack;

        playerActionsAsset.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Disable();
    }

    private void FixedUpdate()
    {
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

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            return true;
        else
            return false;
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        Debug.Log("atacou!");
    }

    private void ToggleSlowTime()
    {
        Debug.Log("Slow Time atual: " + (Time.timeScale));
        if (Time.timeScale == 1.0f) // Se a escala de tempo estiver normal (1.0f), aplique a escala de tempo lenta
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajuste o tempo fixo em conformidade

            // Ajuste a intensidade da vinheta para 0.4f durante o SlowTime
            vignette.intensity.Override(0.4f);
        }
        else // Caso contrário, restaure a escala de tempo normal
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f; // Restaure o tempo fixo padrão

            // Redefina a intensidade da vinheta para 0 durante o tempo normal
            vignette.intensity.Override(0f);
        }
    }
}
