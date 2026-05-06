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
        HandleTrigger(other);
    }

    private void HandleTrigger(Collider other)
    {
        if (other == null)
        {
            return;
        }

        if (other.CompareTag("Shield"))
        {
            if (respawningPickups.Contains(other.gameObject))
            {
                return;
            }

            ActivateShield(activeDuration);
            StartCoroutine(RespawnPickup(other.gameObject, respawnDelay));
            return;
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
        respawningPickups.Add(pickup);
        pickup.SetActive(false);
        yield return new WaitForSeconds(delay);
        pickup.SetActive(true);
        respawningPickups.Remove(pickup);
    }
}