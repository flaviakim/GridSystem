namespace GridSystem.Pathfinding {
    public interface IWalkableGridNode<TGridNode, TGridUnit> : IGridNode<TGridNode>
        where TGridNode : IWalkableGridNode<TGridNode, TGridUnit>
        where TGridUnit : IGridUnit<TGridUnit, TGridNode> {
        TGridUnit GridUnit { get; }
        bool IsWalkable(TGridUnit gridUnit);
        float GetMovementCost(TGridUnit gridUnit);
        /// <summary>
        /// Tries to set the grid unit on this node. Returns false if the node is already occupied.
        /// Should always return true if gridUnit is null (removing the unit).
        /// </summary>
        /// <param name="gridUnit"> The grid unit to set on this node. Null to remove the unit.</param>
        /// <returns> True if the new grid unit was set, false otherwise.</returns>
        bool TrySetGridUnit(TGridUnit gridUnit);
    }
}