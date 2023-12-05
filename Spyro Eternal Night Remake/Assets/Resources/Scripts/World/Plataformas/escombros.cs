using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escombros : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public float translationSpeed = 2.0f;
    public float maxRadius = 10.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        Vector3 newPosition = transform.position + transform.forward * translationSpeed * Time.deltaTime;
        if (Vector3.Distance(Vector3.zero, newPosition) <= maxRadius)
        {
            transform.Translate(Vector3.forward * translationSpeed * Time.deltaTime);
        }
    }
}
