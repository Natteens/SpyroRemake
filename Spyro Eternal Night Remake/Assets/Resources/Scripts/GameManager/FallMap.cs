using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class FallMap : MonoBehaviour
{
    public Transform player; 
    public CinemachineFreeLook freeLookCamera; 
    public LayerMask fallLayer; 
    public float restartDelay = 2.0f; 

    private bool isFalling = false;

    private void OnTriggerEnter(Collider other)
    {
        if ((fallLayer.value & 1 << other.gameObject.layer) != 0)
        {
            freeLookCamera.m_Follow = null;
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
            if (!isFalling)
            {
                isFalling = true;
                Invoke("RestartScene", restartDelay);
            }
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
