using UnityEngine;

public class ObjetoVoando : MonoBehaviour
{
    public float hoverAmplitude = 0.5f;
    public float smoothness = 0.1f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float verticalOffset = Mathf.Sin(Time.time * smoothness) * hoverAmplitude;
        transform.position = initialPosition + new Vector3(0f, verticalOffset, 0f);
    }
}
