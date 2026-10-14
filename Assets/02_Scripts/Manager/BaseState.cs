using System;

// Unity 컴포넌트 기능이 필요 없는 기지 데이터 객체.
public sealed class BaseState
{
    private GameEvents _events;

    public int Health { get; private set; }

    public void Initialize(GameEvents events)
    {
        Shutdown();
        _events = events;
        _events.EnemyReachedBase += OnEnemyReachedBase;
    }

    public void ResetHealth(int health)
    {
        Health = Math.Max(1, health);
        _events.PublishHealth(Health);
    }

    private void OnEnemyReachedBase(int damage)
    {
        if (Health == 0 || damage <= 0)
        {
            return;
        }

        Health = Math.Max(0, Health - damage);
        _events.PublishHealth(Health);

        if (Health == 0)
        {
            _events.PublishBaseDestroyed();
        }
    }

    public void Shutdown()
    {
        if (_events == null)
        {
            return;
        }

        _events.EnemyReachedBase -= OnEnemyReachedBase;
        _events = null;
    }
}
