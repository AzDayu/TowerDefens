# 타워 받침·회전 포탑 분리 이미지

2026-10-14 · 내장 image_gen 사용 · 투명 배경 PNG 7개 · 각 1254×1254.
기존 디자인을 바탕으로 받침 아래나 포신 아래에 가려졌던 부분을 복원해 제작했습니다. 통합 이미지와 픽셀 단위로 완전히 같지는 않습니다.

## 구성

| 파일 | 역할 |
|---|---|
| tower-base-shared-v1.png | 모든 종류·레벨에 공용으로 사용하는 고정 받침 |
| tower-body-lv1-layer-v1.png | 소켓 1개인 회전 몸체 |
| tower-body-lv2-layer-v1.png | 소켓 2개인 회전 몸체 |
| tower-body-lv3-layer-v1.png | 소켓 3개인 회전 몸체 |
| tower-gun-balanced-layer-v1.png | 밸런스형 포신 |
| tower-gun-rapid-layer-v1.png | 연사형 3연장 포신 |
| tower-gun-long-range-layer-v1.png | 사거리형 긴 포신 |

몸체 3종과 포신 3종을 조합하여 9가지 기초 타워를 표현합니다. 속성·파워업 장착 이미지와 공격 효과는 이번 분리 작업에 포함하지 않았습니다.

## 조립 구조

TowerRoot
- Base: 고정 받침
- RotatingRoot: 조준 방향으로 회전하는 부모
  - Body: 레벨에 맞는 몸체
  - Gun: 종류에 맞는 포신
  - Sockets: 몸체 소켓 좌표에 맞춘 장착 파츠

Body와 Gun의 회전 중심은 RotatingRoot의 원점에 겹치게 둡니다. 파츠도 RotatingRoot 아래에 두어 같이 회전하게 구성합니다.

## 미리보기

preview.html을 브라우저에서 열면 9개 조합을 비교할 수 있습니다. 오른쪽·아래·왼쪽·위 버튼과 자동 회전, 받침 표시 및 중심·슬롯 표시 기능을 넣었습니다.
원본 PNG를 브라우저에서 겹쳐 보여 주는 미리보기이며, 게임 전투나 Unity 프리팹이 아닙니다.

## Unity 적용용 좌표

이미지 여백이 서로 달라 중앙 피벗만 사용하면 어긋납니다. 아래 값을 적용하는 기준은 이미지 전체를 같은 Pixels Per Unit(권장 시작값 512)로 가져오는 것입니다. 실제 장면의 타워 크기는 TowerRoot의 스케일로 한꺼번에 조정합니다.

| 레이어 | Unity Custom Pivot X | Unity Custom Pivot Y | 자식 로컬 스케일 | Order in Layer |
|---|---:|---:|---:|---:|
| Base | 0.490 | 0.510 | 0.88 | 0 |
| Body, 레벨 공통 | 0.628 | 0.500 | 0.72 | 1 |
| Gun, 밸런스형 | 0.330 | 0.485 | 0.70 | 2 |
| Gun, 연사형 | 0.330 | 0.485 | 0.56 | 2 |
| Gun, 사거리형 | 0.270 | 0.500 | 0.68 | 2 |

이 값은 브라우저 조립 미리보기에 맞춰 보정한 값입니다. 현재 Unity 프로젝트에는 PNG와 좌표 JSON을 복사했으며, Unity Import Settings·피벗 설정·프리팹 연결은 아직 수행하지 않았습니다.

layer-layout.json은 이미지의 왼쪽 위를 원점으로 한 정규화 좌표를 기록합니다. Unity 피벗으로 바꿀 때 Y를 1-Y로 변환합니다.
슬롯 좌표는 몸체 이미지에서의 위치입니다. RotatingRoot의 로컬 좌표로 옮기려면 다음 식을 사용합니다.

- X = (slotX - bodyPivotX) × imageWidth / PPU × bodyScale
- Y = (bodyPivotY - slotY) × imageHeight / PPU × bodyScale

## 확인 결과

- PNG 7개 모두 1254×1254이며 투명 여백의 알파값 0을 확인했습니다.
- 소켓 1·2·3개를 시각적으로 확인했습니다. 오른쪽의 큰 원형 구조는 포신 지지대입니다.
- 브라우저에서 9개 조합을 표시했고 이미지 로드 실패가 없었습니다.
- 0도, 90도, 180도 상태를 확인했습니다. 받침은 고정되고 몸체와 포신은 함께 회전합니다.
- PNG를 프로그램으로 재가공하거나 기존 이미지를 덮어쓰지 않았습니다.
- Unity 실행·전투·프리팹 동작은 이번 이미지 작업의 검증 범위에 포함하지 않았습니다.

프로젝트 저장 위치: D:\TowerDefens\Assets\05_Art\Towers\LayeredV1
정확한 생성 프롬프트: generation-prompts.md

