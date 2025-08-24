namespace GridSystem.Pathfinding {
    public interface IWalkableGridNode<TGridNode> : IGridNode<TGridNode> where TGridNode : IWalkableGridNode<TGridNode> {
        bool IsWalkable { get; } // TODO change to isWalkable(Unit) method for specific units
        float MovementCost { get; } // TODO change to getMovementCost(Unit) getting method for specific units
    }
}