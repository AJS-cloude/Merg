using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디펜스: 인벤토리에서 타워(ItemData.IsTower) 선택해 방어 슬롯에 배치.
/// </summary>
public class UIDefenseSlotPicker : MonoBehaviour
{
    public static UIDefenseSlotPicker Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject itemButtonPrefab;

    int _targetSlotIndex = -1;
    readonly List<GameObject> _buttons = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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
        foreach (var b in _buttons) { if (b != null) Destroy(b); }
        _buttons.Clear();
        if (buttonContainer == null || itemButtonPrefab == null || InventoryManager.Instance == null) return;

        for (int i = 0; i < InventoryManager.Instance.Count; i++)
        {
            var item = InventoryManager.Instance.GetAt(i);
            if (item == null || !item.IsTower) continue;
            var go = Instantiate(itemButtonPrefab, buttonContainer);
            _buttons.Add(go);
            var img = go.GetComponent<Image>();
            if (img != null && item.icon != null) img.sprite = item.icon;
            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                var capture = item;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSelect(capture));
            }
        }
    }

    void OnSelect(ItemData item)
    {
        if (_targetSlotIndex < 0 || DefenseManager.Instance == null || InventoryManager.Instance == null) return;
        if (!InventoryManager.Instance.RemoveOne(item)) return;
        DefenseManager.Instance.SetSlot(_targetSlotIndex, item);
        Close();
    }

    public void Close()
    {
        _targetSlotIndex = -1;
        if (panel != null) panel.SetActive(false);
    }
}
