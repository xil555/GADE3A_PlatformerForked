using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class LoadNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Advanced";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 🔥 Disable PlayerInput BEFORE scene change
        PlayerInput input = other.GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
        }

        // 🔥 Destroy the old player completely
        Destroy(other.gameObject);

        // Reset time
        Time.timeScale = 1f;

        // Load next scene cleanly
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }

}
