using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디펜스: 게임 오버 패널. 기지 HP 0 시 표시.
/// </summary>
public class UIDefenseGameOver : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text messageText;
    [SerializeField] Button retryButton;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (BaseHealthManager.Instance == null)
        {
            if (retryButton != null) retryButton.gameObject.SetActive(false);
            enabled = false;
            return;
        }
        BaseHealthManager.Instance.OnGameOver += Show;
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);
    }

    void OnDestroy()
    {
        if (BaseHealthManager.Instance != null)
            BaseHealthManager.Instance.OnGameOver -= Show;
    }

    void Show()
    {
        if (panel != null) panel.SetActive(true);
        if (messageText != null) messageText.text = "Game Over";
    }

    void OnRetry()
    {
        if (panel != null) panel.SetActive(false);
        if (BaseHealthManager.Instance != null) BaseHealthManager.Instance.ResetHp();
        if (WaveManager.Instance != null) WaveManager.Instance.ResetWaves();
        if (GameManager.Instance != null) GameManager.Instance.RestartGame();
    }
}
