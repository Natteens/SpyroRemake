using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    public GameManager gm;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            gm.isThx = true;
    }
}
