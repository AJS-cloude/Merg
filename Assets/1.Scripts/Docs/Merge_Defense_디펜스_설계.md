# Merge Defense — 디펜스 모드 설계

기존 **Merge + Idle + 경제** 시스템을 그대로 쓰고, **디펜스(방어)** 요소를 붙인 모드입니다.

---

## 1. 컨셉

- **Merge**: 같은 타워 2개 합치면 상위 타워 (기존 ItemData + 공격력/사거리)
- **슬롯**: 공장 슬롯처럼 **방어 슬롯**에 타워 배치 → 자동으로 적 공격
- **웨이브**: 적이 일정 간격으로 등장, 기지까지 도달하면 기지 HP 감소
- **경제**: 적 처치 시 골드, 골드로 업그레이드·Merge 유지
- **승패**: 기지 HP 0 → 패배. 원하는 웨이브까지 방어 → 승리(무한 웨이브도 가능)

---

## 2. 기존 시스템 활용

| 기존 | 디펜스에서 역할 |
|------|------------------|
| MergeManager | 타워 Merge (동일 타워 2개 → 상위 타워) |
| InventoryManager | 보유 타워 목록 (Merge 보드 + 슬롯 배치 소스) |
| EconomyManager | 골드/젬 (적 처치 보상, 업그레이드) |
| FactoryManager | 공장 모드: 골드 생산. 디펜스 모드에서는 사용 안 하거나 별도 씬에서만 공장 |
| DefenseManager | **신규** — 방어 슬롯 + 타워 자동 공격 |
| WaveManager | **신규** — 웨이브 스폰 |
| BaseHealthManager | **신규** — 기지 HP, 피격 시 감소, 게임 오버 |

---

## 3. 디펜스 전용 데이터

- **ItemData**  
  - 기존 + `attackDamage`, `attackRange`, `attackInterval`  
  - 이 값이 있으면 디펜스 슬롯에 “타워”로 배치 가능.
- **EnemyData** (ScriptableObject)  
  - HP, 이동 속도, 처치 시 골드, 스프라이트.
- **WaveData** (ScriptableObject)  
  - 웨이브 1개 정의: 등장 적 종류와 수.

---

## 4. 플레이 루프 (디펜스)

1. 메인 화면: Merge로 타워 레벨 업 → 방어 슬롯에 타워 배치.
2. 웨이브 시작: 적 스폰 → 전방으로 이동.
3. 슬롯 타워가 사거리 안의 적을 주기적으로 공격.
4. 적 처치 시 골드 지급.
5. 적이 기지에 도달하면 기지 HP 감소.
6. 기지 HP 0 → 게임 오버. 다음 웨이브 준비 시 Merge/슬롯 재배치 후 재도전.

---

## 5. 씬 구성 옵션

- **A**: 메인 씬 하나에서 “공장 탭” / “디펜스 탭” 전환 (같은 Merge·경제 공유).
- **B**: 메인 씬(공장+Merge) + 디펜스 전용 씬(전투만). 디펜스 씬에서도 동일 매니저 사용.

현재 구현은 **디펜스 전용 매니저 + 씬**을 추가하는 방식으로 두고, 필요 시 메인과 탭/씬 전환으로 연결할 수 있게 둡니다.

---

## 6. 폴더/스크립트 추가

- `Data/`: EnemyData, WaveData (기존 ItemData 확장은 완료)
- `Managers/`: WaveManager, DefenseManager, BaseHealthManager
- `Gameplay/`: Enemy (MonoBehaviour), TowerSlot (또는 DefenseManager가 슬롯 관리)
- `UI/`: UIWave, UIBaseHP, UIGameOver

이 설계를 기준으로 실제 스크립트와 씬을 구성하면 됩니다.

---

## 7. 디펜스 씬 구성 요약

### Hierarchy
- **GameBootstrap** (기존) — Config, All Item Data (타워용 ItemData에 attackDamage 등 설정)
- **Managers**: WaveManager, DefenseManager, BaseHealthManager (+ 기존 Economy, Inventory, Merge 등)
- **SpawnPoint** — 빈 오브젝트, 적 스폰 위치 (Transform)
- **BaseTarget** — 빈 오브젝트, 적 이동 목표(기지 위치). BaseHealthManager와 같은 오브젝트 또는 근처
- **Canvas**
  - UIDefenseWave (웨이브 번호)
  - UIDefenseBaseHP (기지 HP 바)
  - UIDefenseStartWave (웨이브 시작 버튼)
  - DefenseSlots (자식에 UIDefenseSlot 0~9, slotIndex 지정)
  - UIDefenseSlotPicker (패널 + 버튼 컨테이너 + 타워 버튼 프리팹)
  - UIDefenseGameOver (게임 오버 패널 + 재시도 버튼)
- **Enemy** 프리팹: Enemy.cs + Collider/Image 등. WaveManager에 할당

### 필수 연결
- **WaveManager**: waves 배열, spawnPoint, baseTarget, enemyPrefab
- **DefenseManager**: maxSlots, slotTransforms(선택) 또는 slotAttackOrigin
- **BaseHealthManager**: maxHp
- **Enemy** 프리팹: SetMoveTarget으로 baseTarget 전달됨 (WaveManager가 자동 설정)

### ItemData 타워 설정
디펜스에서 슬롯에 넣을 아이템은 **attackDamage > 0**으로 설정 (예: 5, 10, 20 …). attackRange, attackInterval도 설정.
