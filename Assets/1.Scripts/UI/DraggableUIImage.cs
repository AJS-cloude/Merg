using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI <see cref="Image"/>를 드래그로 옮기고, 같은 스프라이트를 가진 다른 조각 위에 놓으면 한 단계 위 Body 스프라이트로 합칩니다.
/// <see cref="UIDropZone"/>이 있으면 그 Rect 안에서만 놓기·합성이 되고, 영역 안 조각 수는 상한(기본 10)을 넘기면 원위치로 돌아갑니다.
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
public class DraggableUIImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] bool bringToFrontWhileDragging = true;

    [Tooltip("비우면 Resources 이름 \"BodySpriteMergeCatalog\" 로 로드를 시도합니다.")]
    [SerializeField] BodySpriteMergeCatalog _mergeCatalog;

    [Tooltip("비우면 Resources 이름 \"FaceSpriteCatalog\" 로 로드를 시도합니다.")]
    [SerializeField] FaceSpriteCatalog _faceSpriteCatalog;

    [Tooltip("얼굴 스프라이트를 넣을 자식 이름 (기본 Face).")]
    [SerializeField] string _faceImageChildName = "Face";

    [Tooltip("합성 후 비워진 슬롯 재소환. 비우면 씬에서 CharSpawner를 찾습니다.")]
    [SerializeField] CharSpawner _charSpawner;

    [Tooltip("비우면 씬에서 UIDropZone을 찾습니다. DropPlace 오브젝트에 붙입니다.")]
    [SerializeField] UIDropZone _dropZone;

    [Header("Drop 영역 배치")]
    [Tooltip("서로 다른 조각끼리 맞닿은 뒤 분리할 때, 가장자리 사이에 유지할 최소 여백.")]
    [SerializeField] float _minSeparationGap = 15f;

    [Header("시작 그래픽")]
    [Tooltip("플레이 시작 시 이 색의 squircle(1단)로 Image.sprite를 설정합니다.")]
    [SerializeField] BodyColorId _initialBodyColor = BodyColorId.Blue;

    [Tooltip("Awake에서 카탈로그로 squircle 스프라이트를 바로 적용합니다.")]
    [SerializeField] bool _applyInitialSquircleOnStart = true;

    Image _image;
    RectTransform _rect;
    RectTransform _parentRect;
    Canvas _rootCanvas;

    Vector2 _grabOffset;
    float _localZ;
    int _siblingIndexBeforeDrag;
    CanvasGroup _canvasGroup;

    Vector3 _homeLocalPosition;
    Quaternion _homeLocalRotation;
    Vector3 _homeLocalScale;
    bool _dragSessionActive;

    static readonly Vector3[] CornersBuffer = new Vector3[4];

    void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _parentRect = transform.parent as RectTransform;

        var canvas = GetComponentInParent<Canvas>();
        _rootCanvas = canvas != null ? canvas.rootCanvas : null;

        if (_mergeCatalog == null)
            _mergeCatalog = Resources.Load<BodySpriteMergeCatalog>("BodySpriteMergeCatalog");

        if (_faceSpriteCatalog == null)
            _faceSpriteCatalog = Resources.Load<FaceSpriteCatalog>("FaceSpriteCatalog");

        if (_applyInitialSquircleOnStart)
            ApplyInitialSquircle();

        ApplyRandomFaceFromCatalog();
    }

    void ApplyInitialSquircle()
    {
        if (Catalog == null || _image == null)
            return;

        if (!Catalog.TryGetSquircle(_initialBodyColor, out var squircle))
        {
            Debug.LogWarning(
                $"{nameof(DraggableUIImage)} on '{name}': '{_initialBodyColor}' squircle not in catalog (assign BodySpriteMergeCatalog & sprites).",
                this);
            return;
        }

        _image.sprite = squircle;
        _image.color = Color.white;
    }

    void ApplyRandomFaceFromCatalog()
    {
        if (_faceSpriteCatalog == null)
            return;

        var faceSprite = _faceSpriteCatalog.GetRandomFace();
        if (faceSprite == null)
            return;

        var faceT = transform.Find(_faceImageChildName);
        if (faceT == null)
            return;

        var faceImage = faceT.GetComponent<Image>();
        if (faceImage == null)
            return;

        faceImage.sprite = faceSprite;
        faceImage.color = Color.white;
    }

    BodySpriteMergeCatalog Catalog => _mergeCatalog;

    CharSpawner Spawner => _charSpawner != null
        ? _charSpawner
        : FindFirstObjectByType<CharSpawner>();

    UIDropZone DropZone => _dropZone != null
        ? _dropZone
        : FindFirstObjectByType<UIDropZone>();

    void SaveHomeTransform()
    {
        _homeLocalPosition = _rect.localPosition;
        _homeLocalRotation = _rect.localRotation;
        _homeLocalScale = _rect.localScale;
    }

    void RestoreHomeTransform()
    {
        _rect.localPosition = _homeLocalPosition;
        _rect.localRotation = _homeLocalRotation;
        _rect.localScale = _homeLocalScale;
    }

    void RestoreAfterCanceledDrop()
    {
        RestoreHomeTransform();

        if (bringToFrontWhileDragging)
            transform.SetSiblingIndex(_siblingIndexBeforeDrag);

        _canvasGroup.blocksRaycasts = true;
    }

    Camera GetEventCamera(PointerEventData eventData)
    {
        if (_rootCanvas != null && _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        if (eventData.pressEventCamera != null)
            return eventData.pressEventCamera;

        return _rootCanvas != null ? _rootCanvas.worldCamera : null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentRect = transform.parent as RectTransform;
        SaveHomeTransform();
        _dragSessionActive = false;

        if (_parentRect == null)
            return;

        if (bringToFrontWhileDragging)
        {
            _siblingIndexBeforeDrag = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }

        _canvasGroup.blocksRaycasts = false;

        var cam = GetEventCamera(eventData);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect, eventData.position, cam, out Vector2 localPointer))
        {
            _canvasGroup.blocksRaycasts = true;
            if (bringToFrontWhileDragging)
                transform.SetSiblingIndex(_siblingIndexBeforeDrag);
            return;
        }

        _grabOffset = (Vector2)_rect.localPosition - localPointer;
        _localZ = _rect.localPosition.z;
        _dragSessionActive = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragSessionActive || _parentRect == null)
            return;

        var cam = GetEventCamera(eventData);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect, eventData.position, cam, out Vector2 localPointer))
            return;

        _rect.localPosition = new Vector3(
            localPointer.x + _grabOffset.x,
            localPointer.y + _grabOffset.y,
            _localZ);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragSessionActive)
            return;

        _dragSessionActive = false;

        var zone = DropZone;

        if (zone != null && !zone.ContainsScreenPoint(eventData.position, eventData))
        {
            RestoreAfterCanceledDrop();
            return;
        }

        if (TryMergeOnDrop(eventData))
            return;

        if (zone != null && !zone.IsAtOrUnderCapacity(eventData))
        {
            RestoreAfterCanceledDrop();
            return;
        }

        if (zone != null)
        {
            Transform slotBefore = transform.parent;
            var spawner = Spawner;
            bool cameFromSpawnSlot = spawner != null && spawner.IsRegisteredSpawnPoint(slotBefore);

            if (cameFromSpawnSlot)
            {
                transform.SetParent(zone.DropAreaRect, true);
                transform.SetAsLastSibling();
                spawner.RefillSpawnSlot(slotBefore);
            }
            else if (bringToFrontWhileDragging)
            {
                transform.SetSiblingIndex(_siblingIndexBeforeDrag);
            }

            ResolveOverlapsInDropZone(zone);

            _canvasGroup.blocksRaycasts = true;
            return;
        }

        RestoreAfterCanceledDrop();
    }

    /// <summary>
    /// 방금 놓은 조각(<b>this</b>)이 같은 DropPlace 자식 조각과 겹치면, 이동 거리가 가장 짧은 방향으로 밀어 분리한 뒤 영역 안에 맞춥니다.
    /// </summary>
    void ResolveOverlapsInDropZone(UIDropZone zone)
    {
        var field = zone.DropAreaRect;
        if (field == null || transform.parent != field)
            return;

        const int maxSeparationPasses = 16;
        for (int pass = 0; pass < maxSeparationPasses; pass++)
        {
            Rect mine = AxisAlignedBoundsInParent(_rect, field);
            bool moved = false;

            for (int i = 0; i < field.childCount; i++)
            {
                var child = field.GetChild(i);
                if (child.gameObject == gameObject)
                    continue;

                if (child.GetComponent<DraggableUIImage>() == null)
                    continue;

                var otherRt = child.GetComponent<RectTransform>();
                if (otherRt == null)
                    continue;

                Rect other = AxisAlignedBoundsInParent(otherRt, field);
                Rect otherWithGap = InflateRect(other, _minSeparationGap);
                if (!mine.Overlaps(otherWithGap))
                    continue;

                Vector2 d = SmallestSeparationDelta(mine, otherWithGap);
                if (d.sqrMagnitude < 1e-8f)
                    continue;

                Vector3 lp = _rect.localPosition;
                _rect.localPosition = new Vector3(lp.x + d.x, lp.y + d.y, lp.z);
                moved = true;
                break;
            }

            if (!moved)
                break;
        }

        for (int c = 0; c < 10; c++)
        {
            if (!ClampInsideFieldLocal(field))
                break;
        }
    }

    static Rect AxisAlignedBoundsInParent(RectTransform child, RectTransform parentSpace)
    {
        child.GetWorldCorners(CornersBuffer);
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        for (int i = 0; i < 4; i++)
        {
            Vector3 pl = parentSpace.InverseTransformPoint(CornersBuffer[i]);
            min = Vector2.Min(min, pl);
            max = Vector2.Max(max, pl);
        }

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    static Rect InflateRect(Rect r, float padding)
    {
        return Rect.MinMaxRect(r.xMin - padding, r.yMin - padding, r.xMax + padding, r.yMax + padding);
    }

    /// <summary>moving 을 obstacle 밖으로 한 축만 밀 때 최소 이동량.</summary>
    static Vector2 SmallestSeparationDelta(Rect moving, Rect obstacle)
    {
        Vector2[] candidates =
        {
            new Vector2(obstacle.xMin - moving.xMax, 0f),
            new Vector2(obstacle.xMax - moving.xMin, 0f),
            new Vector2(0f, obstacle.yMin - moving.yMax),
            new Vector2(0f, obstacle.yMax - moving.yMin),
        };

        Vector2 best = Vector2.zero;
        float bestSqr = float.MaxValue;

        for (int i = 0; i < candidates.Length; i++)
        {
            float s = candidates[i].sqrMagnitude;
            if (s > 1e-6f && s < bestSqr)
            {
                bestSqr = s;
                best = candidates[i];
            }
        }

        return best;
    }

    bool ClampInsideFieldLocal(RectTransform field)
    {
        Rect fr = AxisAlignedBoundsInParent(field, field);
        Rect pr = AxisAlignedBoundsInParent(_rect, field);

        Vector2 fix = Vector2.zero;

        if (pr.xMin < fr.xMin)
            fix.x += fr.xMin - pr.xMin;
        else if (pr.xMax > fr.xMax)
            fix.x += fr.xMax - pr.xMax;

        if (pr.yMin < fr.yMin)
            fix.y += fr.yMin - pr.yMin;
        else if (pr.yMax > fr.yMax)
            fix.y += fr.yMax - pr.yMax;

        if (fix.sqrMagnitude < 1e-8f)
            return false;

        Vector3 lp = _rect.localPosition;
        _rect.localPosition = new Vector3(lp.x + fix.x, lp.y + fix.y, lp.z);
        return true;
    }

    bool TryMergeOnDrop(PointerEventData eventData)
    {
        if (EventSystem.current == null || Catalog == null)
            return false;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject == gameObject)
                continue;

            var receiver = r.gameObject.GetComponent<DraggableUIImage>();
            if (receiver == null)
                continue;

            if (!CanMergeWith(receiver))
                continue;

            receiver.AbsorbMergeFrom(this);
            return true;
        }

        return false;
    }

    bool CanMergeWith(DraggableUIImage receiver)
    {
        if (receiver == null || Catalog == null)
            return false;

        var a = _image != null ? _image.sprite : null;
        var b = receiver._image != null ? receiver._image.sprite : null;
        return Catalog.CanMerge(a, b, out _);
    }

    void AbsorbMergeFrom(DraggableUIImage dragged)
    {
        if (dragged == null || dragged == this || Catalog == null)
            return;

        var a = dragged._image != null ? dragged._image.sprite : null;
        var b = _image != null ? _image.sprite : null;
        if (!Catalog.CanMerge(a, b, out var nextSprite))
            return;

        Transform freedSlot = dragged.transform.parent;

        Destroy(dragged.gameObject);

        if (_image != null)
        {
            _image.sprite = nextSprite;
            _image.color = Color.white;
        }

        Spawner?.RefillSpawnSlot(freedSlot);
    }
}
