using ECM.Components;
using ECM.Examples;
using Level_Events;
using UnityEngine;

public class SeamlessTeleporter : MonoBehaviour
{
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerRb = other.GetComponent<Rigidbody>();
            var playerMovement = other.GetComponent<NewPlayerController>();
            var playerController = other.GetComponent<CharacterMovement>();

            if (playerRb != null && playerMovement != null)
            {
                TeleportationManager.TeleportPlayer(other.transform, teleportTarget, playerRb, playerMovement, playerController);
            }
        }
    }
}