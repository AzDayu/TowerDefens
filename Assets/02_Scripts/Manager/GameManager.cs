using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
    Initializing,
    Ready,
    Combat,
    WaveCompleted,
    Victory,
    Defeat
}

[DefaultExecutionOrder(-100)]
public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    [FormerlySerializedAs("startingGold")]
    [SerializeField, Min(0)] private int _startingGold = 100;
    [FormerlySerializedAs("startingHealth")]
    [SerializeField, Min(1)] private int _startingHealth = 20;
    [FormerlySerializedAs("totalWaves")]
    [SerializeField, Min(1)] private int _totalWaves = 5;

    [Header("현재 게임 상태")]
    [SerializeField] private GameState _currentState = GameState.Initializing;

    [Header("시스템 매니저 참조")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private ResourceManager _resourceManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private TowerManager _towerManager;
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;

    private BaseState _baseState;
    private bool _isInitialized;

    public GameEvents Events { get; private set; }
    public MapManager Map => _mapManager;
    public ResourceManager Resources => _resourceManager;
    public EnemyManager Enemies => _enemyManager;
    public TowerManager Towers => _towerManager;
    public WaveManager Waves => _waveManager;
    public UIManager UI => _uiManager;
    public AudioManager Audio => _audioManager;
    public GameState CurrentState => _currentState;
    public GameState State => _currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (Instance != this || !ValidateManagers())
        {
            return;
        }

        InitializeManagers();
        RestartGame();
    }

    private bool ValidateManagers()
    {
        if (_mapManager != null && _resourceManager != null &&
            _enemyManager != null && _towerManager != null &&
            _waveManager != null && _uiManager != null && _audioManager != null)
        {
            return true;
        }

        Debug.LogError("[GameManager] Inspector에서 모든 시스템 매니저 참조를 연결해주세요.", this);
        return false;
    }

    private void InitializeManagers()
    {
        Events = new GameEvents();
        _baseState = new BaseState();

        _mapManager.Initialize();
        _resourceManager.Initialize(Events);
        _baseState.Initialize(Events);
        _enemyManager.Initialize(Events);
        _towerManager.Initialize(Events, _resourceManager, _mapManager);
        _waveManager.Initialize(Events, _enemyManager);
        _uiManager.Initialize(Events);
        _audioManager.Initialize(Events);

        Events.WaveCompleted += OnWaveCompleted;
        Events.BaseDestroyed += OnBaseDestroyed;
        _isInitialized = true;
    }

    public void RestartGame()
    {
        if (!_isInitialized)
        {
            return;
        }

        ChangeState(GameState.Initializing);
        _waveManager.ResetWave();
        _enemyManager.Clear();
        _towerManager.Clear();
        _resourceManager.ResetGold(_startingGold);
        _baseState.ResetHealth(_startingHealth);
        ChangeState(GameState.Ready);
    }

    public void StartNextWave()
    {
        if (!_isInitialized ||
            (_currentState != GameState.Ready && _currentState != GameState.WaveCompleted))
        {
            return;
        }

        ChangeState(GameState.Combat);
        _waveManager.Begin();
    }

    public void FinishWaveSpawning()
    {
        if (_isInitialized && _currentState == GameState.Combat)
        {
            _waveManager.FinishSpawning();
        }
    }

    private void ChangeState(GameState newState)
    {
        OnStateExit(_currentState);
        _currentState = newState;
        OnStateEnter(newState);
        Events.PublishState(newState);
    }

    private void OnStateExit(GameState state)
    {
        // 상태별 종료 동작이 필요해지면 이곳에서 실행한다.
    }

    private void OnStateEnter(GameState state)
    {
        // 상태별 시작 동작이 필요해지면 이곳에서 실행한다.
    }

    private void OnWaveCompleted(int wave)
    {
        if (_currentState != GameState.Combat)
        {
            return;
        }

        ChangeState(wave >= _totalWaves ? GameState.Victory : GameState.WaveCompleted);
    }

    private void OnBaseDestroyed()
    {
        ChangeState(GameState.Defeat);
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        if (_isInitialized)
        {
            Events.WaveCompleted -= OnWaveCompleted;
            Events.BaseDestroyed -= OnBaseDestroyed;

            // 초기화의 역순으로 정리한다. 먼저 이벤트 소비자를 해제한다.
            if (_audioManager != null)
            {
                _audioManager.Shutdown();
            }
            if (_uiManager != null)
            {
                _uiManager.Shutdown();
            }
            if (_waveManager != null)
            {
                _waveManager.Shutdown();
            }
            if (_towerManager != null)
            {
                _towerManager.Shutdown();
            }
            if (_enemyManager != null)
            {
                _enemyManager.Shutdown();
            }
            _baseState.Shutdown();
            if (_resourceManager != null)
            {
                _resourceManager.Shutdown();
            }
            if (_mapManager != null)
            {
                _mapManager.Shutdown();
            }
            _isInitialized = false;
        }

        Instance = null;
    }
}

