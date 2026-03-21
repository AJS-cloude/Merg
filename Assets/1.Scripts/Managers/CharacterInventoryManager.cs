using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모듈형 캐릭터 전용 인벤토리 (머지 보드). 기존 ItemData 인벤과 별도.
/// </summary>
public class CharacterInventoryManager : MonoBehaviour
{
    public static CharacterInventoryManager Instance { get; private set; }

    readonly List<CharacterInstance> _items = new List<CharacterInstance>();

    public int Count => _items.Count;

    public event Action OnChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public CharacterInstance GetAt(int index) =>
        index >= 0 && index < _items.Count ? _items[index] : CharacterInstance.Empty;

    public void AddPiece(CharacterInstance piece)
    {
        if (piece.isEmpty) return;
        _items.Add(piece);
        OnChanged?.Invoke();
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count) return false;
        _items.RemoveAt(index);
        OnChanged?.Invoke();
        return true;
    }

    /// <summary>indices 오름차순이 아니어도 됨 — 높은 인덱스부터 제거.</summary>
    public bool ConsumeIndicesAndAddResult(int[] indices, CharacterInstance result)
    {
        if (indices == null || indices.Length == 0) return false;
        var sorted = new List<int>(indices);
        sorted.Sort((a, b) => b.CompareTo(a));
        foreach (var i in sorted)
        {
            if (i < 0 || i >= _items.Count) return false;
        }
        foreach (var i in sorted)
            _items.RemoveAt(i);

        if (!result.isEmpty)
            _items.Add(result);

        OnChanged?.Invoke();
        return true;
    }

    public IReadOnlyList<CharacterInstance> GetAllForSave() => _items;

    public void LoadFromSave(IReadOnlyList<string> entries)
    {
        _items.Clear();
        if (entries == null) return;
        foreach (var line in entries)
        {
            var p = CharacterInstance.FromSaveString(line);
            if (!p.isEmpty)
                _items.Add(p);
        }
        OnChanged?.Invoke();
    }

    /// <summary>스테이지 10 진입 시 손 없는 조각에 손 부여.</summary>
    public void ApplyHandMigrationIfNeeded(int currentStage, int handsUnlockStage)
    {
        if (currentStage < handsUnlockStage) return;
        for (int i = 0; i < _items.Count; i++)
        {
            var p = _items[i];
            if (p.isEmpty || p.handGesture != 0) continue;
            _items[i] = CharacterInstance.EnsureHandForStage(p, currentStage, handsUnlockStage);
        }
    }
}
