using UnityEngine;

/// <summary>
/// 디펜스 모드: 적 1종류 데이터.
/// </summary>
[CreateAssetMenu(fileName = "Enemy_", menuName = "Merge Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyId;
    public string displayName;
    [Header("Stats")]
    public float maxHp = 10f;
    public float moveSpeed = 2f;
    public int goldReward = 5;
    [Header("Visual")]
    public Sprite icon;
    public Color tint = Color.white;
}
