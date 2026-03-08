# UI 화면 비율 고정 가이드

다양한 해상도·화면 비율에서도 UI 크기가 **항상 동일한 비율**로 보이도록 하는 설정입니다.

---

## 1. Canvas Scaler 설정 (필수)

### 방법 A: 스크립트로 자동 적용 (권장)

1. **Canvas** 루트 오브젝트 선택
2. **UIScreenScaler** 컴포넌트 추가 (Canvas Scaler는 자동으로 추가됨)
3. 인스펙터에서:
   - **Reference Resolution**: `1080 x 1920` (모바일 세로 기준)
   - **Match Width Or Height**: `0.5` (가로/세로 균형, 세로 UI는 `0.5`~`1` 권장)

이렇게 하면 실행 시 항상 **Scale With Screen Size** + **Match Width Or Height** 로 적용됩니다.

### 방법 B: 메뉴로 한 번에 적용

1. Hierarchy에서 **Canvas** 선택
2. 메뉴 **Tools → Merge Factory → Setup Canvas For Screen Ratio** 실행
3. Canvas Scaler가 설정되고, 없으면 **UIScreenScaler**도 추가됩니다.

### Match 값 설명

| Match | 동작 | 추천 |
|-------|------|------|
| **0** | 너비 기준. 화면이 넓어지면 UI가 커짐 | 가로 UI |
| **0.5** | 너비·높이 균형 | 세로 모바일 UI (권장) |
| **1** | 높이 기준. 화면이 길어지면 UI가 커짐 | 세로만 쓸 때 |

---

## 2. 앵커로 영역 고정

UI가 **화면 비율이 바뀌어도 같은 위치·비율**을 유지하려면 **RectTransform 앵커**를 맞춰 두는 것이 좋습니다.

| UI 요소 | 앵커 추천 | 비고 |
|---------|-----------|------|
| **상단 재화 바** | Top Stretch (상단 가로 꽉) | Min Y = 1, Max Y = 1, Left/Right 0~1 |
| **중앙 Merge 보드** | Middle Center 또는 Stretch | 좌우·상하 여백만 두고 Stretch |
| **하단 공장 슬롯** | Bottom Stretch (하단 가로 꽉) | Min Y = 0, Max Y = 0, Left/Right 0~1 |
| **하단 메뉴** | Bottom Stretch | 공장 슬롯 바로 위 또는 같은 줄 |

### 앵커 설정 요약

- **상단 고정**: Anchor Preset → 상단 가로 막대 (Top stretch)
- **하단 고정**: Anchor Preset → 하단 가로 막대 (Bottom stretch)
- **중앙 고정**: Anchor Preset → 가운데 (Middle center)
- **전체 채우기**: Anchor Min (0,0), Max (1,1), Left/Right/Top/Bottom으로 여백만 지정

이렇게 하면 해상도가 바뀌어도 **비율이 같은 영역**에 UI가 배치됩니다.

---

## 3. 노치·안전 영역 (선택)

iPhone 노치, 둥근 모서리, 시스템 UI를 피하고 싶다면:

1. 상단 바 또는 전체 화면을 덮는 패널에 **SafeAreaAnchor** 추가
2. 인스펙터에서 **Apply Top / Bottom / Left / Right** 로 적용할 방향 선택
3. 실행 시 `Screen.safeArea`에 맞춰 RectTransform이 자동 조정됩니다.

---

## 4. 체크리스트

- [ ] Canvas에 **UIScreenScaler** (또는 Canvas Scaler 수동 설정) 적용
- [ ] Reference Resolution: **1080 x 1920** (또는 사용할 기준 해상도)
- [ ] Match Width Or Height: **0.5** (세로 UI)
- [ ] 상단/하단/중앙 UI에 **앵커** 적절히 설정 (Top/Bottom Stretch 등)
- [ ] (선택) 노치 대응 시 **SafeAreaAnchor** 사용

이렇게 설정하면 다양한 기기·화면 비율에서도 UI 크기가 **항상 동일한 비율**로 유지됩니다.
