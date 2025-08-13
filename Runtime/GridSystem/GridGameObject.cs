using System;
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

        public int Width => _gridImplementation.Width;

        public int Height => _gridImplementation.Height;

        public float CellSize => _gridImplementation.CellSize;

        public Vector3 OriginPosition => _gridImplementation.OriginPosition;

        public Vector2 Pivot {
            get => _gridImplementation.Pivot;
            set => _gridImplementation.Pivot = value;
        }

        public Vector3 GetWorldPosition(int x, int y) {
            return _gridImplementation.GetWorldPosition(x, y);
        }
        public Vector3 GetWorldPosition(Vector2 positionInGrid) {
            return _gridImplementation.GetWorldPosition(positionInGrid);
        }
        public TGridNode GetGridNodeFromWorldPos(Vector3 worldPosition) {
            return _gridImplementation.GetGridNodeFromWorldPos(worldPosition);
        }
        public void SetGridNode(int x, int y, TGridNode value) {
            _gridImplementation.SetGridNode(x, y, value);
        }
        public TGridNode GetGridNode(int x, int y) {
            return _gridImplementation.GetGridNode(x, y);
        }
    }
}