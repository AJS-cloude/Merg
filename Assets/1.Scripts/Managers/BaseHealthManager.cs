using System;
using UnityEngine;

/// <summary>
/// 디펜스: 기지 HP. 적이 도달 시 감소, 0이면 게임 오버.
/// </summary>
public class BaseHealthManager : MonoBehaviour
{
    public static BaseHealthManager Instance { get; private set; }

    [SerializeField] int maxHp = 10;
    int _hp;

    public int CurrentHp => _hp;
    public int MaxHp => maxHp;
    public bool IsAlive => _hp > 0;
    public event Action<int, int> OnHpChanged;
    public event Action OnGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _hp = maxHp;
        OnHpChanged?.Invoke(_hp, maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (_hp <= 0) return;
        _hp = Mathf.Max(0, _hp - damage);
        OnHpChanged?.Invoke(_hp, maxHp);
        if (_hp <= 0)
            OnGameOver?.Invoke();
    }

    public void Heal(int amount)
    {
        _hp = Mathf.Min(maxHp, _hp + amount);
        OnHpChanged?.Invoke(_hp, maxHp);
    }

    public void ResetHp()
    {
        _hp = maxHp;
        OnHpChanged?.Invoke(_hp, maxHp);
    }
}
