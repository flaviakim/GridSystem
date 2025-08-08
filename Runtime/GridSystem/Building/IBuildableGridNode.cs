namespace GridSystem.Building {
    public interface IBuildableGridNode<TGridNode> : IGridNode<TGridNode> where TGridNode : IBuildableGridNode<TGridNode> {
        bool IsBuildable { get; }
        IStructure Structure { get; }
        bool PlaceStructure(IStructure structure, int x, int y);
        bool RemoveStructure();
    }
}