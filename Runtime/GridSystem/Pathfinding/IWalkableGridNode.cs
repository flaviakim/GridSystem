namespace GridSystem.Pathfinding {
    public interface IWalkableGridNode<TGridNode> : IGridNode<TGridNode> where TGridNode : IWalkableGridNode<TGridNode> {
        bool IsWalkable { get; }
        float MovementCost { get; }
    }
}