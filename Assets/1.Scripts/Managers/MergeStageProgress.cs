using UnityEngine;

/// <summary>
/// 목표 점수/스테이지 게임에서 사용할 스테이지 번호. 손 UI·머지 필요 개수(2→3)에 사용.
/// </summary>
public static class MergeStageProgress
{
    const string KeyStage = "MergeFactory_CurrentStage";
    public const int DefaultHandsUnlockStage = 10;

    /// <summary>1부터 시작하는 스테이지 번호.</summary>
    public static int CurrentStage
    {
        get => PlayerPrefs.GetInt(KeyStage, 1);
        set
        {
            PlayerPrefs.SetInt(KeyStage, Mathf.Max(1, value));
            PlayerPrefs.Save();
            CharacterInventoryManager.Instance?.ApplyHandMigrationIfNeeded(value, HandsUnlockStage);
        }
    }

    public static int HandsUnlockStage => DefaultHandsUnlockStage;

    /// <summary>스테이지 10 이전: 2개 머지. 이후: 3개 머지.</summary>
    public static int MergePiecesRequired =>
        CurrentStage >= HandsUnlockStage ? 3 : 2;

    public static bool HandsVisible => CurrentStage >= HandsUnlockStage;
}
