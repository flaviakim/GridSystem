using System;
using UnityEngine;

namespace GridSystem {
    public abstract class BaseGridNode<TGridNode> : IGridNode<TGridNode> where TGridNode : IGridNode<TGridNode> {
        public int X => GridPosition.x;
        public int Y => GridPosition.y;
        public Vector2Int GridPosition { get; }
        public Grid<TGridNode> Grid { get; }

        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;

        public BaseGridNode(Grid<TGridNode> grid, int x, int y) {
            Grid = grid;
            GridPosition = new Vector2Int(x, y);
        }

        void IGridNode<TGridNode>.TriggerGridNodeRemoved() {
            OnGridNodeRemoved?.Invoke(this, new GridNodeChangedEventArgs(X, Y));
        }

        protected void TriggerChanged() {
            OnGridNodeChanged?.Invoke(this, new GridNodeChangedEventArgs(X, Y));
        }
        
        public override bool Equals(object obj) {
            if (obj is TGridNode other) {
                return Equals(other);
            }
            return false;
        }

        protected bool Equals(TGridNode other) {
            return X == other.X && Y == other.Y && Grid == other.Grid;
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y, Grid);
        }


        public override string ToString() {
            return $"GridNode({X}, {Y})";
        }
    }
}
