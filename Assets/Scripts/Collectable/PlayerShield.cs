using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Shield Timing")]
    public bool shieldActive = false;
    [SerializeField] private float activeDuration = 5f;
    [SerializeField] private float respawnDelay = 5f;

    [Header("UI")]
    public float shieldTimer;
    public TextMeshProUGUI shieldTimerText;

    [Header("Tags")]
    [SerializeField] private string shieldPickupTag = "Shield";
    [SerializeField] private string damageTagUpper = "Damage";
    [SerializeField] private string damageTagLower = "";

    private Coroutine shieldRoutine;
    private readonly HashSet<GameObject> respawningPickups = new HashSet<GameObject>();

    private void Update()
    {
        if (!shieldActive)
        {
            return;
        }

        shieldTimer -= Time.deltaTime;
        if (shieldTimerText != null)
        {
            shieldTimerText.text = "Shield: " + Mathf.Ceil(shieldTimer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!shieldActive || ShouldIgnoreCollider(other))
        {
            return;
        }

        if (HasDamageTag(other))
        {
            other.gameObject.SetActive(false);
        }
    }

    private void HandleTrigger(Collider other)
    {
        if (other == null || ShouldIgnoreCollider(other))
        {
            return;
        }

        if (HasTag(other, shieldPickupTag))
        {
            if (respawningPickups.Contains(other.gameObject))
            {
                return;
            }

            respawningPickups.Add(other.gameObject);
            ActivateShield(activeDuration);
            other.gameObject.SetActive(false);
            StartCoroutine(RespawnPickup(other.gameObject, respawnDelay));
            return;
        }

        if (shieldActive && HasDamageTag(other))
        {
            other.gameObject.SetActive(false);
        }
    }

    public void ActivateShield(float duration)
    {
        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
        }

        shieldActive = true;
        shieldTimer = duration;
        shieldRoutine = StartCoroutine(ShieldCountdown(duration));
    }

    private IEnumerator ShieldCountdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        shieldActive = false;
        shieldTimer = 0f;

        if (shieldTimerText != null)
        {
            shieldTimerText.text = string.Empty;
        }

        shieldRoutine = null;
    }

    private IEnumerator RespawnPickup(GameObject pickup, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (pickup != null)
        {
            pickup.SetActive(true);
        }

        respawningPickups.Remove(pickup);
    }

    private bool HasTag(Collider other, string tagName)
    {
        if (other == null || string.IsNullOrEmpty(tagName))
        {
            return false;
        }

        return other.gameObject.tag == tagName;
    }

    private bool HasDamageTag(Collider other)
    {
        return HasTag(other, damageTagUpper)
            || (!string.IsNullOrEmpty(damageTagLower) && HasTag(other, damageTagLower));
    }

    private bool ShouldIgnoreCollider(Collider other)
    {
        return HasTag(other, "Gems")
            || HasTag(other, "Checkpoint")
            || HasTag(other, "HealthPickUp");
    }
}
