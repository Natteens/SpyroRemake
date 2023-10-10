using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaCircular : MonoBehaviour
{
    public float raio = 5.0f;
    public float velocidade = 1.0f;
    private Vector3 centro;

    void Start()
    {
        centro = transform.position;
    }

    private void Update()
    {
        Orbitar();
    }

    void Orbitar()
    {
        float angulo = Time.time * velocidade;
        float x = centro.x + raio * Mathf.Cos(angulo);
        float z = centro.z + raio * Mathf.Sin(angulo);
        transform.position = new Vector3(x, transform.position.y, z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 newPosition = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            other.transform.position = newPosition;
            Vector3 relativePosition = other.transform.position - transform.position;
            float angle = Mathf.Atan2(relativePosition.x, relativePosition.z) * Mathf.Rad2Deg;
            other.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            other.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
    }
}
