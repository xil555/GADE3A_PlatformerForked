using UnityEngine;
using System;


public class PlayerStatsManager : MonoBehaviour
{
    public PlayerStats stats = new PlayerStats();

    public static Action<PlayerStats> OnStatsChanged;

    public void AddScore(int amount)
    {
        stats.score += amount;
        OnStatsChanged?.Invoke(stats);
    }

    public void AddCoins(int amount)
    {
        stats.coins += amount;
        OnStatsChanged?.Invoke(stats);
    }

    public void LoseLife()
    {
        stats.lives--;
        OnStatsChanged?.Invoke(stats);
    }
}
