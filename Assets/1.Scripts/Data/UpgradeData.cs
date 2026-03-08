using UnityEngine;

/// <summary>
/// GDD: 업그레이드 15종 - 생산속도, Merge보너스, 자동Merge, 판매가 증가 등.
/// </summary>
[CreateAssetMenu(fileName = "Upgrade_", menuName = "Merge Factory/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    [TextArea(1, 3)]
    public string description;

    public enum UpgradeType
    {
        ProductionSpeed,   // 생산 속도 증가
        MergeBonus,        // Merge 보너스
        AutoMerge,         // 자동 Merge
        SellPrice          // 판매 가격 증가
    }

    public UpgradeType type;
    public int baseCost = 50;
    public float costMultiplierPerLevel = 1.5f;
    public int maxLevel = 15;
    public float valuePerLevel = 0.1f;  // e.g. 10% per level
}
