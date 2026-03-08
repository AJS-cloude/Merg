using UnityEngine;

/// <summary>
/// 디펜스: 적 유닛. 목표 지점으로 이동, 피격 시 HP 감소, 사망 시 골드.
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField] Transform moveTarget;
    [SerializeField] UnityEngine.UI.Image iconImage;

    EnemyData _data;
    float _hp;
    bool _dead;

    public EnemyData Data => _data;
    public float Hp => _hp;
    public bool IsDead => _dead;
    public Vector3 Position => transform.position;

    public void Setup(EnemyData data)
    {
        _data = data;
        _hp = data != null ? data.maxHp : 0f;
        _dead = false;
        if (iconImage != null && data != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.color = data.tint;
        }
    }

    public void SetMoveTarget(Transform target) => moveTarget = target;

    void Update()
    {
        if (_dead || _data == null) return;
        if (moveTarget != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                moveTarget.position,
                _data.moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, moveTarget.position) < 0.1f)
                ReachedBase();
        }
    }

    public void TakeDamage(float damage)
    {
        if (_dead) return;
        _hp -= damage;
        if (_hp <= 0f)
            Die();
    }

    void Die()
    {
        if (_dead) return;
        _dead = true;
        if (EconomyManager.Instance != null && _data != null)
            EconomyManager.Instance.AddGold(_data.goldReward);
        if (DefenseManager.Instance != null)
            DefenseManager.Instance.NotifyEnemyKilled(this);
        Destroy(gameObject);
    }

    public void ReachedBase()
    {
        if (_dead) return;
        _dead = true;
        if (BaseHealthManager.Instance != null)
            BaseHealthManager.Instance.TakeDamage(1);
        if (DefenseManager.Instance != null)
            DefenseManager.Instance.NotifyEnemyReachedBase(this);
        Destroy(gameObject);
    }
}
