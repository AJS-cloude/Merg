using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GDD: 게임 전체 라이프사이클. 매니저 초기화, 씬/플로우.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] GameConfig config;
    [SerializeField] ItemData[] allItemData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (SaveManager.Instance != null && allItemData != null && allItemData.Length > 0)
            SaveManager.Instance.SetAllItemData(allItemData);
    }

    void Start()
    {
        // 첫 실행 시 재화는 SaveManager에서 로드하거나, 신규면 config 기준
        if (EconomyManager.Instance != null && !PlayerPrefs.HasKey("MergeFactory_Gold"))
            EconomyManager.Instance.ApplyStartingCurrencies();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
