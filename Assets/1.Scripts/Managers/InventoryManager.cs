using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Merge 보드/인벤토리: 플레이어가 보유한 아이템 목록 (Merge용 + 공장 배치용).
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    readonly List<ItemData> _items = new List<ItemData>();

    public int Count => _items.Count;
    public ItemData GetAt(int index) => index >= 0 && index < _items.Count ? _items[index] : null;

    public event Action<int, ItemData> OnItemAdded;
    public event Action<int, ItemData> OnItemRemoved;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        if (item == null) return;
        _items.Add(item);
        OnItemAdded?.Invoke(_items.Count - 1, item);
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count) return false;
        var item = _items[index];
        _items.RemoveAt(index);
        OnItemRemoved?.Invoke(index, item);
        return true;
    }

    public bool RemoveOne(ItemData item)
    {
        int idx = _items.IndexOf(item);
        if (idx < 0) return false;
        return RemoveAt(idx);
    }

    /// <summary>같은 ItemData 2개 제거 후 result 1개 추가 (Merge)</summary>
    public bool ConsumeTwoAndAddResult(ItemData sameItem, ItemData result)
    {
        if (sameItem == null) return false;
        int first = _items.IndexOf(sameItem);
        if (first < 0) return false;
        int second = -1;
        for (int i = first + 1; i < _items.Count; i++)
        {
            if (_items[i] == sameItem) { second = i; break; }
        }
        if (second < 0) return false;
        _items.RemoveAt(second);
        _items.RemoveAt(first);
        if (result != null)
            _items.Add(result);
        return true;
    }

    public IReadOnlyList<ItemData> GetAllItemsForSave()
    {
        return _items;
    }

    public void LoadFromSave(List<string> itemIds, ItemData[] allItemData)
    {
        _items.Clear();
        if (itemIds == null || allItemData == null) return;
        foreach (var id in itemIds)
        {
            if (string.IsNullOrEmpty(id)) continue;
            foreach (var data in allItemData)
            {
                if (data != null && data.itemId == id)
                {
                    _items.Add(data);
                    break;
                }
            }
        }
    }
}
