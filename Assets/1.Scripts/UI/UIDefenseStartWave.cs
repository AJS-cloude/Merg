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
        if (startButton != null)
            startButton.onClick.AddListener(OnStartWave);
    }

    void OnStartWave()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.StartNextWave();
        if (buttonLabel != null)
            buttonLabel.text = "Next Wave";
    }
}
