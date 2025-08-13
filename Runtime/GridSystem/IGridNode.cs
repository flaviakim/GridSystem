using System;

namespace GridSystem {
    public interface IGridNode<TGridNode> where TGridNode : IGridNode<TGridNode> {
        int X { get; }
        int Y { get; }
        Grid<TGridNode> Grid { get; }
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;
        internal void TriggerGridNodeRemoved();
    }
}