using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string Menu;

    public GameObject pauseMenuUI;
    public GameObject HudHP;

    private bool isPaused = false;

    private Character character;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        HudHP.SetActive(true);
        Time.timeScale = 1f; 
        isPaused = false;
        character.InIdleMode = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        HudHP.SetActive(false);
        Time.timeScale = 0f; 
        isPaused = true;
        character.InIdleMode = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
         SceneManager.LoadScene(Menu);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
