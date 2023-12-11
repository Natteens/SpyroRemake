using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class ReceivePowers : MonoBehaviour
{
    public GameObject Main;

    public VisualEffect vfx;

    public Transform centralPoint; 
    public GameObject[] objectsToActivate; 
    public float activationDelay = 1.0f;

     public GameObject playerObject;
     public Character playerCharacter;
     public PlayerInput playerInput;
     public CharacterController charController;
     public Status StatusBuff;
     public Vector3 initialPlayerPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerObject = other.gameObject;
            playerCharacter = playerObject.GetComponent<Character>();
            playerInput = playerObject.GetComponent<PlayerInput>();
            charController = playerObject.GetComponent<CharacterController>();
            StatusBuff = playerObject.GetComponent<Status>();

            initialPlayerPosition = playerObject.transform.position;
            playerCharacter.playerActionsAsset.Disable();
            playerInput.DeactivateInput();
         
            CentralizePlayer();
            StartCoroutine(SubindoPlataformas());           
        }
    }

    private void CentralizePlayer()
    {
        playerObject.transform.position = centralPoint.position;
        vfx.Play();
    }

    private IEnumerator SubindoPlataformas()
    {
        playerCharacter.InIdleMode = true;
        charController.enabled = false;

        foreach (GameObject platform in objectsToActivate)
        {
            yield return new WaitForSeconds(activationDelay);
        }
        playerCharacter.playerActionsAsset.Enable();
        playerInput.ActivateInput();
       

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        yield return new WaitForSeconds(activationDelay);
        charController.enabled = true;
        playerCharacter.InIdleMode = false;

        StatusBuff.currentMana = StatusBuff.maxMana;
        playerCharacter.receivePowers = true;
        Destroy(Main);
    }
}
