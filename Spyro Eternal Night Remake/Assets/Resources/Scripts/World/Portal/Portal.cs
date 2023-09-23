using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    public Transform destination;
    public float teleportDelay = 2.0f;
    public float portalDuration = 3.0f;
    public Collider triggerCollider;
    public VisualEffect VFX;
    public Transform TPcenter; 

    private bool playerInsidePortal = false;
    private GameObject playerObject;
    private Vector3 initialPlayerPosition;
    private Character playerCharacter;
    private PlayerInput playerInput;
    private CharacterController charController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInsidePortal)
        {
            playerInsidePortal = true;
            playerObject = other.gameObject;
            initialPlayerPosition = playerObject.transform.position;
            playerCharacter = playerObject.GetComponent<Character>();
            playerCharacter.playerActionsAsset.Disable();
            playerInput = playerObject.GetComponent<PlayerInput>();
            playerInput.DeactivateInput();
            charController = playerObject.GetComponent<CharacterController>();
            charController.enabled = false;
            CentralizePlayer(); 
            StartCoroutine(TeleportPlayer());
        }
    }

    private void CentralizePlayer()
    {
      playerObject.transform.position = TPcenter.position;
    }

    private IEnumerator TeleportPlayer()
    {
        VFX.Play();

        yield return new WaitForSeconds(teleportDelay);
        yield return new WaitForSeconds(portalDuration);

        if (destination != null && playerObject != null)
        {
            playerObject.transform.position = destination.position;
        }

        yield return new WaitForSeconds(0.5f);

        playerCharacter.playerActionsAsset.Enable();

        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }

        if (charController != null)
        {
            charController.enabled = true;
        }

        VFX.Stop();
        playerInsidePortal = false;
    }
}
