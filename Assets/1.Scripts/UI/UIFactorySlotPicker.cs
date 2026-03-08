using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리에서 아이템 선택해 공장 슬롯에 배치.
/// </summary>
public class UIFactorySlotPicker : MonoBehaviour
{
    public static UIFactorySlotPicker Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject itemButtonPrefab;
    [SerializeField] ItemData[] allItems;

    int _targetSlotIndex = -1;
    readonly List<GameObject> _buttons = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void OpenForSlot(int slotIndex)
    {
        _targetSlotIndex = slotIndex;
        if (panel != null) panel.SetActive(true);
        RefreshButtons();
    }

    void RefreshButtons()
    {
        foreach (var b in _buttons)
        {
            if (b != null) Destroy(b);
        }
        _buttons.Clear();
        if (buttonContainer == null || itemButtonPrefab == null || allItems == null) return;
        if (InventoryManager.Instance == null) return;

        for (int i = 0; i < InventoryManager.Instance.Count; i++)
        {
            var item = InventoryManager.Instance.GetAt(i);
            if (item == null) continue;
            var go = Instantiate(itemButtonPrefab, buttonContainer);
            _buttons.Add(go);
            var img = go.GetComponent<Image>();
            if (img != null && item.icon != null) img.sprite = item.icon;
            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                var capture = item;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSelectItem(capture));
            }
        }
    }

    void OnSelectItem(ItemData item)
    {
        if (_targetSlotIndex < 0 || FactoryManager.Instance == null || InventoryManager.Instance == null) return;
        if (!InventoryManager.Instance.RemoveOne(item)) return;
        FactoryManager.Instance.SetSlot(_targetSlotIndex, item);
        Close();
    }

    public void Close()
    {
        _targetSlotIndex = -1;
        if (panel != null) panel.SetActive(false);
    }
}
