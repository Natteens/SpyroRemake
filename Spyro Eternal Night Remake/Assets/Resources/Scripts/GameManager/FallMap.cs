using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class FallMap : MonoBehaviour
{
    public Transform player;
    public CinemachineFreeLook freeLookCamera;
    public LayerMask fallLayer;
    public float restartDelay = 2.0f;
    public float fallTimeThreshold = 3.0f; // Limiar de tempo para considerar uma queda
    public float fallHeightThreshold = -25.0f; // Altura para considerar uma queda

    private bool isFalling = false;
    private float fallTimer = 0f;

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
            }
        }
    }

    private void Update()
    {
        if (isFalling)
        {
            fallTimer += Time.deltaTime;

            // Adicione a lógica aqui para verificar se o jogador está caindo por mais de alguns segundos
            if (fallTimer >= fallTimeThreshold)
            {
                RestartScene();
            }

            // Verificar a altura do jogador
            if (player.position.y <= fallHeightThreshold)
            {
                RestartScene();
            }
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
