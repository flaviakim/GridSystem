using System;
using UnityEngine;

namespace GridSystem {
    public interface IGridNode<TGridNode> where TGridNode : IGridNode<TGridNode> {
        int X { get; }
        int Y { get; }
        Vector2Int GridPosition { get; }
        Grid<TGridNode> Grid { get; }
        public event EventHandler<GridNodeChangedEvent> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEvent> OnGridNodeRemoved;
        public void TriggerGridNodeRemoved();
    }
}
