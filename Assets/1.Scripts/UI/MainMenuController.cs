using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>0.Main 로비에서 게임 씬 진입 및 추후 설정/상점 연결용.</summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] string gameSceneName = "2.Game";
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button shopButton;

    void Awake()
    {
        if (playButton == null || settingsButton == null || shopButton == null)
            WireButtonsFromBtnsContainer();

        if (playButton != null)
            playButton.onClick.AddListener(LoadGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettingsPlaceholder);
        if (shopButton != null)
            shopButton.onClick.AddListener(OpenShopPlaceholder);
    }

    void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(LoadGame);
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OpenSettingsPlaceholder);
        if (shopButton != null)
            shopButton.onClick.RemoveListener(OpenShopPlaceholder);
    }

    void WireButtonsFromBtnsContainer()
    {
        var btns = transform.Find("BG/Btns");
        if (btns == null || btns.childCount < 3)
            return;

        playButton = btns.GetChild(0).GetComponent<Button>();
        settingsButton = btns.GetChild(1).GetComponent<Button>();
        shopButton = btns.GetChild(2).GetComponent<Button>();
    }

    void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenSettingsPlaceholder()
    {
        Debug.Log("[MainMenu] Settings — UI 패널 또는 별도 씬을 연결하세요.");
    }

    void OpenShopPlaceholder()
    {
        Debug.Log("[MainMenu] Shop — UI 패널 또는 별도 씬을 연결하세요.");
    }
}
