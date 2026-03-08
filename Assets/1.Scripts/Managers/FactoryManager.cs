using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GDD: 공장 슬롯에 아이템 배치 시 자동 생산(Idle). 게임 종료 후에도 생산, 최대 8시간.
/// </summary>
public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance { get; private set; }

    [SerializeField] GameConfig config;
    [SerializeField] float tickInterval = 1f;

    readonly List<ItemData> _slots = new List<ItemData>();
    float _accumulatedTime;

    public int MaxSlots => config != null ? config.maxFactorySlots : 10;
    public int SlotCount => _slots.Count;
    public float MaxOfflineSeconds => config != null ? config.maxOfflineSeconds : 8f * 3600f;

    public event Action<int, ItemData> OnSlotChanged;
    public event Action<float> OnGoldProduced;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        _accumulatedTime += Time.deltaTime;
        if (_accumulatedTime < tickInterval) return;
        _accumulatedTime -= tickInterval;
        ProduceGold(tickInterval);
    }

    void ProduceGold(float deltaTime)
    {
        float speedMult = 1f;
        if (UpgradeManager.Instance != null) speedMult = UpgradeManager.Instance.GetProductionSpeedMultiplier();
        if (AdManager.Instance != null) speedMult *= AdManager.Instance.ProductionMultiplier;
        float total = 0f;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] != null)
                total += _slots[i].goldPerSecond * deltaTime * speedMult;
        }
        if (total > 0 && EconomyManager.Instance != null)
        {
            EconomyManager.Instance.AddGold(Mathf.FloorToInt(total));
            OnGoldProduced?.Invoke(total);
        }
    }

    /// <summary>오프라인 생산 (최대 8시간). SaveManager에서 호출.</summary>
    public float CalculateOfflineGold(float offlineSeconds)
    {
        float capped = Mathf.Min(offlineSeconds, MaxOfflineSeconds);
        float speedMult = 1f;
        if (UpgradeManager.Instance != null) speedMult = UpgradeManager.Instance.GetProductionSpeedMultiplier();
        float total = 0f;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] != null)
                total += _slots[i].goldPerSecond * capped * speedMult;
        }
        return total;
    }

    public void ApplyOfflineGold(float gold)
    {
        if (gold > 0 && EconomyManager.Instance != null)
            EconomyManager.Instance.AddGold(Mathf.FloorToInt(gold));
    }

    public ItemData GetSlot(int index)
    {
        if (index < 0 || index >= _slots.Count) return null;
        return _slots[index];
    }

    public bool SetSlot(int index, ItemData item)
    {
        if (index < 0 || index >= MaxSlots) return false;
        while (_slots.Count <= index)
            _slots.Add(null);
        _slots[index] = item;
        OnSlotChanged?.Invoke(index, item);
        return true;
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= _slots.Count) return;
        _slots[index] = null;
        OnSlotChanged?.Invoke(index, null);
    }

    /// <summary>세이브/로드용: 슬롯 전체 덮어쓰기</summary>
    public void SetSlotsFromSave(IReadOnlyList<string> itemIds, ItemData[] allItems)
    {
        _slots.Clear();
        if (itemIds == null || allItems == null) return;
        for (int i = 0; i < itemIds.Count && i < MaxSlots; i++)
        {
            var id = itemIds[i];
            ItemData found = null;
            if (!string.IsNullOrEmpty(id))
            {
                foreach (var data in allItems)
                {
                    if (data != null && data.itemId == id) { found = data; break; }
                }
            }
            _slots.Add(found);
        }
        for (int i = 0; i < _slots.Count; i++)
            OnSlotChanged?.Invoke(i, _slots[i]);
    }

    public IReadOnlyList<ItemData> GetSlotsForSave()
    {
        return _slots;
    }
}
