using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 디펜스: 타워 슬롯 1개. 클릭 시 배치/제거 또는 피커.
/// </summary>
public class UIDefenseSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image background;
    [SerializeField] Text label;
    [SerializeField] int slotIndex;
    [SerializeField] Color emptyColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] Color filledColor = Color.white;

    ItemData _current;

    void Start()
    {
        if (DefenseManager.Instance != null)
        {
            DefenseManager.Instance.OnSlotChanged += OnSlotChanged;
            _current = DefenseManager.Instance.GetSlot(slotIndex);
        }
        Refresh();
    }

    void OnDestroy()
    {
        if (DefenseManager.Instance != null)
            DefenseManager.Instance.OnSlotChanged -= OnSlotChanged;
    }

    void OnSlotChanged(int index, ItemData item)
    {
        if (index != slotIndex) return;
        _current = item;
        Refresh();
    }

    void Refresh()
    {
        var item = DefenseManager.Instance != null ? DefenseManager.Instance.GetSlot(slotIndex) : null;
        _current = item;
        if (iconImage != null)
        {
            iconImage.enabled = item != null;
            if (item != null && item.icon != null) { iconImage.sprite = item.icon; iconImage.color = item.tint; }
        }
        if (label != null) label.text = item != null ? item.displayName : "";
        if (background != null) background.color = item != null ? filledColor : emptyColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DefenseManager.Instance == null) return;
        if (_current != null)
        {
            DefenseManager.Instance.ClearSlot(slotIndex);
            InventoryManager.Instance?.AddItem(_current);
            return;
        }
        UIDefenseSlotPicker.Instance?.OpenForSlot(slotIndex);
    }
}
