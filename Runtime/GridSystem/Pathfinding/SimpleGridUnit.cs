namespace GridSystem.Pathfinding {
    // TODO move to Samples
    public class SimpleGridUnit<TGridUnit, TGridNode> : IGridUnit<TGridUnit, TGridNode> where TGridNode : IWalkableGridNode<TGridNode, TGridUnit> where TGridUnit : class, IGridUnit<TGridUnit, TGridNode> {
        public SimpleGridUnit(IGrid<TGridNode> grid, TGridNode currentNode) {
            Grid = grid;
            CurrentNode = currentNode;
            if (currentNode != null) {
                currentNode.TrySetGridUnit(this as TGridUnit);
            }
        }
        public TGridNode CurrentNode { get; }
        public IGrid<TGridNode> Grid { get; }
        public bool CanSetNode(TGridNode targetNode) {
            if (targetNode == null) {
                return false;
            }
            if (!targetNode.IsWalkable(this as TGridUnit)) {
                return false;
            }
            return true;
        }
        
        public bool TrySetNode(TGridNode targetNode) {
            if (!CanSetNode(targetNode)) {
                return false;
            }
            if (CurrentNode != null) {
                CurrentNode.TrySetGridUnit(null);
            }
            return targetNode.TrySetGridUnit(this as TGridUnit);
        }
    }
}