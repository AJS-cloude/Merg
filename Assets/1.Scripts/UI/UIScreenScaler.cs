using UnityEngine;

/// <summary>
/// 화면 비율과 해상도가 바뀌어도 UI가 항상 동일한 비율로 스케일되도록 Canvas를 설정합니다.
/// Canvas 루트에 붙이고, Canvas Scaler는 "Scale With Screen Size"로 두면 이 스크립트가 기준 해상도를 적용합니다.
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(UnityEngine.UI.CanvasScaler))]
public class UIScreenScaler : MonoBehaviour
{
    [Header("기준 해상도 (Reference Resolution)")]
    [Tooltip("모바일 세로 기준: 1080 x 1920. 이 비율을 기준으로 다른 해상도에서도 동일한 비율로 스케일됩니다.")]
    [SerializeField] Vector2 referenceResolution = new Vector2(1080f, 1920f);

    [Header("스케일 방식 (Match Width Or Height)")]
    [Range(0f, 1f)]
    [Tooltip("0 = 너비 기준(좁은 화면), 1 = 높이 기준(넓은 화면), 0.5 = 균형. 세로 UI는 보통 0.5~1 권장.")]
    [SerializeField] float matchWidthOrHeight = 0.5f;

    [Header("기타")]
    [SerializeField] float referencePixelsPerUnit = 100f;
    [SerializeField] float dynamicPixelsPerUnit = 100f;

    Canvas _canvas;
    UnityEngine.UI.CanvasScaler _scaler;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _scaler = GetComponent<UnityEngine.UI.CanvasScaler>();
        Apply();
    }

    void Apply()
    {
        if (_scaler == null) return;

        _scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _scaler.referenceResolution = referenceResolution;
        _scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        _scaler.matchWidthOrHeight = matchWidthOrHeight;
        _scaler.referencePixelsPerUnit = referencePixelsPerUnit;
        _scaler.dynamicPixelsPerUnit = dynamicPixelsPerUnit;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying && _scaler != null)
            Apply();
    }
#endif
}
