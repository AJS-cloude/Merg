using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디펜스: 기지 HP 바.
/// </summary>
public class UIDefenseBaseHP : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] Image fillImage;
    [SerializeField] Text hpText;
    [SerializeField] Color fullColor = Color.green;
    [SerializeField] Color lowColor = Color.red;

    void Start()
    {
        if (BaseHealthManager.Instance == null)
        {
            if (hpSlider != null) hpSlider.gameObject.SetActive(false);
            if (fillImage != null) fillImage.enabled = false;
            if (hpText != null) hpText.text = "";
            enabled = false;
            return;
        }
        BaseHealthManager.Instance.OnHpChanged += OnHpChanged;
        OnHpChanged(BaseHealthManager.Instance.CurrentHp, BaseHealthManager.Instance.MaxHp);
    }

    void OnDestroy()
    {
        if (BaseHealthManager.Instance != null)
            BaseHealthManager.Instance.OnHpChanged -= OnHpChanged;
    }

    void OnHpChanged(int current, int max)
    {
        if (hpSlider != null && max > 0)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = max;
            hpSlider.value = current;
        }
        if (fillImage != null)
            fillImage.color = max > 0 ? Color.Lerp(lowColor, fullColor, (float)current / max) : fullColor;
        if (hpText != null)
            hpText.text = current + " / " + max;
    }
}
