using UnityEngine;

/// <summary>
/// GDD: Merge Board - 같은 아이템 2개 선택 시 Merge 실행.
/// </summary>
public class MergeBoard : MonoBehaviour
{
    public static MergeBoard Instance { get; private set; }

    [SerializeField] ItemData initialSpawnItem;
    [SerializeField] int initialSpawnCount = 3;

    int _selectedIndex = -1;

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
        if (InventoryManager.Instance == null) return;
        if (InventoryManager.Instance.Count == 0 && initialSpawnItem != null && initialSpawnCount > 0)
        {
            for (int i = 0; i < initialSpawnCount; i++)
                InventoryManager.Instance.AddItem(initialSpawnItem);
        }
    }

    public void OnItemClicked(int inventoryIndex)
    {
        if (InventoryManager.Instance == null || MergeManager.Instance == null) return;
        var item = InventoryManager.Instance.GetAt(inventoryIndex);
        if (item == null) return;

        if (_selectedIndex < 0)
        {
            _selectedIndex = inventoryIndex;
            return;
        }

        if (_selectedIndex == inventoryIndex)
        {
            _selectedIndex = -1;
            return;
        }

        var first = InventoryManager.Instance.GetAt(_selectedIndex);
        if (first != item || !MergeManager.Instance.CanMerge(first, item))
        {
            _selectedIndex = inventoryIndex;
            return;
        }

        var result = MergeManager.Instance.GetMergeResult(item);
        if (InventoryManager.Instance.ConsumeTwoAndAddResult(item, result))
        {
            MergeManager.Instance.NotifyMerged(first, item, result);
        }
        _selectedIndex = -1;
    }

    public void ClearSelection() => _selectedIndex = -1;
    public int GetSelectedIndex() => _selectedIndex;

    public void SpawnItem(ItemData item)
    {
        if (item != null && InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(item);
    }
}
