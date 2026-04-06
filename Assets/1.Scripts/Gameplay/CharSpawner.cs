using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <b>Char</b> 프리팹을 스폰 지점마다 한 마리씩 생성합니다.
/// 합성으로 비워진 등록 스폰 지점은(옵션) 한 프레임 뒤 다시 채웁니다.
/// </summary>
public class CharSpawner : MonoBehaviour
{
    [Tooltip("생성할 Char 프리팹 (예: Assets/4.Prefab/Char).")]
    [SerializeField] GameObject _charPrefab;

    [Tooltip("소환 위치(Transform / RectTransform). null 항목은 건너뜁니다.")]
    [SerializeField] List<Transform> _spawnPoints = new();

    [Tooltip("플레이 시작 시 등록된 스폰 지점마다 자동 스폰합니다.")]
    [SerializeField] bool _spawnOnStart = true;

    [Tooltip("결합으로 Char가 사라진 슬롯이 등록된 스폰 지점이면, 같은 자리에 Char를 다시 생성합니다.")]
    [SerializeField] bool _refillFreedSpawnSlots = true;

    [Tooltip("비우기 후 재소환은 Destroy 처리가 끝난 다음 프레임에 시도합니다.")]
    [SerializeField] bool _refillOnlyRegisteredPoints = true;

    [Tooltip("스폰되는 Char UI 가로·세로 크기.")]
    [SerializeField] Vector2 _spawnedCharSize = new Vector2(160f, 160f);

    public IReadOnlyList<Transform> SpawnPoints => _spawnPoints;

    public bool IsRegisteredSpawnPoint(Transform slot) =>
        slot != null && _spawnPoints.Contains(slot);

    void Start()
    {
        if (_spawnOnStart)
            SpawnAll();
    }

    /// <summary>현재 등록된 모든 스폰 지점에 Char를 한 마리씩 생성합니다.</summary>
    [ContextMenu("Spawn All")]
    public void SpawnAll()
    {
        RectTransform layoutRoot = null;

        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            var p = _spawnPoints[i];
            if (p != null && layoutRoot == null && p.parent is RectTransform pr)
                layoutRoot = pr;

            SpawnAt(p);
            if (layoutRoot != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        }
    }

    /// <summary>
    /// 비워진 등록 스폰 슬롯에 Char를 한 마리 둡니다.
    /// 이미 자식이 없으면(예: 필드로 옮기며 부모만 바꾼 경우) 같은 프레임에 생성하고, Destroy 직후라면 다음 프레임에 시도합니다.
    /// </summary>
    public void RefillSpawnSlot(Transform slot)
    {
        if (!_refillFreedSpawnSlots || slot == null || _charPrefab == null)
            return;

        if (_refillOnlyRegisteredPoints && !_spawnPoints.Contains(slot))
            return;

        if (slot.childCount == 0)
        {
            SpawnAt(slot);

            if (slot.parent is RectTransform layoutRoot)
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);

            return;
        }

        StartCoroutine(RefillSlotAfterFrame(slot));
    }

    IEnumerator RefillSlotAfterFrame(Transform slot)
    {
        yield return null;

        if (slot == null || _charPrefab == null)
            yield break;

        if (_refillOnlyRegisteredPoints && !_spawnPoints.Contains(slot))
            yield break;

        if (slot.childCount > 0)
            yield break;

        SpawnAt(slot);

        if (slot.parent is RectTransform layoutRoot)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
    }

    /// <summary>스폰 지점을 추가합니다(중복은 무시).</summary>
    public void AddSpawnPoint(Transform point)
    {
        if (point == null || _spawnPoints.Contains(point))
            return;

        _spawnPoints.Add(point);
    }

    /// <summary>스폰 지점을 제거합니다. 제거 여부를 반환합니다.</summary>
    public bool RemoveSpawnPoint(Transform point)
    {
        return point != null && _spawnPoints.Remove(point);
    }

    /// <summary>스폰 지점 목록을 비웁니다.</summary>
    public void ClearSpawnPoints() => _spawnPoints.Clear();

    void SpawnAt(Transform point)
    {
        if (_charPrefab == null || point == null)
            return;

        var instance = Instantiate(_charPrefab, point);
        instance.name = _charPrefab.name;

        if (instance.TryGetComponent<RectTransform>(out var rect))
        {
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = _spawnedCharSize;
            rect.anchoredPosition3D = Vector3.zero;
        }
        else
        {
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }
    }
}
