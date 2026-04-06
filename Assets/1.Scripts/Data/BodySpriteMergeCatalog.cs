using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>에셋 이름 접두어와 동일한 소문자 (예: blue_body_squircle).</summary>
public enum BodyColorId
{
    Blue,
    Green,
    Pink,
    Purple,
    Red,
    Yellow,
}

/// <summary>
/// Body 스프라이트 이름 규칙: <c>{색상}_body_{모양}</c>
/// 합성 단계(낮음→높음): squircle → square → rhombus → circle
/// </summary>
[CreateAssetMenu(fileName = "BodySpriteMergeCatalog", menuName = "MergIdel/Body Sprite Merge Catalog")]
public class BodySpriteMergeCatalog : ScriptableObject
{
    /// <summary>1단 squircle … 최종 circle</summary>
    public static readonly string[] ShapeTierOrder = { "squircle", "square", "rhombus", "circle" };

    [Tooltip("Body 폴더 스프라이트 전부(24장)을 넣어도 되고, 사용하는 것만 넣어도 됩니다. 이름으로 다음 단계를 찾습니다.")]
    [SerializeField] List<Sprite> _bodySprites = new();

    Dictionary<string, Sprite> _byName;

    void OnValidate()
    {
        _byName = null;
    }

    void BuildMap()
    {
        if (_byName != null)
            return;

        _byName = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        if (_bodySprites == null)
            return;

        foreach (var s in _bodySprites)
        {
            if (s != null && !_byName.ContainsKey(s.name))
                _byName[s.name] = s;
        }
    }

    /// <summary>1단 squircle 스프라이트.</summary>
    public bool TryGetSquircle(BodyColorId color, out Sprite sprite)
    {
        return TryGetSquircle(color.ToString().ToLowerInvariant(), out sprite);
    }

    /// <param name="colorPrefix">파일명 접두어 (예: blue).</param>
    public bool TryGetSquircle(string colorPrefix, out Sprite sprite)
    {
        BuildMap();
        sprite = null;
        if (string.IsNullOrEmpty(colorPrefix))
            return false;

        string key = $"{colorPrefix.ToLowerInvariant()}_body_squircle";
        return _byName.TryGetValue(key, out sprite);
    }

    public static bool TryParseBodySpriteName(string spriteName, out string color, out string shape)
    {
        color = null;
        shape = null;

        if (string.IsNullOrEmpty(spriteName))
            return false;

        const string mid = "_body_";
        int i = spriteName.IndexOf(mid, StringComparison.Ordinal);
        if (i <= 0)
            return false;

        color = spriteName.Substring(0, i);
        shape = spriteName.Substring(i + mid.Length);
        return !string.IsNullOrEmpty(color) && !string.IsNullOrEmpty(shape);
    }

    /// <summary>현재 단계의 바로 위 단계 스프라이트(circle이면 false).</summary>
    public bool TryGetNextTier(Sprite current, out Sprite next)
    {
        BuildMap();
        next = null;
        if (current == null || !TryParseBodySpriteName(current.name, out var color, out var shape))
            return false;

        int tier = Array.IndexOf(ShapeTierOrder, shape);
        if (tier < 0 || tier >= ShapeTierOrder.Length - 1)
            return false;

        string key = $"{color}_body_{ShapeTierOrder[tier + 1]}";
        return _byName.TryGetValue(key, out next);
    }

    /// <summary>같은 종류끼리이고, 아직 다음 단계가 있으면 합칠 수 있음.</summary>
    public bool CanMerge(Sprite a, Sprite b, out Sprite nextTier)
    {
        nextTier = null;
        if (a == null || b == null)
            return false;

        bool samePiece = ReferenceEquals(a, b)
                         || string.Equals(a.name, b.name, StringComparison.Ordinal);
        if (!samePiece)
            return false;

        return TryGetNextTier(a, out nextTier);
    }
}
