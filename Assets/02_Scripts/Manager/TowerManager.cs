using System.Collections.Generic;
using UnityEngine;

public sealed class TowerManager : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, GameObject> _towers = new Dictionary<Vector2Int, GameObject>();
    private GameEvents _events;
    private ResourceManager _resourceManager;
    private MapManager _mapManager;

    public int Count => _towers.Count;

    public void Initialize(GameEvents events, ResourceManager resourceManager, MapManager mapManager)
    {
        Shutdown();
        _events = events;
        _resourceManager = resourceManager;
        _mapManager = mapManager;
    }

    public bool TryBuild(GameObject prefab, Vector2Int cell, Vector3 position, int cost)
    {
        if (prefab == null || !_mapManager.CanBuild(cell) ||
            _towers.ContainsKey(cell) || cost < 0 || _resourceManager.Gold < cost)
        {
            return false;
        }

        GameObject tower = Instantiate(prefab, position, Quaternion.identity);
        _towers.Add(cell, tower);
        _resourceManager.TrySpend(cost);
        _events.PublishTowerCount(Count);
        return true;
    }

    public bool TrySell(Vector2Int cell, int refund)
    {
        if (refund < 0 || !_towers.TryGetValue(cell, out GameObject tower))
        {
            return false;
        }

        _towers.Remove(cell);
        Destroy(tower);
        _resourceManager.AddGold(refund);
        _events.PublishTowerCount(Count);
        return true;
    }

    public void Clear()
    {
        foreach (GameObject tower in _towers.Values)
        {
            if (tower != null)
            {
                Destroy(tower);
            }
        }

        _towers.Clear();
        _events?.PublishTowerCount(0);
    }

    public void Shutdown()
    {
        _events = null;
        Clear();
        _resourceManager = null;
        _mapManager = null;
    }

    private void OnDestroy()
    {
        Shutdown();
    }
}
