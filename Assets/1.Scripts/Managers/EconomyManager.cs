using System;
using UnityEngine;

/// <summary>
/// GDD: 재화 관리. Gold(업그레이드/구매), Gems(프리미엄).
/// </summary>
public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] GameConfig config;

    int _gold;
    int _gems;

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = Mathf.Max(0, value);
            OnGoldChanged?.Invoke(_gold);
        }
    }

    public int Gems
    {
        get => _gems;
        set
        {
            _gems = Mathf.Max(0, value);
            OnGemsChanged?.Invoke(_gems);
        }
    }

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitFromSave(int gold, int gems)
    {
        _gold = gold;
        _gems = gems;
        OnGoldChanged?.Invoke(_gold);
        OnGemsChanged?.Invoke(_gems);
    }

    public void ApplyStartingCurrencies()
    {
        if (config != null)
        {
            Gold = config.startingGold;
            Gems = config.startingGems;
        }
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0 || _gold < amount) return false;
        Gold -= amount;
        return true;
    }

    public bool TrySpendGems(int amount)
    {
        if (amount <= 0 || _gems < amount) return false;
        Gems -= amount;
        return true;
    }

    public void AddGold(int amount) => Gold += amount;
    public void AddGems(int amount) => Gems += amount;
}
