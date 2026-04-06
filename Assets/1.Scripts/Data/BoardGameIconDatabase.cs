using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kenney Board Game Icons(128px Double) 등 스프라이트 이름 → <see cref="Sprite"/> 조회용 카탈로그.
/// 에디터 메뉴 <b>MergIdel / Rebuild Board Game Icon Database</b>로 폴더를 다시 스캔합니다.
/// </summary>
[CreateAssetMenu(fileName = "BoardGameIconDatabase", menuName = "MergIdel/Board Game Icon Database (empty)")]
public class BoardGameIconDatabase : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        [Tooltip("파일명에서 확장자를 뺀 id (예: hand_hexagon).")]
        public string id;

        public Sprite sprite;
    }

    [SerializeField] List<Entry> _entries = new();

    readonly Dictionary<string, Sprite> _byId = new(StringComparer.OrdinalIgnoreCase);

    void OnEnable() => RefreshLookup();

    public void RefreshLookup()
    {
        _byId.Clear();

        if (_entries == null)
            return;

        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            if (string.IsNullOrEmpty(e.id) || e.sprite == null)
                continue;

            _byId[e.id] = e.sprite;
        }
    }

    public IReadOnlyList<Entry> Entries => _entries;

    public bool TryGetSprite(string id, out Sprite sprite)
    {
        if (string.IsNullOrEmpty(id))
        {
            sprite = null;
            return false;
        }

        return _byId.TryGetValue(id, out sprite);
    }

    public Sprite GetSpriteOrNull(string id) =>
        TryGetSprite(id, out var s) ? s : null;
}
