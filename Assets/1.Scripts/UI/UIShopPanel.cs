using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GDD: Shop - 골드/젬으로 아이템 구매 등. 확장용.
/// </summary>
public class UIShopPanel : MonoBehaviour
{
    [SerializeField] Button buyStarterButton;
    [SerializeField] int starterCost = 20;
    [SerializeField] ItemData starterItem;

    void Start()
    {
        if (buyStarterButton != null && starterItem != null)
            buyStarterButton.onClick.AddListener(OnBuyStarter);
    }

    void OnBuyStarter()
    {
        if (EconomyManager.Instance == null || !EconomyManager.Instance.TrySpendGold(starterCost)) return;
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(starterItem);
    }
}
