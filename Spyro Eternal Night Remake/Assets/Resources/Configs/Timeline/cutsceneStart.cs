using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutsceneStart : MonoBehaviour
{
    public GameObject main;
    public Character p;
    public byte t = 15;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            p.InIdleMode = true;
            main.SetActive(true);
            StartCoroutine(inCutscene());
        }
    }

    IEnumerator inCutscene()
    {
        yield return new WaitForSeconds(t);
        Destroy(main);
        p.InIdleMode = false;
    }
}
