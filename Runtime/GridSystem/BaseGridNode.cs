using System;

namespace GridSystem {
    public abstract class BaseGridNode : IGridNode<BaseGridNode> {
        public int X { get; }
        public int Y { get; }
        public Grid<BaseGridNode> Grid { get; }

        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;

        public BaseGridNode(Grid<BaseGridNode> grid, int x, int y) {
            Grid = grid;
            X = x;
            Y = y;
        }

        void IGridNode<BaseGridNode>.TriggerGridNodeRemoved() {
            OnGridNodeRemoved?.Invoke(this, new GridNodeChangedEventArgs(X, Y));
        }

        protected void TriggerChanged() {
            OnGridNodeChanged?.Invoke(this, new GridNodeChangedEventArgs(X, Y));
        }
        
        public override bool Equals(object obj) {
            if (obj is BaseGridNode other) {
                return Equals(other);
            }
            return false;
        }

        protected bool Equals(BaseGridNode other) {
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