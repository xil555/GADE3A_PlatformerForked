using UnityEngine;

public class UIListener : MonoBehaviour
{
    [SerializeField] private InventoryUI ui;

    private void OnEnable()
    {
        PlayerStatsManager.OnStatsChanged += ui.UpdateUI;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnStatsChanged -= ui.UpdateUI;
    }
}
