using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Advanced";

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
