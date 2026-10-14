using System;
using UnityEngine;

public sealed class UIManager : MonoBehaviour
{
    private GameEvents _events;

    public event Action Changed;

    public int Gold { get; private set; }
    public int BaseHealth { get; private set; }
    public int Wave { get; private set; }
    public GameState State { get; private set; }

    public void Initialize(GameEvents events)
    {
        Shutdown();
        _events = events;
        _events.ResourceChanged += OnResourceChanged;
        _events.BaseHealthChanged += OnBaseHealthChanged;
        _events.WaveStarted += OnWaveStarted;
        _events.StateChanged += OnStateChanged;
    }

    private void OnResourceChanged(int gold)
    {
        Gold = gold;
        Changed?.Invoke();
    }

    private void OnBaseHealthChanged(int health)
    {
        BaseHealth = health;
        Changed?.Invoke();
    }

    private void OnWaveStarted(int wave)
    {
        Wave = wave;
        Changed?.Invoke();
    }

    private void OnStateChanged(GameState state)
    {
        State = state;
        if (state == GameState.Initializing)
        {
            Wave = 0;
        }

        Changed?.Invoke();
    }

    public void Shutdown()
    {
        if (_events == null)
        {
            return;
        }

        _events.ResourceChanged -= OnResourceChanged;
        _events.BaseHealthChanged -= OnBaseHealthChanged;
        _events.WaveStarted -= OnWaveStarted;
        _events.StateChanged -= OnStateChanged;
        _events = null;
    }

    private void OnDestroy()
    {
        Shutdown();
        Changed = null;
    }
}
