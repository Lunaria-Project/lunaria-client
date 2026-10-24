# FullscreenUIFitter 적용 방법

전체 화면 UI를 고정된 디자인 좌표계로 유지하면서 부모 RectTransform 영역에 맞춰 균일하게 확대·축소합니다. 특정 화면, Lunaria UI 타입, 게임 상태에 의존하지 않습니다.

## 권장 구조

Canvas (Canvas Scaler)
- ScreenRoot: 부모 전체에 Stretch, Offset 0
  - Background: 필요하면 부모 전체에 Stretch
  - Content: FullscreenUIFitter, 기존 UI 전체를 자식으로 배치

1. 기존 Canvas Scaler는 유지합니다. 신규 Canvas라면 Scale With Screen Size / Reference Resolution 1920 x 1080을 기준으로 시작합니다.
2. 부모(ScreenRoot)는 화면 또는 원하는 표시 영역 전체를 차지하게 설정합니다.
3. 고정 비율로 표시할 Content에 Add Component > Lunaria > UI > Fullscreen UI Fitter를 추가합니다. 기존 ViewParent가 같은 역할이면 그 오브젝트에 붙입니다. Canvas 자체에는 붙이지 않습니다.
4. Reference Resolution에는 해당 프리팹의 실제 디자인 크기를 입력합니다. 기본값은 1920 x 1080입니다.
5. Scale Mode는 FitInside를 권장합니다. Alignment (0.5, 0.5)는 중앙 정렬입니다.
6. Game View에서 16:9, 4:3, 21:9, 세로 화면을 확인합니다.

## 모드와 한계

- FitInside: 가로·세로 중 작은 배율을 적용해 전체가 보입니다. 화면 종횡비가 다르면 여백이 생깁니다.
- FillParent: 큰 배율을 적용해 영역을 덮습니다. 일부 콘텐츠가 부모 영역 밖으로 나갑니다. 실제 잘라내기가 필요하면 부모에 RectMask2D를 별도로 설정합니다. 중요한 버튼이 있는 콘텐츠에는 주의하세요.
- 계산에는 Screen.width 대신 부모의 UI 좌표 크기를 사용하므로 Canvas Scaler, 중첩 패널과 함께 쓸 수 있습니다.
- 같은 RectTransform의 크기·위치·Scale을 LayoutGroup, AspectRatioFitter, ContentSizeFitter, 애니메이션 등에서 동시에 제어하지 마세요. 필요한 경우 독립적인 Content 래퍼를 둡니다.
- 실행 중 및 에디터의 씬/Prefab Mode에서 갱신합니다. 기준 크기·앵커·위치·Scale은 이 컴포넌트가 제어합니다. 비활성화하면 제어권을 해제하되 마지막 계산값은 유지합니다.
- 이미지 테두리/PPU, Safe Area, Canvas 설정 및 자식 레이아웃은 변경하지 않습니다. 노치 대응이 필요하면 부모 표시 영역을 별도로 구성합니다.
- 종횡비가 크게 달라도 화면을 빈틈없이 쓰는 재배치는 별도 앵커/레이아웃 설계가 필요합니다.

## 검증

Unity 6000.2.7f2 별도 임시 프로젝트에서 실제 컴파일 및 RectTransform 검증을 통과했습니다.
1920x1080, 1280x720, 2560x1440, 1024x768, 2560x1080, 1080x1920의 Fit/Fill 배율, 비활성화/재활성화, 부모 변경, 0 크기 부모를 확인했습니다. 실제 각 화면의 시각 검증은 적용 후 필요합니다.

## 여백 색상

Fill Empty Area를 켜면 FitInside에서 생긴 상하/좌우 여백만 채웁니다. Empty Area Color 기본값은 불투명 검정(#000000)입니다. artifact_craft_view에는 이 옵션을 켜두었습니다.
부모 영역 전체에 배경을 깔지 않고 네 개의 여백 사각형만 생성하므로 콘텐츠 안쪽의 투명 영역은 바뀌지 않습니다. 생성한 Lunaria.Image는 클릭을 가로채지 않으며, 컴포넌트 비활성화/제거 시 함께 정리됩니다. 임시 오브젝트는 프리팹/씬에 저장하지 않습니다.
