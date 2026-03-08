using UnityEngine;

/// <summary>
/// 노치/라운드 코너/시스템 UI를 피해 UI가 안전 영역 안에 오도록 RectTransform을 조정합니다.
/// 상단/하단 바 등에 붙이면 됩니다.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaAnchor : MonoBehaviour
{
    [Header("적용할 방향")]
    [SerializeField] bool applyLeft = true;
    [SerializeField] bool applyRight = true;
    [SerializeField] bool applyTop = true;
    [SerializeField] bool applyBottom = true;

    RectTransform _rect;
    Rect _lastSafeArea;
    Vector2 _lastResolution;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.width != _lastResolution.x || Screen.height != _lastResolution.y
            || Screen.safeArea != _lastSafeArea)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        if (_rect == null) return;

        _lastSafeArea = Screen.safeArea;
        _lastResolution = new Vector2(Screen.width, Screen.height);

        Rect safe = _lastSafeArea;
        float w = Screen.width;
        float h = Screen.height;

        Vector2 anchorMin = _rect.anchorMin;
        Vector2 anchorMax = _rect.anchorMax;

        if (applyLeft)  anchorMin.x = safe.xMin / w;
        if (applyRight) anchorMax.x = safe.xMax / w;
        if (applyBottom) anchorMin.y = safe.yMin / h;
        if (applyTop)    anchorMax.y = safe.yMax / h;

        _rect.anchorMin = anchorMin;
        _rect.anchorMax = anchorMax;
    }
}
