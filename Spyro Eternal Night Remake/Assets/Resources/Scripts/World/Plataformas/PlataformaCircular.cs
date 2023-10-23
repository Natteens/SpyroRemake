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

    private void FixedUpdate()
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
       other.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
       other.transform.parent = null;
    }
}
