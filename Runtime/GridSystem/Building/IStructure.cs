namespace GridSystem.Building {
    public interface IStructure {
        int Width { get; }
        int Height { get; }
        bool CanBePlacedOn<TGridNode>(TGridNode gridNode) where TGridNode : IBuildableGridNode<TGridNode>;
        void AfterPlacing<TGridNode>(TGridNode mainGridNode, Grid<TGridNode> grid) where TGridNode : IBuildableGridNode<TGridNode>;
        void Remove();
    }
}