using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 중앙 Merge Board — 인벤토리 그리드. 레거시(ItemData) 또는 캐릭터 인벤.
/// </summary>
public class UIInventoryGrid : MonoBehaviour
{
    [SerializeField] Transform slotContainer;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Color selectedColor = Color.yellow;
    [SerializeField] Color normalColor = Color.white;

    [Header("Character merge")]
    [SerializeField] CharacterVisualCatalog characterVisuals;

    readonly List<UIMergeSlot> _slots = new List<UIMergeSlot>();

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemAdded += OnLegacyInventoryChanged;
            InventoryManager.Instance.OnItemRemoved += OnLegacyInventoryChanged;
        }
        if (CharacterInventoryManager.Instance != null)
            CharacterInventoryManager.Instance.OnChanged += OnCharacterInventoryChanged;

        BuildSlots();
        RefreshAll();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemAdded -= OnLegacyInventoryChanged;
            InventoryManager.Instance.OnItemRemoved -= OnLegacyInventoryChanged;
        }
        if (CharacterInventoryManager.Instance != null)
            CharacterInventoryManager.Instance.OnChanged -= OnCharacterInventoryChanged;
    }

    void OnLegacyInventoryChanged(int index, ItemData item) => RefreshAll();

    void OnCharacterInventoryChanged() => RefreshAll();

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
        if (MergeBoard.Instance == null) return;

        bool charMode = MergeBoard.Instance.UseCharacterMerge;
        if (charMode && CharacterInventoryManager.Instance != null)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var p = CharacterInventoryManager.Instance.GetAt(i);
                bool sel = MergeBoard.Instance.IsIndexSelected(i);
                _slots[i].SetCharacter(p, characterVisuals, MergeStageProgress.HandsVisible, sel);
            }
        }
        else if (InventoryManager.Instance != null)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var item = InventoryManager.Instance.GetAt(i);
                _slots[i].SetItem(item, MergeBoard.Instance.IsIndexSelected(i));
            }
        }
    }
}
