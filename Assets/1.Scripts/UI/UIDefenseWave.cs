using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디펜스: 현재 웨이브 표시.
/// </summary>
public class UIDefenseWave : MonoBehaviour
{
    [SerializeField] Text waveText;
    [SerializeField] string format = "Wave {0}";

    void Start()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += OnWaveStarted;
            Refresh(WaveManager.Instance.CurrentWaveIndex);
        }
    }

    void OnDestroy()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
    }

    void OnWaveStarted(int index)
    {
        Refresh(index);
    }

    void Refresh(int index)
    {
        if (waveText != null)
            waveText.text = string.Format(format, index + 1);
    }
}
