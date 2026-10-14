# 코어 시스템 연결 방법

1. 씬의 빈 GameObject에 `GameManager` 컴포넌트를 추가한다. 같은 오브젝트나 자식 오브젝트에 MapManager, ResourceManager, EnemyManager, TowerManager, WaveManager, UIManager, AudioManager 컴포넌트를 추가한다.
2. GameManager Inspector의 시스템 매니저 참조 7개를 연결하고 시작 재화, 기지 체력, 총 웨이브 수를 설정한다. 빠진 참조가 있으면 오류를 기록하고 초기화를 중단한다.
3. GameManager는 Awake에서 Instance를 등록하고 Start에서 각 매니저의 Initialize를 순서대로 호출한다. 기본 실행 순서는 -100이므로 기본 실행 순서(0)의 다른 컴포넌트는 Start에서 접근할 수 있다. 다른 컴포넌트의 Awake에서는 초기화된 시스템에 접근하지 않는다. 매니저에는 독립적인 Awake/Start 초기화 로직을 추가하지 않는다.
4. MapManager Inspector에서 경로와 설치 가능 셀을 설정하거나 맵 로더가 초기화 후 `Map.Configure(waypoints, allowedCells)`를 호출한다. 맵은 재시작 시 유지된다.
5. UI는 `UI.Changed`를 구독하고 Gold, BaseHealth, Wave, State를 읽어 화면에 표시한다. 구독 직후에도 현재 값을 한 번 읽고, 뷰를 해제할 때 구독을 해제한다.
6. AudioManager Inspector에 음악/효과음 AudioSource를 연결한다. 코드에서는 `Audio.Configure(musicSource, effectSource)`도 사용할 수 있다. `Events.RequestSound(clip)`은 효과음을 재생한다. 사망 사운드 종류는 게임 콘텐츠에서 결정해 요청한다.

## 웨이브와 적

스포너는 `Events.WaveStarted`를 구독한다. 시작 버튼에서 `StartNextWave()`를 호출하면 WaveStarted가 발행된다. 스포너가 프리팹을 생성할 때마다 `Enemies.Register(enemy)`를 호출하고, 해당 웨이브의 모든 생성 작업을 마쳤을 때 `FinishWaveSpawning()`을 호출한다. 여러 프레임에 걸친 생성도 지원하며, 마지막 생성 전에는 완료를 알리지 않는다.

적 사망 시 `Enemies.Kill(enemy, reward)`, 기지 도착 시 `Enemies.ReachBase(enemy, damage)`를 호출한다. 두 API는 적을 목록에서 제거하고 GameObject도 파괴한다. 등록된 적을 외부에서 직접 Destroy하지 않는다. 처치 보상은 ResourceManager가 이벤트로 지급한다. 생성이 끝나고 적이 모두 제거되면 다음 웨이브 준비 상태 또는 최종 승리 상태가 된다.

스포너는 StateChanged를 구독해 Combat 외 상태에서 남은 생성 코루틴을 취소해야 한다. 재시작 버튼은 `RestartGame()`을 호출한다.

## 타워

`Towers.TryBuild(prefab, cell, worldPosition, cost)`는 설치 가능 셀, 중복 설치, 재화를 검사하고 타워를 생성한다. `Towers.TrySell(cell, refund)`로 판매한다. 비용과 환급액은 이후 타워 데이터에서 전달한다. 공격, 타깃 탐색, 업그레이드 규칙은 타워 콘텐츠 구현 단계에서 연결한다.

## 수명과 실행 순서

GameManager만 싱글톤이며 모든 매니저는 씬에 배치한 MonoBehaviour다. 기존 동작처럼 씬 수명에 맞추며 DontDestroyOnLoad는 적용하지 않았다. 이벤트 전달용 GameEvents와 기지 데이터 BaseState는 일반 C# 객체다.

초기화 순서: 맵 → 자원 → 기지 데이터 → 적 → 타워 → 웨이브 → UI → 오디오. 모든 이벤트 구독을 마친 뒤 초기화 상태 → 웨이브/적/타워 리셋 → 자원/기지 리셋 → 준비 상태로 진행한다.

GameManager의 OnDestroy에서 역순으로 Shutdown을 호출한다. 컴포넌트가 먼저 파괴될 경우에는 각자의 OnDestroy에서도 구독을 해제하며, Shutdown 중복 호출은 안전하다. IDisposable과 매니저 생성자는 사용하지 않는다. Update 기반 시스템은 아직 없으며, 이후 필요하면 GameManager에서 Tick 호출 순서를 명시한다.

코드 규칙: 클래스/메서드/프로퍼티/이벤트는 PascalCase, private 필드는 _camelCase, 매개변수와 지역 변수는 camelCase. 중괄호는 다음 줄에 배치하고 조건문의 중괄호를 생략하지 않는다. Inspector 필드는 [SerializeField] private으로 선언하고 공개 읽기 전용 프로퍼티를 제공한다.

현재 구현은 코어 기반이다. 실제 맵 로딩, 웨이브 데이터와 생성 간격, 적 이동·공격, 타워 업그레이드, UI 뷰, 오디오 에셋은 별도 콘텐츠가 필요하다.

검증: MonoBehaviour 전환 후 Unity 6000.3.25f1의 CoreModule/AudioModule 참조로 C# 빌드 성공. 외부 빌드는 Inspector 할당을 인식하지 못해 직렬화 참조 필드에 CS0649 경고를 표시한다. 이전 일반 클래스 버전에서 검증한 이벤트 흐름을 유지했으나, 전환 후 Unity 플레이 모드의 초기화·프리팹 생성·화면·오디오 동작은 아직 검증하지 않았다.
