using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// GDD: 하단 Factory 슬롯 1개. 클릭 시 배치/제거.
/// </summary>
public class UIFactorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image background;
    [SerializeField] Text label;
    [SerializeField] Color emptyColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] Color filledColor = Color.white;

    [SerializeField] int slotIndex;
    int _slotIndex;
    ItemData _currentItem;

    void Start()
    {
        Setup(slotIndex);
    }

    public void Setup(int slotIndex)
    {
        _slotIndex = slotIndex;
        Refresh(null);
        if (FactoryManager.Instance != null)
            FactoryManager.Instance.OnSlotChanged += OnSlotChanged;
    }

    void OnDestroy()
    {
        if (FactoryManager.Instance != null)
            FactoryManager.Instance.OnSlotChanged -= OnSlotChanged;
    }

    void OnSlotChanged(int index, ItemData item)
    {
        if (index != _slotIndex) return;
        Refresh(item);
    }

    void Refresh(ItemData item)
    {
        _currentItem = item;
        if (iconImage != null)
        {
            iconImage.enabled = item != null;
            if (item != null)
            {
                iconImage.sprite = item.icon;
                iconImage.color = item.tint;
            }
        }
        if (label != null) label.text = item != null ? item.displayName : "";
        if (background != null) background.color = item != null ? filledColor : emptyColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIFactorySlotPicker.Instance == null) return;
        if (_currentItem != null)
        {
            FactoryManager.Instance?.ClearSlot(_slotIndex);
            InventoryManager.Instance?.AddItem(_currentItem);
            return;
        }
        UIFactorySlotPicker.Instance.OpenForSlot(_slotIndex);
    }
}
