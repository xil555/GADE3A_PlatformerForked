using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static readonly List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    public string playerTag = "Player";
    [SerializeField] private ResetTrigger resetTrigger;

    private bool isActivated;

    private void OnEnable()
    {
        if (!allCheckpoints.Contains(this))
        {
            allCheckpoints.Add(this);
        }
    }

    private void OnDisable()
    {
        allCheckpoints.Remove(this);
    }

    public static void ResetAll()
    {
        for (int i = 0; i < allCheckpoints.Count; i++)
        {
            if (allCheckpoints[i] != null)
            {
                allCheckpoints[i].isActivated = false;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isActivated || resetTrigger == null)
        {
            return;
        }

        if (!other.CompareTag(playerTag) && !other.transform.root.CompareTag(playerTag))
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player == null)
        {
            return;
        }

        player.SaveCheckpoint(transform.position);

        resetTrigger.SetSpawnPoint(transform.position);

        isActivated = true;
    }
}
