using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GDD: 세이브/로드 + 오프라인 보상 (최대 8시간 생산 반영).
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    const string KeyLastQuitTime = "MergeFactory_LastQuitTime";
    const string KeyGold = "MergeFactory_Gold";
    const string KeyGems = "MergeFactory_Gems";
    const string KeyFactorySlotCount = "MergeFactory_FactorySlots";
    const string KeyFactorySlotPrefix = "MergeFactory_Slot_";
    const string KeyInventoryCount = "MergeFactory_InventoryCount";
    const string KeyInventoryPrefix = "MergeFactory_Inv_";
    const string KeyCharInvCount = "MergeFactory_CharInvCount";
    const string KeyCharInvPrefix = "MergeFactory_CharInv_";

    [SerializeField] GameConfig config;
    [SerializeField] ItemData[] allItemData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadGame();
    }

    void Start()
    {
        ApplyOfflineReward();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) SaveGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetString(KeyLastQuitTime, DateTime.UtcNow.ToBinary().ToString());
        if (EconomyManager.Instance != null)
        {
            PlayerPrefs.SetInt(KeyGold, EconomyManager.Instance.Gold);
            PlayerPrefs.SetInt(KeyGems, EconomyManager.Instance.Gems);
        }
        if (FactoryManager.Instance != null)
        {
            var slots = FactoryManager.Instance.GetSlotsForSave();
            PlayerPrefs.SetInt(KeyFactorySlotCount, slots.Count);
            for (int i = 0; i < slots.Count; i++)
            {
                var item = slots[i];
                PlayerPrefs.SetString(KeyFactorySlotPrefix + i, item != null ? item.itemId : "");
            }
        }
        if (InventoryManager.Instance != null)
        {
            var items = InventoryManager.Instance.GetAllItemsForSave();
            PlayerPrefs.SetInt(KeyInventoryCount, items.Count);
            for (int i = 0; i < items.Count; i++)
                PlayerPrefs.SetString(KeyInventoryPrefix + i, items[i] != null ? items[i].itemId : "");
        }
        if (CharacterInventoryManager.Instance != null)
        {
            var list = CharacterInventoryManager.Instance.GetAllForSave();
            PlayerPrefs.SetInt(KeyCharInvCount, list.Count);
            for (int i = 0; i < list.Count; i++)
                PlayerPrefs.SetString(KeyCharInvPrefix + i, list[i].ToSaveString());
        }
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        int gold = config != null ? config.startingGold : 100;
        int gems = config != null ? config.startingGems : 10;
        if (PlayerPrefs.HasKey(KeyGold)) gold = PlayerPrefs.GetInt(KeyGold);
        if (PlayerPrefs.HasKey(KeyGems)) gems = PlayerPrefs.GetInt(KeyGems);

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.InitFromSave(gold, gems);

        if (FactoryManager.Instance != null && allItemData != null && allItemData.Length > 0)
        {
            int count = PlayerPrefs.GetInt(KeyFactorySlotCount, 0);
            var ids = new List<string>();
            for (int i = 0; i < count; i++)
                ids.Add(PlayerPrefs.GetString(KeyFactorySlotPrefix + i, ""));
            FactoryManager.Instance.SetSlotsFromSave(ids, allItemData);
        }

        if (InventoryManager.Instance != null && allItemData != null && allItemData.Length > 0)
        {
            int count = PlayerPrefs.GetInt(KeyInventoryCount, 0);
            var ids = new List<string>();
            for (int i = 0; i < count; i++)
                ids.Add(PlayerPrefs.GetString(KeyInventoryPrefix + i, ""));
            InventoryManager.Instance.LoadFromSave(ids, allItemData);
        }

        if (CharacterInventoryManager.Instance != null)
        {
            int count = PlayerPrefs.GetInt(KeyCharInvCount, 0);
            var lines = new List<string>();
            for (int i = 0; i < count; i++)
                lines.Add(PlayerPrefs.GetString(KeyCharInvPrefix + i, ""));
            CharacterInventoryManager.Instance.LoadFromSave(lines);
        }
    }

    void ApplyOfflineReward()
    {
        if (!PlayerPrefs.HasKey(KeyLastQuitTime)) return;
        if (FactoryManager.Instance == null) return;

        long binary = long.Parse(PlayerPrefs.GetString(KeyLastQuitTime));
        var lastQuit = DateTime.FromBinary(binary);
        float offlineSeconds = (float)(DateTime.UtcNow - lastQuit).TotalSeconds;
        float gold = FactoryManager.Instance.CalculateOfflineGold(offlineSeconds);
        FactoryManager.Instance.ApplyOfflineGold(gold);
        OnOfflineRewardApplied?.Invoke(offlineSeconds, gold);
    }

    public event Action<float, float> OnOfflineRewardApplied;

    public void SetAllItemData(ItemData[] items) => allItemData = items;
}
