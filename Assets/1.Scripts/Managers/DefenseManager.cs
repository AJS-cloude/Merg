using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 디펜스: 방어 슬롯(타워) 관리 + 사거리 내 적 자동 공격.
/// </summary>
public class DefenseManager : MonoBehaviour
{
    public static DefenseManager Instance { get; private set; }

    [SerializeField] int maxSlots = 10;
    [Tooltip("슬롯별 공격 원점(월드). 비어 있으면 slotAttackOrigin 또는 (0,0,0) 사용")]
    [SerializeField] Transform[] slotTransforms;
    [SerializeField] Transform slotAttackOrigin;
    readonly List<ItemData> _slots = new List<ItemData>();
    readonly List<float> _nextAttackTime = new List<float>();
    readonly List<Enemy> _liveEnemies = new List<Enemy>();

    public int MaxSlots => maxSlots;
    public event Action<int, ItemData> OnSlotChanged;
    public event Action<Enemy> OnEnemyKilled;
    public event Action<Enemy> OnEnemyReachedBase;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemySpawned += RegisterEnemy;
    }

    void OnDisable()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemySpawned -= RegisterEnemy;
    }

    void RegisterEnemy(Enemy e)
    {
        if (e != null && !_liveEnemies.Contains(e))
            _liveEnemies.Add(e);
    }

    public void NotifyEnemyKilled(Enemy e)
    {
        _liveEnemies.Remove(e);
        OnEnemyKilled?.Invoke(e);
    }

    public void NotifyEnemyReachedBase(Enemy e)
    {
        _liveEnemies.Remove(e);
        OnEnemyReachedBase?.Invoke(e);
    }

    void Update()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] == null || !_slots[i].IsTower) continue;
            if (Time.time < _nextAttackTime[i]) continue;
            Enemy target = GetNearestEnemyInRange(GetSlotWorldPosition(i), _slots[i].attackRange);
            if (target != null)
            {
                target.TakeDamage(_slots[i].attackDamage);
                _nextAttackTime[i] = Time.time + _slots[i].attackInterval;
            }
        }
    }

    Vector3 GetSlotWorldPosition(int index)
    {
        if (slotTransforms != null && index >= 0 && index < slotTransforms.Length && slotTransforms[index] != null)
            return slotTransforms[index].position;
        if (slotAttackOrigin != null)
            return slotAttackOrigin.position;
        return Vector3.zero;
    }

    Enemy GetNearestEnemyInRange(Vector3 from, float range)
    {
        Enemy nearest = null;
        float minDist = range * range;
        for (int i = _liveEnemies.Count - 1; i >= 0; i--)
        {
            var e = _liveEnemies[i];
            if (e == null || e.IsDead) { _liveEnemies.RemoveAt(i); continue; }
            float sq = (e.Position - from).sqrMagnitude;
            if (sq <= minDist) { minDist = sq; nearest = e; }
        }
        return nearest;
    }

    public ItemData GetSlot(int index)
    {
        if (index < 0 || index >= _slots.Count) return null;
        return _slots[index];
    }

    public bool SetSlot(int index, ItemData tower)
    {
        if (index < 0 || index >= maxSlots) return false;
        while (_slots.Count <= index) { _slots.Add(null); _nextAttackTime.Add(0f); }
        _slots[index] = tower;
        if (_nextAttackTime.Count <= index) _nextAttackTime.Add(0f);
        OnSlotChanged?.Invoke(index, tower);
        return true;
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= _slots.Count) return;
        _slots[index] = null;
        OnSlotChanged?.Invoke(index, null);
    }

    public void SetAttackOrigin(Transform t) => slotAttackOrigin = t;
}
