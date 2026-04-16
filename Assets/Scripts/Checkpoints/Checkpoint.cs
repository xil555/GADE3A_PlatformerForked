using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string playerTag = "Player";
    [SerializeField] private ResetTrigger resetTrigger;

    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated || !other.CompareTag(playerTag) || resetTrigger == null)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SaveCheckpoint(transform.position);
        }

        resetTrigger.SetSpawnPoint(transform.position);

        isActivated = true;
    }
}