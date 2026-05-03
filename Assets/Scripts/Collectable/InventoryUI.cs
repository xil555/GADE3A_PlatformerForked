using TMPro;
using UnityEngine;
using System;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    private void Awake()
    {
        CacheUI();
    }

    private void CacheUI()
    {
        // Auto-assign if not set in Inspector (safe fallback)
        if (coinsText == null)
            coinsText = transform.Find("CoinsText")?.GetComponent<TextMeshProUGUI>();

        if (scoreText == null)
            scoreText = transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();

        if (livesText == null)
            livesText = transform.Find("LivesText")?.GetComponent<TextMeshProUGUI>();
    }

    // EVENT-DRIVEN UPDATE (Advanced approach)
    public void UpdateUI(PlayerStats stats)
    {
        if (stats == null) return;

        if (coinsText != null)
            coinsText.text = $"Coins: {stats.coins}";

        if (scoreText != null)
            scoreText.text = $"Score: {stats.score}";

        if (livesText != null)
            livesText.text = $"Lives: {stats.lives}";
    }
}
