using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GDD: 메뉴 - Shop / Upgrade / Inventory (패널 토글).
/// </summary>
public class UIMenuPanel : MonoBehaviour
{
    [SerializeField] Button shopButton;
    [SerializeField] Button upgradeButton;
    [SerializeField] Button inventoryButton;
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject upgradePanel;
    [SerializeField] GameObject inventoryPanel;

    void Start()
    {
        if (shopButton != null) shopButton.onClick.AddListener(() => OpenPanel(shopPanel));
        if (upgradeButton != null) upgradeButton.onClick.AddListener(() => OpenPanel(upgradePanel));
        if (inventoryButton != null) inventoryButton.onClick.AddListener(() => OpenPanel(inventoryPanel));
        OpenPanel(null);
    }

    void OpenPanel(GameObject panel)
    {
        if (shopPanel != null) shopPanel.SetActive(shopPanel == panel);
        if (upgradePanel != null) upgradePanel.SetActive(upgradePanel == panel);
        if (inventoryPanel != null) inventoryPanel.SetActive(inventoryPanel == panel);
    }
}
