using UnityEngine;

namespace GridSystem.GridPosition {
    public abstract class BaseGridPosition<TGridNode> : IGridPosition<TGridNode> where TGridNode : IGridNode<TGridNode> {
        public Vector2 PositionInGrid { get; }
        public Vector3 PositionInWorld => Node.Grid.TransformGridPositionToWorld(PositionInGrid);
        public TGridNode Node { get; }

        public BaseGridPosition(TGridNode node, Vector2 positionInGrid) {
            Node = node;
            PositionInGrid = positionInGrid;
        }
    }
}