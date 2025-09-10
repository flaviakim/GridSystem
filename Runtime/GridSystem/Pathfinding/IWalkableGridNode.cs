namespace GridSystem.Pathfinding {
    public interface IWalkableGridNode<TGridNode, TGridUnit> : IGridNode<TGridNode> 
        where TGridNode : IWalkableGridNode<TGridNode, TGridUnit>
        where TGridUnit : IGridUnit
    {
        TGridUnit GridUnit { get; }
        bool IsWalkable(TGridUnit gridUnit);
        float GetMovementCost(TGridUnit gridUnit);
        bool TrySetGridUnit(TGridUnit gridUnit);
    }

    public interface IGridUnit {
        
    }
}