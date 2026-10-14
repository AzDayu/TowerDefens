using System.Collections.Generic;
using UnityEngine;

public sealed class MapManager : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private List<Vector3> _waypoints = new List<Vector3>();
    [SerializeField] private List<Vector2Int> _allowedBuildCells = new List<Vector2Int>();

    private readonly HashSet<Vector2Int> _buildCells = new HashSet<Vector2Int>();

    public IReadOnlyList<Vector3> Path => _waypoints.AsReadOnly();

    public void Initialize()
    {
        _buildCells.Clear();
        _buildCells.UnionWith(_allowedBuildCells);
    }

    public void Configure(IEnumerable<Vector3> waypoints, IEnumerable<Vector2Int> allowedCells)
    {
        // 먼저 복사해 자체 목록을 인자로 전달해도 데이터가 사라지지 않게 한다.
        List<Vector3> newPath = waypoints == null ? new List<Vector3>() : new List<Vector3>(waypoints);
        List<Vector2Int> newCells = allowedCells == null ? new List<Vector2Int>() : new List<Vector2Int>(allowedCells);
        _waypoints = newPath;
        _allowedBuildCells = newCells;
        Initialize();
    }

    public bool CanBuild(Vector2Int cell)
    {
        return _buildCells.Contains(cell);
    }

    public void Shutdown()
    {
        _buildCells.Clear();
    }
}
