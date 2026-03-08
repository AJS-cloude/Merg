using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// GDD: 중앙 Merge Board - 인벤토리 아이템 그리드. 클릭 시 Merge 선택.
/// </summary>
public class UIInventoryGrid : MonoBehaviour
{
    [SerializeField] Transform slotContainer;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Color selectedColor = Color.yellow;
    [SerializeField] Color normalColor = Color.white;

    readonly List<UIMergeSlot> _slots = new List<UIMergeSlot>();

    void Start()
    {
        if (InventoryManager.Instance == null) return;
        InventoryManager.Instance.OnItemAdded += OnInventoryChanged;
        InventoryManager.Instance.OnItemRemoved += OnInventoryChanged;
        BuildSlots();
        RefreshAll();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemAdded -= OnInventoryChanged;
            InventoryManager.Instance.OnItemRemoved -= OnInventoryChanged;
        }
    }

    void OnInventoryChanged(int index, ItemData item)
    {
        RefreshAll();
    }

    void BuildSlots()
    {
        if (slotContainer == null || slotPrefab == null) return;
        int count = 24;
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            var slot = go.GetComponent<UIMergeSlot>();
            if (slot == null) slot = go.AddComponent<UIMergeSlot>();
            slot.Setup(i, OnSlotClicked);
            _slots.Add(slot);
        }
    }

    void OnSlotClicked(int index)
    {
        MergeBoard.Instance?.OnItemClicked(index);
        RefreshAll();
    }

    void RefreshAll()
    {
        if (InventoryManager.Instance == null || MergeBoard.Instance == null) return;
        int selected = MergeBoard.Instance.GetSelectedIndex();
        for (int i = 0; i < _slots.Count; i++)
        {
            var item = InventoryManager.Instance.GetAt(i);
            _slots[i].SetItem(item, i == selected);
        }
    }

    void Update()
    {
        if (MergeBoard.Instance != null)
            RefreshAll();
    }
}
