using UnityEngine;

public sealed class ResourceManager : MonoBehaviour
{
    private GameEvents _events;

    public int Gold { get; private set; }

    public void Initialize(GameEvents events)
    {
        Shutdown();
        _events = events;
        _events.EnemyDied += AddGold;
    }

    public void ResetGold(int gold)
    {
        Gold = Mathf.Max(0, gold);
        _events.PublishResource(Gold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gold = checked(Gold + amount);
        _events.PublishResource(Gold);
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0 || Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        _events.PublishResource(Gold);
        return true;
    }

    public void Shutdown()
    {
        if (_events == null)
        {
            return;
        }

        _events.EnemyDied -= AddGold;
        _events = null;
    }

    private void OnDestroy()
    {
        Shutdown();
    }
}
