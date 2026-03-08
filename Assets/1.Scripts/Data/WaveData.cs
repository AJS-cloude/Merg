using UnityEngine;

/// <summary>
/// 디펜스 모드: 웨이브 1개 정의. 등장할 적 종류와 수.
/// </summary>
[CreateAssetMenu(fileName = "Wave_", menuName = "Merge Defense/Wave Data")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public EnemyData enemy;
        public int count = 1;
    }

    public string waveId;
    public int waveIndex;
    [Tooltip("이 웨이브에서 스폰할 적 목록")]
    public Entry[] entries;
    [Tooltip("다음 웨이브까지 대기 시간(초). 0이면 곧바로")]
    public float delayBeforeNext = 2f;
}
