using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState
{
    public enum State
    {
        Idle,
        Walk,
        Jump,
        Glide,
        Dash,
        SlowTime
    }

    public State state;
    protected PlayerController playerController;

    public abstract void Enter(PlayerController player);
    public abstract void Update(PlayerController player);
    public abstract void Exit(PlayerController player);
}

public class IdleState : PlayerState
{
    public override void Enter(PlayerController player)
    {
        Debug.Log("Entrou no Idle");
    }

    public override void Update(PlayerController player)
    {

        if (player.Move.ReadValue<Vector2>().magnitude >= 0.1f)
        {
            player.ChangeState(new WalkState());
        }

        if (player.Jump.triggered)
        {
            player.ChangeState(new JumpState());
        }

    }

    public override void Exit(PlayerController player)
    {
      
    }
}

public class WalkState : PlayerState
{
    private Transform cam;
    private CharacterController controller;
    private float speed;
    private float turnSmothTime;
    private float turnSmoothVelocity;
    public override void Enter(PlayerController player)
    {
        speed = player.Speed;
        turnSmothTime = player.TurnSmoothTime;
        turnSmoothVelocity = player.TurnSmoothVelocity;
        cam = player.Cam;
        controller = player.Controller;
        state = State.Walk;
        Debug.Log("Entrou no Walk");
    }

    public override void Update(PlayerController player)
    {
        
        Vector2 moveInput = player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmothTime);
            player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }


        if (player.Jump.triggered)
        {
            player.ChangeState(new JumpState());
        }
    }

    public override void Exit(PlayerController player)
    {
       
    }
}

public class JumpState : PlayerState
{
    private int jumpHeight;
    private int numJumps;
    private int currentJumpCount = 0;
    private float gravity;
    private Vector3 velocity;

    private Transform cam;
    private CharacterController controller;
    private float speed;
    private float turnSmothTime;
    private float turnSmoothVelocity;

    public override void Enter(PlayerController player)
    {
        jumpHeight = player.JumpHeight;
        numJumps = player.NumJumps;
        currentJumpCount = 0;
        gravity = player.Gravity;
        speed = player.Speed;
        turnSmothTime = player.TurnSmoothTime;
        turnSmoothVelocity = player.TurnSmoothVelocity;
        cam = player.Cam;
        controller = player.Controller;
        Debug.Log("Entrou no Jump");
    }

    public override void Update(PlayerController player)
    {
        Vector2 moveInput = player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmothTime);
            player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;

        // Depuração: Verifique se o jogador está no chão.
        if (player.IsGrounded())
        {
            Debug.Log("No chão");
            currentJumpCount = 0;

            if (player.Jump.triggered && currentJumpCount < numJumps)
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * -gravity);
                currentJumpCount++;
            }

            // Transição para o estado Idle após o pulo.
            player.ChangeState(new IdleState());
        }
        else
        {
            Debug.Log("No ar");
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public override void Exit(PlayerController player)
    {

    }
}

