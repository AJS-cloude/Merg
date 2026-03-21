using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Merge 보드 슬롯 — 레거시(ItemData) 또는 모듈형 캐릭터(몸/표정/손).
/// </summary>
public class UIMergeSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Legacy (단일 아이콘)")]
    [SerializeField] Image iconImage;
    [SerializeField] Image frameImage;
    [SerializeField] Text levelText;

    [Header("Character (비어 있으면 iconImage에 몸만)")]
    [SerializeField] Image bodyImage;
    [SerializeField] Image faceImage;
    [SerializeField] Image handImage;

    int _index;
    System.Action<int> _onClick;
    Color _normalColor = Color.white;

    void Awake()
    {
        if (frameImage != null) _normalColor = frameImage.color;
    }

    public void Setup(int index, System.Action<int> onClick)
    {
        _index = index;
        _onClick = onClick;
    }

    public void SetItem(ItemData item, bool selected)
    {
        ClearCharacterLayers();
        if (iconImage != null)
        {
            iconImage.enabled = item != null;
            if (item != null)
            {
                iconImage.sprite = item.icon;
                iconImage.color = item.tint;
            }
        }
        if (levelText != null) levelText.text = item != null ? "Lv" + item.level : "";
        if (frameImage != null) frameImage.color = selected ? new Color(1f, 1f, 0.5f) : _normalColor;
    }

    public void SetCharacter(CharacterInstance c, CharacterVisualCatalog catalog, bool handsVisible, bool selected)
    {
        if (iconImage != null) iconImage.enabled = false;
        bool show = !c.isEmpty;
        Image body = bodyImage != null ? bodyImage : iconImage;

        if (body != null)
        {
            body.enabled = show;
            if (show && catalog != null)
            {
                body.sprite = catalog.GetBody(c.colorIndex, c.shapeIndex);
                body.color = Color.white;
            }
        }

        if (faceImage != null)
        {
            var fs = catalog != null ? catalog.GetFace(c.faceIndex) : null;
            faceImage.enabled = show && fs != null;
            if (faceImage.enabled) faceImage.sprite = fs;
        }

        if (handImage != null)
        {
            bool showHand = handsVisible && c.handGesture > 0;
            var hs = showHand && catalog != null ? catalog.GetHand(c.colorIndex, c.handGesture) : null;
            handImage.enabled = hs != null;
            if (handImage.enabled) handImage.sprite = hs;
        }

        if (levelText != null) levelText.text = "";
        if (frameImage != null) frameImage.color = selected ? new Color(1f, 1f, 0.5f) : _normalColor;
    }

    void ClearCharacterLayers()
    {
        if (faceImage != null) faceImage.enabled = false;
        if (handImage != null) handImage.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke(_index);
    }
}
