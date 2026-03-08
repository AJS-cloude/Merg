using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Merge 보드의 슬롯 1개. 아이템 표시 + 클릭 시 선택.
/// </summary>
public class UIMergeSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image frameImage;
    [SerializeField] Text levelText;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke(_index);
    }
}
