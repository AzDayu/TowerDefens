using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyManager : MonoBehaviour
{
    private readonly HashSet<GameObject> _enemies = new HashSet<GameObject>();
    private GameEvents _events;

    public int Count => _enemies.Count;

    public void Initialize(GameEvents events)
    {
        Shutdown();
        _events = events;
    }

    public bool Register(GameObject enemy)
    {
        if (enemy == null || !_enemies.Add(enemy))
        {
            return false;
        }

        _events.PublishEnemyCount(Count);
        return true;
    }

    // 등록한 적은 외부 Destroy 대신 Kill 또는 ReachBase로 제거한다.
    public void Kill(GameObject enemy, int reward)
    {
        if (!_enemies.Remove(enemy))
        {
            return;
        }

        Destroy(enemy);
        _events.PublishEnemyDied(Mathf.Max(0, reward));
        _events.PublishEnemyCount(Count);
    }

    public void ReachBase(GameObject enemy, int damage)
    {
        if (!_enemies.Remove(enemy))
        {
            return;
        }

        Destroy(enemy);
        _events.PublishBaseHit(Mathf.Max(0, damage));
        _events.PublishEnemyCount(Count);
    }

    public void Clear()
    {
        foreach (GameObject enemy in _enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        _enemies.Clear();
        _events?.PublishEnemyCount(0);
    }

    public void Shutdown()
    {
        // 정리는 사망이나 웨이브 완료 이벤트를 발생시키지 않는다.
        _events = null;
        Clear();
    }

    private void OnDestroy()
    {
        Shutdown();
    }
}
