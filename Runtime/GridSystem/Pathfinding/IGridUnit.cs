namespace GridSystem.Pathfinding {
    public interface IGridUnit<TGridUnit, TGridNode>
        where TGridUnit : IGridUnit<TGridUnit, TGridNode>
        where TGridNode : IWalkableGridNode<TGridNode, TGridUnit> {
        TGridNode CurrentNode { get; }
        IGrid<TGridNode> Grid { get; }
        bool CanSetNode(TGridNode targetNode);
        bool TrySetNode(TGridNode targetNode);
    }
}