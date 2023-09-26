using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class FallMap : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public CinemachineFreeLook freeLookCamera; // Referência à FreeLook Camera do Cinemachine
    public LayerMask fallLayer; // A camada que representa a queda no Inspector
    public float restartDelay = 2.0f; // Tempo de atraso antes de reiniciar a cena

    private bool isFalling = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifique se o jogador entrou em um collider da camada de queda
        if ((fallLayer.value & 1 << other.gameObject.layer) != 0)
        {
            // Pare de seguir o jogador e desabilite o controle do mouse na câmera
            freeLookCamera.m_Follow = null;
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";

            // Inicie o processo de reinício da cena após um atraso
            if (!isFalling)
            {
                isFalling = true;
                Invoke("RestartScene", restartDelay);
            }
        }
    }

    private void RestartScene()
    {
        // Reinicie a cena do jogo após o atraso especificado
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
