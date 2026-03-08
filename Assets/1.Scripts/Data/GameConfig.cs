using UnityEngine;

/// <summary>
/// GDD: 게임 전역 설정 (공장 슬롯 수, 오프라인 최대 시간 등)
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Merge Factory/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Factory")]
    [Tooltip("GDD: 공장 슬롯 10개")]
    public int maxFactorySlots = 10;

    [Header("Idle / Offline")]
    [Tooltip("GDD: 최대 오프라인 시간 8시간 (초)")]
    public float maxOfflineSeconds = 8f * 3600f;

    [Header("Economy")]
    public int startingGold = 100;
    public int startingGems = 10;

    [Header("Upgrades (GDD: 15개 확장 가능)")]
    public int maxUpgradeLevel = 15;
}
