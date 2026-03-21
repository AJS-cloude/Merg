using UnityEngine;

/// <summary>
/// 색상별 몸(4도형) + 표정 + 손 제스처 스프라이트 연결. 인스펙터에서 에셋 시트를 채웁니다.
/// </summary>
[CreateAssetMenu(fileName = "CharacterVisualCatalog", menuName = "Merge Factory/Character Visual Catalog")]
public class CharacterVisualCatalog : ScriptableObject
{
    [System.Serializable]
    public class ShapeSet
    {
        public Sprite square;
        public Sprite circle;
        public Sprite roundedSquare;
        public Sprite diamond;

        public Sprite GetByShapeIndex(int shapeIndex)
        {
            return shapeIndex switch
            {
                0 => square,
                1 => circle,
                2 => roundedSquare,
                3 => diamond,
                _ => square
            };
        }
    }

    [Tooltip("순서: Blue, Purple, Pink, Red, Yellow, Green")]
    public ShapeSet[] bodiesByColor = new ShapeSet[CharacterInstance.ColorCount];

    [Tooltip("표정 스프라이트 (인덱스 = faceIndex)")]
    public Sprite[] faces = new Sprite[CharacterInstance.MaxFaceIndex];

    [System.Serializable]
    public class HandSet
    {
        [Tooltip("6종 제스처, 인덱스 0~5 = 손 1~6")]
        public Sprite[] gestures = new Sprite[6];
    }

    [Tooltip("색상별 손 6종")]
    public HandSet[] handsByColor = new HandSet[CharacterInstance.ColorCount];

    public Sprite GetBody(int colorIndex, int shapeIndex)
    {
        if (bodiesByColor == null || colorIndex < 0 || colorIndex >= bodiesByColor.Length) return null;
        var set = bodiesByColor[colorIndex];
        return set == null ? null : set.GetByShapeIndex(shapeIndex);
    }

    public Sprite GetFace(int faceIndex)
    {
        if (faces == null || faces.Length == 0) return null;
        if (faceIndex < 0 || faceIndex >= faces.Length) return faces[0];
        return faces[faceIndex];
    }

    public Sprite GetHand(int colorIndex, int gestureIndex1To6)
    {
        if (gestureIndex1To6 < 1 || gestureIndex1To6 > 6) return null;
        if (handsByColor == null || colorIndex < 0 || colorIndex >= handsByColor.Length) return null;
        var hs = handsByColor[colorIndex];
        if (hs?.gestures == null) return null;
        int i = gestureIndex1To6 - 1;
        if (i < 0 || i >= hs.gestures.Length) return null;
        return hs.gestures[i];
    }
}
