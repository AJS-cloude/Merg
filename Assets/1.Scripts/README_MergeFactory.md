# Merge Factory (GDD 구현)

**씬 종류와 스크립트 배치:** [`Scripts/Docs/씬_및_스크립트_배치_가이드.md`](Docs/씬_및_스크립트_배치_가이드.md) 참고.

## 폴더 구조
- **Data**: ItemData, GameConfig, UpgradeData (ScriptableObject)
- **Managers**: GameManager, MergeManager, FactoryManager, EconomyManager, SaveManager, InventoryManager, UpgradeManager, AdManager
- **Gameplay**: MergeBoard
- **UI**: UICurrencyBar, UIInventoryGrid, UIMergeSlot, UIFactorySlot, UIFactorySlotPicker, UIMenuPanel, UIShopPanel, UIUpgradePanel
- **Core**: GameBootstrap
- **Editor**: CreateMergeFactoryDefaults (기본 데이터 생성)

## 씬 설정 순서
1. **Tools > Merge Factory > Create Default Data** 실행 → `Assets/Data`에 GameConfig, 아이템(Lv1~6), 업그레이드 생성
2. 빈 GameObject 생성 후 **GameBootstrap** 추가
   - Config: Assets/Data/GameConfig
   - All Item Data: Data/Items 폴더의 ItemData 6개 드래그
   - Upgrades: Data/Upgrades 폴더의 UpgradeData 드래그
3. Canvas 하위에:
   - **상단**: UICurrencyBar (Gold/Gems Text 연결)
   - **중앙**: UIInventoryGrid (Slot Container, Slot Prefab 연결) + MergeBoard (Initial Spawn Item = 철 조각, Count = 3)
   - **하단**: Factory 슬롯 10개 — 각각 UIFactorySlot (Setup(slotIndex) 호출은 코드에서 또는 런타임에)
4. UIFactorySlotPicker: Panel, Button Container, Item Button Prefab, All Items 배열
5. 메뉴: UIMenuPanel (Shop / Upgrade / Inventory 버튼·패널 연결)

## 플레이 루프
- 인벤토리에서 같은 아이템 2개 클릭 → Merge → 상위 아이템 1개 생성
- 공장 슬롯 클릭 → 비어 있으면 인벤토리에서 선택해 배치, 있으면 슬롯에서 제거해 인벤토리로
- 슬롯에 배치된 아이템은 자동으로 골드 생산 (업그레이드·광고 배율 적용)
- 오프라인 최대 8시간까지 자동 생산 후 재접속 시 보상

## 저장
- SaveManager가 OnApplicationPause/OnApplicationQuit 시 자동 저장
- Gold, Gems, 공장 슬롯, 인벤토리 저장
