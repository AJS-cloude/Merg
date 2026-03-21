using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Merge Board — 레거시(ItemData 2개) 또는 모듈형 캐릭터(2~3개, 스테이지별).
/// </summary>
public class MergeBoard : MonoBehaviour
{
    public static MergeBoard Instance { get; private set; }

    [Header("Mode")]
    [SerializeField] bool useCharacterMerge = true;

    [Header("Legacy (ItemData)")]
    [SerializeField] ItemData initialSpawnItem;
    [SerializeField] int initialSpawnCount = 3;

    [Header("Character merge")]
    [SerializeField] byte starterColorIndex;
    [SerializeField] int characterInitialSpawnCount = 3;

    int _legacySelectedIndex = -1;
    readonly List<int> _selectedIndices = new List<int>();

    public bool UseCharacterMerge => useCharacterMerge;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (useCharacterMerge)
        {
            if (CharacterInventoryManager.Instance == null) return;
            if (CharacterInventoryManager.Instance.Count == 0 && characterInitialSpawnCount > 0)
            {
                for (int i = 0; i < characterInitialSpawnCount; i++)
                {
                    byte face = (byte)Random.Range(0, CharacterInstance.MaxFaceIndex);
                    CharacterInventoryManager.Instance.AddPiece(CharacterInstance.CreateStarter(starterColorIndex, face));
                }
            }
            CharacterInventoryManager.Instance.ApplyHandMigrationIfNeeded(
                MergeStageProgress.CurrentStage, MergeStageProgress.HandsUnlockStage);
        }
        else
        {
            if (InventoryManager.Instance == null) return;
            if (InventoryManager.Instance.Count == 0 && initialSpawnItem != null && initialSpawnCount > 0)
            {
                for (int i = 0; i < initialSpawnCount; i++)
                    InventoryManager.Instance.AddItem(initialSpawnItem);
            }
        }
    }

    public void OnItemClicked(int inventoryIndex)
    {
        if (useCharacterMerge)
            OnCharacterSlotClicked(inventoryIndex);
        else
            OnLegacyItemClicked(inventoryIndex);
    }

    void OnLegacyItemClicked(int inventoryIndex)
    {
        if (InventoryManager.Instance == null || MergeManager.Instance == null) return;
        var item = InventoryManager.Instance.GetAt(inventoryIndex);
        if (item == null) return;

        if (_legacySelectedIndex < 0)
        {
            _legacySelectedIndex = inventoryIndex;
            return;
        }

        if (_legacySelectedIndex == inventoryIndex)
        {
            _legacySelectedIndex = -1;
            return;
        }

        var first = InventoryManager.Instance.GetAt(_legacySelectedIndex);
        if (first != item || !MergeManager.Instance.CanMerge(first, item))
        {
            _legacySelectedIndex = inventoryIndex;
            return;
        }

        var result = MergeManager.Instance.GetMergeResult(item);
        if (InventoryManager.Instance.ConsumeTwoAndAddResult(item, result))
            MergeManager.Instance.NotifyMerged(first, item, result);
        _legacySelectedIndex = -1;
    }

    void OnCharacterSlotClicked(int index)
    {
        if (CharacterInventoryManager.Instance == null) return;
        var piece = CharacterInventoryManager.Instance.GetAt(index);

        if (piece.isEmpty)
        {
            _selectedIndices.Clear();
            return;
        }

        if (_selectedIndices.Contains(index))
        {
            _selectedIndices.Remove(index);
            return;
        }

        if (_selectedIndices.Count == 0)
        {
            _selectedIndices.Add(index);
            return;
        }

        var first = CharacterInventoryManager.Instance.GetAt(_selectedIndices[0]);
        if (!piece.EqualsMergeIdentity(first))
        {
            _selectedIndices.Clear();
            _selectedIndices.Add(index);
            return;
        }

        int need = MergeStageProgress.MergePiecesRequired;
        if (_selectedIndices.Count >= need)
        {
            _selectedIndices.Clear();
            _selectedIndices.Add(index);
            return;
        }

        _selectedIndices.Add(index);
        if (_selectedIndices.Count == need)
            TryCharacterMerge();
    }

    void TryCharacterMerge()
    {
        int need = MergeStageProgress.MergePiecesRequired;
        if (_selectedIndices.Count != need) return;
        if (CharacterInventoryManager.Instance == null) return;

        var piece = CharacterInventoryManager.Instance.GetAt(_selectedIndices[0]);
        if (piece.isEmpty || piece.IsMaxTier)
        {
            ClearSelection();
            return;
        }

        for (int i = 1; i < need; i++)
        {
            if (!CharacterInventoryManager.Instance.GetAt(_selectedIndices[i]).EqualsMergeIdentity(piece))
            {
                ClearSelection();
                return;
            }
        }

        var result = CharacterInstance.ResultAfterMerge(
            piece,
            MergeStageProgress.CurrentStage,
            MergeStageProgress.HandsUnlockStage);

        if (result.isEmpty)
        {
            ClearSelection();
            return;
        }

        var indices = _selectedIndices.ToArray();
        CharacterInventoryManager.Instance.ConsumeIndicesAndAddResult(indices, result);
        ClearSelection();
    }

    public void ClearSelection()
    {
        _legacySelectedIndex = -1;
        _selectedIndices.Clear();
    }

    public int GetSelectedIndex() => useCharacterMerge
        ? (_selectedIndices.Count > 0 ? _selectedIndices[0] : -1)
        : _legacySelectedIndex;

    public bool IsIndexSelected(int index)
    {
        if (useCharacterMerge)
            return _selectedIndices.Contains(index);
        return index == _legacySelectedIndex;
    }

    public void SpawnItem(ItemData item)
    {
        if (item != null && InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(item);
    }
}
