using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private static readonly List<Coins> registeredPickups = new List<Coins>();

    private void Awake()
    {
        Register(this);
    }

    private void OnDestroy()
    {
        registeredPickups.Remove(this);
    }

    private static void Register(Coins pickup)
    {
        if (pickup == null || registeredPickups.Contains(pickup))
        {
            return;
        }

        registeredPickups.Add(pickup);
    }

    /// <summary>
    /// Call when the player loses all lives: re-enables every gem that was collected (disabled).
    /// </summary>
    public static void RestoreAllPickups()
    {
        for (int i = 0; i < registeredPickups.Count; i++)
        {
            Coins c = registeredPickups[i];
            if (c != null)
            {
                c.ResetForRespawn();
            }
        }
    }

    private bool collected;

    public void ResetForRespawn()
    {
        collected = false;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || other == null)
        {
            return;
        }

        PlayerInventory inventory = ResolvePlayerInventory(other);
        if (inventory != null)
        {
            collected = true;

            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlaySound("Gem");
            }

            inventory.CoinCollection();
            gameObject.SetActive(false);
        }
    }

    private static PlayerInventory ResolvePlayerInventory(Component other)
    {
        PlayerInventory inv = other.GetComponentInParent<PlayerInventory>();
        if (inv != null)
        {
            return inv;
        }

        return other.transform.root.GetComponent<PlayerInventory>();
    }
}
