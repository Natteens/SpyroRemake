using System.Collections;
using UnityEngine;

public class PlataformAndPlayer : MonoBehaviour
{
    public byte speed = 4;

    private void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
