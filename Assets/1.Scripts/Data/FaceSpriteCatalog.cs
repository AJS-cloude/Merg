using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>3.UI/Character/PNG/Double/Face</c> 등 얼굴 스프라이트 목록. 랜덤 선택에 사용합니다.
/// </summary>
[CreateAssetMenu(fileName = "FaceSpriteCatalog", menuName = "MergIdel/Face Sprite Catalog")]
public class FaceSpriteCatalog : ScriptableObject
{
    [Tooltip("face_a … 등 얼굴 스프라이트 전부.")]
    [SerializeField] List<Sprite> _faces = new();

    public IReadOnlyList<Sprite> Faces => _faces;

    public Sprite GetRandomFace()
    {
        BuildNonEmptyList();
        if (_nonEmpty.Count == 0)
            return null;

        return _nonEmpty[Random.Range(0, _nonEmpty.Count)];
    }

    readonly List<Sprite> _nonEmpty = new();

    void OnValidate()
    {
        _nonEmpty.Clear();
    }

    void BuildNonEmptyList()
    {
        if (_nonEmpty.Count > 0)
            return;

        if (_faces == null)
            return;

        for (int i = 0; i < _faces.Count; i++)
        {
            if (_faces[i] != null)
                _nonEmpty.Add(_faces[i]);
        }
    }
}
