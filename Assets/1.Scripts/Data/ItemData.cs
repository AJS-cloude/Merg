using UnityEngine;

/// <summary>
/// GDD: ScriptableObject 기반 아이템 데이터.
/// 아이템 레벨에 따라 Merge 시 상위 아이템으로 합쳐짐.
/// </summary>
[CreateAssetMenu(fileName = "Item_", menuName = "Merge Factory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string displayName;
    [TextArea(2, 4)]
    public string description;

    [Header("Merge")]
    /// <summary>레벨 1 = 철 조각, 2 = 철판, 3 = 기어 ... 6 = 로봇</summary>
    public int level = 1;
    /// <summary>이 아이템 2개를 Merge하면 생성되는 상위 아이템 (null이면 최대 레벨)</summary>
    public ItemData nextTier;

    [Header("Economy")]
    /// <summary>판매 시 획득 골드 (기본값)</summary>
    public int sellPrice = 10;
    /// <summary>Idle 생산 시 이 아이템 1개당 골드/초</summary>
    public float goldPerSecond = 1f;

    [Header("Visual")]
    public Sprite icon;
    public Color tint = Color.white;

    [Header("Defense (디펜스 모드용)")]
    [Tooltip("0이면 타워 아님. 디펜스 모드에서 슬롯에 배치 시 이 값으로 공격.")]
    public float attackDamage = 0f;
    public float attackRange = 3f;
    public float attackInterval = 1f;
    /// <summary>디펜스 모드에서 타워로 쓸 수 있는지</summary>
    public bool IsTower => attackDamage > 0f;

    /// <summary>최대 레벨 아이템인지 (Merge 불가)</summary>
    public bool IsMaxTier => nextTier == null;

    /// <summary>동일 레벨 2개 Merge 시 nextTier 반환</summary>
    public ItemData GetMergedResult()
    {
        return nextTier;
    }
}
