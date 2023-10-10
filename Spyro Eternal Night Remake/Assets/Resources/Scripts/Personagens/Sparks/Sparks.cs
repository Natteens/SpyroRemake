using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5.0f;
    public float smoothness = 0.1f;
    public float xOffset = 3.0f;
    public float yOffset = 3.0f;
    public float zOffset = -4.0f;
    public float hoverAmplitude = 0.5f;

    private Rigidbody rb;
    private Vector3 initialOffset;

    private void Start()
    {
        initialOffset = new Vector3(xOffset, yOffset, zOffset);
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = player.position;
        Vector3 targetPosition = playerPosition + player.right * xOffset + player.up * (yOffset + Mathf.Sin(Time.time) * hoverAmplitude) + player.forward * initialOffset.z;
        rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, smoothness * Time.fixedDeltaTime));
    }
}
