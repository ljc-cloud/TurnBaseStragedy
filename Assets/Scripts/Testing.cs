using TurnBaseStragedy.Units;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Unit unit;

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         GridPosition targetGridPosition =
    //             LevelGrid.Instance.GetGridPosition(MouseWorld.GetMouseWorldPosition());
    //         GridPosition startGridPosition = new GridPosition(0, 0);
    //
    //         List<GridPosition> path = PathFinding.Instance.FindPath(startGridPosition, targetGridPosition);
    //
    //         Debug.Log($"[Testing] {path.Count} Path: {string.Join(", ", path.Select(x => x.ToString()))}");
    //
    //         Vector3[] pathPositions = path.Select(x => LevelGrid.Instance.GetWorldPosition(x)).ToArray();
    //         
    //         lineRenderer.positionCount = pathPositions.Length;
    //         lineRenderer.SetPositions(pathPositions);
    //
    //         for (int i = 0; i < path.Count - 1; i++)
    //         {
    //             Debug.DrawLine(
    //                 LevelGrid.Instance.GetWorldPosition(path[i]), 
    //                 LevelGrid.Instance.GetWorldPosition(path[i + 1]), 
    //                 Color.green
    //                 );
    //         }
    //     }
    // }
}