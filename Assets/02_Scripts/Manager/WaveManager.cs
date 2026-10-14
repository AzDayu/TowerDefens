using UnityEngine;

public sealed class WaveManager : MonoBehaviour
{
    private GameEvents _events;
    private EnemyManager _enemyManager;
    private bool _isActive;
    private bool _isSpawningFinished;

    public int CurrentWave { get; private set; }

    public void Initialize(GameEvents events, EnemyManager enemyManager)
    {
        Shutdown();
        _events = events;
        _enemyManager = enemyManager;
        _events.EnemyCountChanged += OnEnemyCountChanged;
        _events.StateChanged += OnStateChanged;
        ResetWave();
    }

    internal void Begin()
    {
        if (_isActive)
        {
            return;
        }

        _isActive = true;
        _isSpawningFinished = false;
        CurrentWave++;
        _events.PublishWaveStarted(CurrentWave);
    }

    internal void FinishSpawning()
    {
        if (!_isActive)
        {
            return;
        }

        _isSpawningFinished = true;
        OnEnemyCountChanged(_enemyManager.Count);
    }

    internal void ResetWave()
    {
        _isActive = false;
        _isSpawningFinished = false;
        CurrentWave = 0;
    }

    private void OnEnemyCountChanged(int count)
    {
        if (!_isActive || !_isSpawningFinished || count != 0)
        {
            return;
        }

        _isActive = false;
        _events.PublishWaveCompleted(CurrentWave);
    }

    private void OnStateChanged(GameState state)
    {
        if (state != GameState.Combat)
        {
            _isActive = false;
        }
    }

    public void Shutdown()
    {
        if (_events != null)
        {
            _events.EnemyCountChanged -= OnEnemyCountChanged;
            _events.StateChanged -= OnStateChanged;
        }

        _events = null;
        _enemyManager = null;
        ResetWave();
    }

    private void OnDestroy()
    {
        Shutdown();
    }
}
