using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디펜스: "웨이브 시작" 버튼.
/// </summary>
public class UIDefenseStartWave : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Text buttonLabel;

    void Start()
    {
        bool hasWave = WaveManager.Instance != null;
        if (startButton != null)
        {
            startButton.interactable = hasWave;
            if (hasWave)
                startButton.onClick.AddListener(OnStartWave);
        }
        if (buttonLabel != null && !hasWave)
            buttonLabel.text = "";
    }

    void OnStartWave()
    {
        if (WaveManager.Instance == null) return;
        WaveManager.Instance.StartNextWave();
        if (buttonLabel != null)
            buttonLabel.text = "Next Wave";
    }
}
