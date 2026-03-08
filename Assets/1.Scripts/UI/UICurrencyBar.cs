using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GDD: 상단 재화 표시 (Gold, Gems).
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UICurrencyBar : MonoBehaviour
{
    [SerializeField] Text goldText;
    [SerializeField] Text gemsText;

    void Start()
    {
        if (EconomyManager.Instance == null) return;
        EconomyManager.Instance.OnGoldChanged += OnGoldChanged;
        EconomyManager.Instance.OnGemsChanged += OnGemsChanged;
        OnGoldChanged(EconomyManager.Instance.Gold);
        OnGemsChanged(EconomyManager.Instance.Gems);
    }

    void OnDestroy()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnGoldChanged -= OnGoldChanged;
            EconomyManager.Instance.OnGemsChanged -= OnGemsChanged;
        }
    }

    void OnGoldChanged(int value)
    {
        if (goldText != null) goldText.text = value.ToString("N0");
    }

    void OnGemsChanged(int value)
    {
        if (gemsText != null) gemsText.text = value.ToString("N0");
    }
}
