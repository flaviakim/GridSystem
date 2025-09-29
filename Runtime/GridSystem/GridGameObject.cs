using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public class GridGameObject<TGridNode> : MonoBehaviour, IGrid<TGridNode> where TGridNode : IGridNode<TGridNode> {
        private Grid<TGridNode> _gridImplementation;

        public void Initialize(int width, int height, Func<Grid<TGridNode>, int, int, TGridNode> createGridNode) {
            _gridImplementation = new Grid<TGridNode>(width, height, createGridNode);
        }

        private void OnDestroy() {
            Dispose();
        }

        public void Dispose() {
            _gridImplementation?.Dispose();
            _gridImplementation = null;
        }

        public int Count => _gridImplementation.Count;

        public int Width => _gridImplementation.Width;

        public int Height => _gridImplementation.Height;

        public float CellSize => _gridImplementation.CellSize;

        public Vector3 OriginPosition => _gridImplementation.OriginPosition;

        public Vector2 Pivot {
            get => _gridImplementation.Pivot;
            set => _gridImplementation.Pivot = value;
        }

        public Vector3 GetWorldPosition(int x, int y, out bool isInBounds) {
            return _gridImplementation.GetWorldPosition(x, y, out isInBounds);
        }
        public Vector3 TransformGridPositionToWorld(Vector2 positionInGrid) {
            return _gridImplementation.TransformGridPositionToWorld(positionInGrid);
        }

        public Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition) {
            return _gridImplementation.GetGridPositionFromWorldPosition(worldPosition);
        }

        public bool GetXYFromWorldPosition(Vector3 worldPosition, out int x, out int y) {
            return _gridImplementation.GetXYFromWorldPosition(worldPosition, out x, out y);
        }

        public TGridNode GetGridNodeFromWorldPos(Vector3 worldPosition) {
            return _gridImplementation.GetGridNodeFromWorldPos(worldPosition);
        }

        public List<TGridNode> GetGridNodesInArea(int x, int y, int width, int height) {
            return _gridImplementation.GetGridNodesInArea(x, y, width, height);
        }

        public void SetGridNode(int x, int y, TGridNode value) {
            _gridImplementation.SetGridNode(x, y, value);
        }
        public TGridNode GetGridNode(int x, int y) {
            return _gridImplementation.GetGridNode(x, y);
        }

        public int GetManhattanDistance(TGridNode from, TGridNode to) {
            return _gridImplementation.GetManhattanDistance(from, to);
        }

        public List<TGridNode> GetGridNodes(IEnumerable<Vector2Int> positions) {
            return _gridImplementation.GetGridNodes(positions);
        }

        public event EventHandler<GridNodeChangedEvent> OnGridNodeChanged {
            add => _gridImplementation.OnGridNodeChanged += value;
            remove => _gridImplementation.OnGridNodeChanged -= value;
        }

        public event EventHandler<GridNodeAddedEvent> OnGridNodeAdded {
            add => _gridImplementation.OnGridNodeAdded += value;
            remove => _gridImplementation.OnGridNodeAdded -= value;
        }

        public event EventHandler<GridNodeRemovedEvent> OnGridNodeRemoved {
            add => _gridImplementation.OnGridNodeRemoved += value;
            remove => _gridImplementation.OnGridNodeRemoved -= value;
        }
    }
}
