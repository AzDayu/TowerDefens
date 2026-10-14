using System;
using UnityEngine;

public sealed class GameEvents
{
    public event Action<GameState> StateChanged;
    public event Action<int> ResourceChanged;
    public event Action<int> BaseHealthChanged;
    public event Action BaseDestroyed;
    public event Action<int> EnemyDied;
    public event Action<int> EnemyReachedBase;
    public event Action<int> EnemyCountChanged;
    public event Action<int> WaveStarted;
    public event Action<int> WaveCompleted;
    public event Action<int> TowerCountChanged;
    public event Action<AudioClip> SoundRequested;
    internal void PublishState(GameState value)
    {
        StateChanged?.Invoke(value);
    }

    internal void PublishResource(int value)
    {
        ResourceChanged?.Invoke(value);
    }

    internal void PublishHealth(int value)
    {
        BaseHealthChanged?.Invoke(value);
    }

    internal void PublishBaseDestroyed()
    {
        BaseDestroyed?.Invoke();
    }

    internal void PublishEnemyDied(int reward)
    {
        EnemyDied?.Invoke(reward);
    }

    internal void PublishBaseHit(int damage)
    {
        EnemyReachedBase?.Invoke(damage);
    }

    internal void PublishEnemyCount(int count)
    {
        EnemyCountChanged?.Invoke(count);
    }

    internal void PublishWaveStarted(int wave)
    {
        WaveStarted?.Invoke(wave);
    }

    internal void PublishWaveCompleted(int wave)
    {
        WaveCompleted?.Invoke(wave);
    }

    internal void PublishTowerCount(int count)
    {
        TowerCountChanged?.Invoke(count);
    }

    public void RequestSound(AudioClip clip)
    {
        SoundRequested?.Invoke(clip);
    }

}

