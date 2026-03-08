using System;
using UnityEngine;

/// <summary>
/// GDD: 동일 레벨 아이템 2개를 합치면 상위 아이템 생성.
/// </summary>
public class MergeManager : MonoBehaviour
{
    public static MergeManager Instance { get; private set; }

    /// <summary>두 아이템이 Merge 가능한지 (같은 ItemData, 최대 레벨 아님)</summary>
    public bool CanMerge(ItemData a, ItemData b)
    {
        if (a == null || b == null) return false;
        if (a != b) return false;
        return !a.IsMaxTier && a.nextTier != null;
    }

    /// <summary>Merge 결과 아이템 (2개 소비 시 1개 생성)</summary>
    public ItemData GetMergeResult(ItemData item)
    {
        if (item == null || item.IsMaxTier) return null;
        return item.nextTier;
    }

    public event Action<ItemData, ItemData, ItemData> OnMerged;

    public void NotifyMerged(ItemData consumedA, ItemData consumedB, ItemData result)
    {
        OnMerged?.Invoke(consumedA, consumedB, result);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
