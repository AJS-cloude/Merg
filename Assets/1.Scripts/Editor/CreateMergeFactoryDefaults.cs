using UnityEngine;
using UnityEditor;

/// <summary>
/// GDD 기준 기본 데이터 생성: GameConfig, 아이템 체인(Lv1~6), Upgrade 예시.
/// 메뉴: Tools > Merge Factory > Create Default Data
/// </summary>
public static class CreateMergeFactoryDefaults
{
    const string DataPath = "Assets/Data";

    [MenuItem("Tools/Merge Factory/Create Default Data")]
    public static void CreateAll()
    {
        EnsureFolder(DataPath);
        EnsureFolder(DataPath + "/Items");
        EnsureFolder(DataPath + "/Upgrades");

        var config = CreateOrLoad<GameConfig>(DataPath + "/GameConfig.asset", () =>
        {
            var c = ScriptableObject.CreateInstance<GameConfig>();
            c.maxFactorySlots = 10;
            c.maxOfflineSeconds = 8f * 3600f;
            c.startingGold = 100;
            c.startingGems = 10;
            c.maxUpgradeLevel = 15;
            return c;
        });

        ItemData lv6 = CreateItem("Item_Robot", "로봇", 6, 500, 50f, null);
        ItemData lv5 = CreateItem("Item_Engine", "엔진", 5, 200, 20f, lv6);
        ItemData lv4 = CreateItem("Item_Motor", "모터", 4, 80, 8f, lv5);
        ItemData lv3 = CreateItem("Item_Gear", "기어", 3, 30, 3f, lv4);
        ItemData lv2 = CreateItem("Item_IronPlate", "철판", 2, 10, 1.5f, lv3);
        ItemData lv1 = CreateItem("Item_IronScrap", "철 조각", 1, 3, 0.5f, lv2);

        CreateUpgrade("Upgrade_ProductionSpeed", "생산 속도", UpgradeData.UpgradeType.ProductionSpeed);
        CreateUpgrade("Upgrade_SellPrice", "판매 가격", UpgradeData.UpgradeType.SellPrice);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Merge Factory default data created under Assets/Data.");
    }

    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder("Assets")) return;
        string[] parts = path.Replace("Assets/", "").Split('/');
        string current = "Assets";
        for (int i = 0; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    static T CreateOrLoad<T>(string path, System.Func<T> create) where T : Object
    {
        var existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null) return existing;
        var asset = create();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    static ItemData CreateItem(string id, string name, int level, int sellPrice, float goldPerSec, ItemData nextTier)
    {
        string path = DataPath + "/Items/" + id + ".asset";
        var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (item != null) return item;
        item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId = id;
        item.displayName = name;
        item.level = level;
        item.sellPrice = sellPrice;
        item.goldPerSecond = goldPerSec;
        item.nextTier = nextTier;
        AssetDatabase.CreateAsset(item, path);
        return item;
    }

    static void CreateUpgrade(string id, string name, UpgradeData.UpgradeType type)
    {
        string path = DataPath + "/Upgrades/" + id + ".asset";
        if (AssetDatabase.LoadAssetAtPath<UpgradeData>(path) != null) return;
        var u = ScriptableObject.CreateInstance<UpgradeData>();
        u.upgradeId = id;
        u.displayName = name;
        u.type = type;
        u.baseCost = 50;
        u.costMultiplierPerLevel = 1.5f;
        u.maxLevel = 15;
        u.valuePerLevel = 0.1f;
        AssetDatabase.CreateAsset(u, path);
    }
}
