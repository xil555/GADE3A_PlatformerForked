using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public int noOfCoins = 0;

    [SerializeField] private TextMeshProUGUI gemCounterText;

    private void Awake()
    {
        // Singleton (prevents reset between scenes)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void CoinCollection()
    {
        noOfCoins++;
        RefreshUI();
    }

    public void ResetCoins()
    {
        noOfCoins = 0;
        RefreshUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TextMeshProUGUI[] allTexts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);

        foreach (TextMeshProUGUI txt in allTexts)
        {
            if (txt.CompareTag("GemText")) 
            {
                gemCounterText = txt;
                break;
            }
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (gemCounterText != null)
        {
            gemCounterText.text = noOfCoins.ToString();
        }
    }
}
