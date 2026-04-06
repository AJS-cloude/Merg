using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Canvas 안의 드롭 허용 영역(<b>DropPlace</b> 등). 포인터가 이 Rect 안에 있을 때만 놓기·합성을 허용하고,
/// 피벗이 영역 안에 있는 <see cref="DraggableUIImage"/> 개수로 상한을 둡니다.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIDropZone : MonoBehaviour
{
    [SerializeField] [Min(1)] int _maxDraggables = 10;

    RectTransform _rect;
    Canvas _rootCanvas;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
    }

    public int MaxDraggables => _maxDraggables;

    public RectTransform DropAreaRect => _rect;

    Camera GetCamera(PointerEventData eventData)
    {
        if (_rootCanvas != null && _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        if (eventData != null && eventData.pressEventCamera != null)
            return eventData.pressEventCamera;

        return _rootCanvas != null ? _rootCanvas.worldCamera : null;
    }

    public bool ContainsScreenPoint(Vector2 screenPosition, PointerEventData eventData)
    {
        var cam = GetCamera(eventData);
        return RectTransformUtility.RectangleContainsScreenPoint(_rect, screenPosition, cam);
    }

    /// <summary>활성 DraggableUIImage 중 피벗 화면 좌표가 이 영역 안에 있는 개수.</summary>
    public int CountDraggablesInside(PointerEventData eventData)
    {
        var cam = GetCamera(eventData);
        int n = 0;

        var found = FindObjectsByType<DraggableUIImage>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < found.Length; i++)
        {
            var d = found[i];
            if (d == null || !d.isActiveAndEnabled)
                continue;

            var rt = d.GetComponent<RectTransform>();
            if (rt == null)
                continue;

            Vector2 sp = RectTransformUtility.WorldToScreenPoint(cam, rt.position);
            if (RectTransformUtility.RectangleContainsScreenPoint(_rect, sp, cam))
                n++;
        }

        return n;
    }

    /// <summary>현재 배치 기준으로 상한 이하면 true (같은 개수 허용).</summary>
    public bool IsAtOrUnderCapacity(PointerEventData eventData)
    {
        return CountDraggablesInside(eventData) <= _maxDraggables;
    }
}
