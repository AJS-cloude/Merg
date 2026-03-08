using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GDD: Upgrade - 생산속도/판매가 등 업그레이드 구매.
/// </summary>
public class UIUpgradePanel : MonoBehaviour
{
    [SerializeField] Transform listContainer;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] UpgradeData[] upgrades;

    void Start()
    {
        if (UpgradeManager.Instance == null || listContainer == null || rowPrefab == null || upgrades == null) return;
        foreach (var u in upgrades)
        {
            if (u == null) continue;
            var row = Instantiate(rowPrefab, listContainer);
            var nameText = row.transform.Find("Name")?.GetComponent<Text>();
            var levelText = row.transform.Find("Level")?.GetComponent<Text>();
            var costText = row.transform.Find("Cost")?.GetComponent<Text>();
            var btn = row.GetComponentInChildren<Button>();
            if (nameText != null) nameText.text = u.displayName;
            RefreshRow(row, u);
            if (btn != null)
            {
                var capture = u;
                btn.onClick.AddListener(() =>
                {
                    if (UpgradeManager.Instance.TryPurchaseUpgrade(capture.upgradeId))
                        RefreshRow(row, capture);
                });
            }
        }
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeLevelChanged += (id, lv) => RefreshAllRows();
    }

    void RefreshRow(GameObject row, UpgradeData u)
    {
        if (u == null || UpgradeManager.Instance == null) return;
        int lv = UpgradeManager.Instance.GetLevel(u.upgradeId);
        int cost = UpgradeManager.Instance.GetUpgradeCost(u.upgradeId);
        var levelText = row.transform.Find("Level")?.GetComponent<Text>();
        var costText = row.transform.Find("Cost")?.GetComponent<Text>();
        var btn = row.GetComponentInChildren<Button>();
        if (levelText != null) levelText.text = "Lv " + lv + "/" + u.maxLevel;
        if (costText != null) costText.text = cost <= 0 || cost >= 999999 ? "-" : cost.ToString("N0");
        if (btn != null) btn.interactable = cost > 0 && cost < 999999 && EconomyManager.Instance != null && EconomyManager.Instance.Gold >= cost;
    }

    void RefreshAllRows()
    {
        if (listContainer == null || upgrades == null) return;
        int i = 0;
        foreach (Transform t in listContainer)
        {
            if (i < upgrades.Length && upgrades[i] != null) RefreshRow(t.gameObject, upgrades[i]);
            i++;
        }
    }
}
