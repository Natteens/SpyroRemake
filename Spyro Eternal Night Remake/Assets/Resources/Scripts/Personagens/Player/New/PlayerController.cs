using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;
    private PlayerInputActions playerActionsAsset;

    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed;
    [SerializeField] private byte jumpHeight;
    [SerializeField] private byte numJumps;
    [SerializeField] private float gravity;
    [SerializeField] private Transform cam;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float turnSmoothVelocity;

    #region InputActions
    public InputAction Move { get; private set; }
    public InputAction Glide { get; private set; }
    public InputAction SlowTime { get; private set; }
    public InputAction Attack { get; private set; }
    public InputAction Dash { get; private set; }
    public InputAction Jump { get; private set; }
    #endregion
    #region Getter
    public float Speed => speed;
    public byte JumpHeight => jumpHeight;
    public byte NumJumps => numJumps;
    public float Gravity => gravity;
    public Transform Cam => cam;
    public float TurnSmoothTime => turnSmoothTime;
    public float TurnSmoothVelocity => turnSmoothVelocity;
    public CharacterController Controller => controller;
    #endregion

    private void Awake()
    {
        InitializeInputManager();
    }
    private void InitializeInputManager()
    {
        playerActionsAsset = new PlayerInputActions();
        Move = playerActionsAsset.Player.Move;
        Glide = playerActionsAsset.Player.Glide;
        SlowTime = playerActionsAsset.Player.SlowTime;
        Attack = playerActionsAsset.Player.Attack;
        Dash = playerActionsAsset.Player.Dash;
        Jump = playerActionsAsset.Player.Jump;
    }

    private void OnEnable()
    {
        InitializeInputManager();
        playerActionsAsset.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Disable();
    }

    private void Start()
    {
        ChangeState(new IdleState());
    }

    private void Update()
    {
        currentState.Update(this);
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            Debug.Log("no chao");
            return true;
        }
        else
        {
            Debug.Log("fora do chao");
            return false;
        }
            
    }
}
