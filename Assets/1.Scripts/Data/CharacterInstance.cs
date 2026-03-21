using System;
using UnityEngine;

/// <summary>
/// 머지 보드용 모듈형 캐릭터 1개. 색 6종 × 도형 4단계(네모→원→둥근사각→마름모) + 표정 + 손(스테이지 N 이후).
/// </summary>
[Serializable]
public struct CharacterInstance : IEquatable<CharacterInstance>
{
    public const int ColorCount = 6;
    public const int ShapeCount = 4;
    public const int MaxFaceIndex = 12;

    /// <summary>0=Blue … 5=Green</summary>
    public byte colorIndex;
    /// <summary>0=Square, 1=Circle, 2=RoundedSquare, 3=Diamond</summary>
    public byte shapeIndex;
    public byte faceIndex;
    /// <summary>0=손 없음(표시만), 1~6=제스처. 스테이지 이후 머지 시 매칭에 포함.</summary>
    public byte handGesture;
    public bool isEmpty;

    public static CharacterInstance Empty => new CharacterInstance { isEmpty = true };

    public bool IsMaxTier => !isEmpty && colorIndex == ColorCount - 1 && shapeIndex == ShapeCount - 1;

    public bool EqualsMergeIdentity(CharacterInstance other)
    {
        if (isEmpty || other.isEmpty) return false;
        return colorIndex == other.colorIndex
               && shapeIndex == other.shapeIndex
               && faceIndex == other.faceIndex
               && handGesture == other.handGesture;
    }

    public bool Equals(CharacterInstance other) => EqualsMergeIdentity(other) && isEmpty == other.isEmpty;

    public override bool Equals(object obj) => obj is CharacterInstance other && Equals(other);

    public override int GetHashCode()
    {
        if (isEmpty) return -1;
        return HashCode.Combine(colorIndex, shapeIndex, faceIndex, handGesture);
    }

    /// <summary>동일한 조각 2~3개를 합친 뒤 결과 1개. 손은 스테이지에 따라 새로 부여.</summary>
    public static CharacterInstance ResultAfterMerge(CharacterInstance mergedPiece, int currentStage, int handsUnlockStage)
    {
        if (mergedPiece.isEmpty) return Empty;
        if (mergedPiece.IsMaxTier) return mergedPiece;

        byte c = mergedPiece.colorIndex;
        byte s = mergedPiece.shapeIndex;

        if (s < ShapeCount - 1)
            s++;
        else if (c < ColorCount - 1)
        {
            c++;
            s = 0;
        }
        else
            return mergedPiece;

        var r = new CharacterInstance
        {
            colorIndex = c,
            shapeIndex = s,
            faceIndex = mergedPiece.faceIndex,
            handGesture = 0,
            isEmpty = false
        };

        if (currentStage >= handsUnlockStage)
            r.handGesture = (byte)UnityEngine.Random.Range(1, 7);

        return r;
    }

    /// <summary>스테이지 손 해금 시 기존 조각에 손이 없으면 랜덤 부여.</summary>
    public static CharacterInstance EnsureHandForStage(CharacterInstance piece, int currentStage, int handsUnlockStage)
    {
        if (piece.isEmpty || currentStage < handsUnlockStage) return piece;
        if (piece.handGesture != 0) return piece;
        var p = piece;
        p.handGesture = (byte)UnityEngine.Random.Range(1, 7);
        return p;
    }

    public static CharacterInstance CreateStarter(byte color = 0, byte face = 0)
    {
        return new CharacterInstance
        {
            colorIndex = color,
            shapeIndex = 0,
            faceIndex = face,
            handGesture = 0,
            isEmpty = false
        };
    }

    public string ToSaveString()
    {
        if (isEmpty) return "";
        return $"{colorIndex},{shapeIndex},{faceIndex},{handGesture}";
    }

    public static CharacterInstance FromSaveString(string s)
    {
        if (string.IsNullOrEmpty(s)) return Empty;
        var p = s.Split(',');
        if (p.Length < 4) return Empty;
        if (!byte.TryParse(p[0], out var c)) return Empty;
        if (!byte.TryParse(p[1], out var sh)) return Empty;
        if (!byte.TryParse(p[2], out var f)) return Empty;
        if (!byte.TryParse(p[3], out var h)) return Empty;
        return new CharacterInstance
        {
            colorIndex = c,
            shapeIndex = sh,
            faceIndex = f,
            handGesture = h,
            isEmpty = false
        };
    }
}
