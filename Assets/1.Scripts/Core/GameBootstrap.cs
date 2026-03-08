using UnityEngine;

/// <summary>
/// 씬에 매니저가 없을 때 자동 생성. 한 씬에 빈 GameObject에 붙여두면 됨.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] GameConfig config;
    [SerializeField] ItemData[] allItemData;
    [SerializeField] UpgradeData[] upgrades;

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            var go = new GameObject("GameManager");
            var gm = go.AddComponent<GameManager>();
            if (config != null) SetPrivate(gm, "config", config);
            if (allItemData != null) SetPrivate(gm, "allItemData", allItemData);
        }

        if (EconomyManager.Instance == null) new GameObject("EconomyManager").AddComponent<EconomyManager>();
        if (MergeManager.Instance == null) new GameObject("MergeManager").AddComponent<MergeManager>();
        if (FactoryManager.Instance == null)
        {
            var go = new GameObject("FactoryManager");
            var fm = go.AddComponent<FactoryManager>();
            if (config != null) SetPrivate(fm, "config", config);
        }
        if (InventoryManager.Instance == null) new GameObject("InventoryManager").AddComponent<InventoryManager>();
        if (UpgradeManager.Instance == null)
        {
            var go = new GameObject("UpgradeManager");
            var um = go.AddComponent<UpgradeManager>();
            if (upgrades != null) SetPrivate(um, "upgrades", upgrades);
        }
        if (AdManager.Instance == null) new GameObject("AdManager").AddComponent<AdManager>();
        if (SaveManager.Instance == null)
        {
            var go = new GameObject("SaveManager");
            var sm = go.AddComponent<SaveManager>();
            if (config != null) SetPrivate(sm, "config", config);
            if (allItemData != null) SetPrivate(sm, "allItemData", allItemData);
        }
    }

    static void SetPrivate(object obj, string fieldName, object value)
    {
        var type = obj.GetType();
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
