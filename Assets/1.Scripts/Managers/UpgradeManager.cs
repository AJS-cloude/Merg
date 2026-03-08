using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GDD: 업그레이드 시스템 - 생산속도, Merge보너스, 자동Merge, 판매가.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] UpgradeData[] upgrades;
    readonly Dictionary<string, int> _levels = new Dictionary<string, int>();

    public event Action<string, int> OnUpgradeLevelChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int GetLevel(string upgradeId)
    {
        return _levels.TryGetValue(upgradeId, out var lv) ? lv : 0;
    }

    public void SetLevelFromSave(string upgradeId, int level)
    {
        if (string.IsNullOrEmpty(upgradeId)) return;
        _levels[upgradeId] = Mathf.Clamp(level, 0, GetMaxLevel(upgradeId));
    }

    public int GetMaxLevel(string upgradeId)
    {
        var data = GetUpgradeData(upgradeId);
        return data != null ? data.maxLevel : 1;
    }

    public UpgradeData GetUpgradeData(string upgradeId)
    {
        if (upgrades == null) return null;
        foreach (var u in upgrades)
            if (u != null && u.upgradeId == upgradeId) return u;
        return null;
    }

    public int GetUpgradeCost(string upgradeId)
    {
        var data = GetUpgradeData(upgradeId);
        if (data == null) return int.MaxValue;
        int lv = GetLevel(upgradeId);
        if (lv >= data.maxLevel) return int.MaxValue;
        return Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.costMultiplierPerLevel, lv));
    }

    public bool TryPurchaseUpgrade(string upgradeId)
    {
        var data = GetUpgradeData(upgradeId);
        if (data == null) return false;
        int lv = GetLevel(upgradeId);
        if (lv >= data.maxLevel) return false;
        int cost = GetUpgradeCost(upgradeId);
        if (EconomyManager.Instance == null || !EconomyManager.Instance.TrySpendGold(cost)) return false;
        _levels[upgradeId] = lv + 1;
        OnUpgradeLevelChanged?.Invoke(upgradeId, _levels[upgradeId]);
        return true;
    }

    public float GetProductionSpeedMultiplier()
    {
        float mult = 1f;
        if (upgrades == null) return mult;
        foreach (var u in upgrades)
        {
            if (u == null || u.type != UpgradeData.UpgradeType.ProductionSpeed) continue;
            int lv = GetLevel(u.upgradeId);
            mult += u.valuePerLevel * lv;
        }
        return mult;
    }

    public float GetSellPriceMultiplier()
    {
        float mult = 1f;
        if (upgrades == null) return mult;
        foreach (var u in upgrades)
        {
            if (u == null || u.type != UpgradeData.UpgradeType.SellPrice) continue;
            int lv = GetLevel(u.upgradeId);
            mult += u.valuePerLevel * lv;
        }
        return mult;
    }
}
