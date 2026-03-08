using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Canvas에 Canvas Scaler를 "Scale With Screen Size"로 한 번에 설정합니다.
/// 메뉴: Tools > Merge Factory > Setup Canvas For Screen Ratio
/// </summary>
public static class CanvasScalerSetup
{
    const float DefaultRefWidth = 1080f;
    const float DefaultRefHeight = 1920f;
    const float DefaultMatch = 0.5f;

    [MenuItem("Tools/Merge Factory/Setup Canvas For Screen Ratio")]
    public static void SetupSelectedCanvas()
    {
        var canvas = Selection.activeGameObject?.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Canvas가 있는 오브젝트를 선택한 뒤 다시 실행하세요.");
            return;
        }

        var scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(DefaultRefWidth, DefaultRefHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = DefaultMatch;
        scaler.referencePixelsPerUnit = 100f;
        scaler.dynamicPixelsPerUnit = 100f;

        var uiScaler = canvas.GetComponent<UIScreenScaler>();
        if (uiScaler == null)
            canvas.gameObject.AddComponent<UIScreenScaler>();

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("Canvas Scaler 설정 완료. 기준 해상도 " + DefaultRefWidth + "x" + DefaultRefHeight + ", Match = " + DefaultMatch);
    }

    [MenuItem("Tools/Merge Factory/Setup Canvas For Screen Ratio", true)]
    static bool ValidateSetup()
    {
        return Selection.activeGameObject != null;
    }
}
