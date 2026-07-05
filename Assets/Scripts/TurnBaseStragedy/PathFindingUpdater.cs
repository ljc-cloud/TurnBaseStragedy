using System;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Path;
using UnityEngine;

namespace TurnBaseStragedy
{
    public class PathFindingUpdater : MonoBehaviour
    {
        private void Start()
        {
            DestructCrate.OnAntDestroyed += OnAntDestroyed;
        }

        private void OnAntDestroyed(object sender, EventArgs e)
        {
            DestructCrate destructCrate = sender as DestructCrate;
            PathFinding.Instance.SetWalkableGridPosition(destructCrate.GridPosition, true);
        }
    }
}