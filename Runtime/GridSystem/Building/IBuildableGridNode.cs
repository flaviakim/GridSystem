namespace GridSystem.Building {
    public interface IBuildableGridNode<TGridNode> : IGridNode<TGridNode> where TGridNode : IBuildableGridNode<TGridNode> {
        /// <summary>
        /// Indicates whether a structure can be placed on this node.
        /// This should not be influenced by the presence of a structure, but rather by the node's properties.
        /// </summary>
        bool IsBuildable { get; }
        /// <summary>
        /// The structure that is currently placed on this node, or null if no structure is placed.
        /// </summary>
        IStructure Structure { get; }
        /// <summary>
        /// Attempts to place a structure on this node.
        /// After a successful call to this method, the node's <see cref="Structure"/> property will be set to the structure that was placed.
        /// </summary>
        /// <param name="structure"> The structure to place on the node.</param>
        /// <returns> True if the structure was successfully placed, false otherwise.</returns>
        bool TryPlaceStructure(IStructure structure);
        /// <summary>
        /// Called after a structure has been successfully placed on the node.
        ///
        /// This method is called after the structure's AfterPlacing method has been called.
        /// It allows the node to perform any additional setup or state changes that are necessary, and it must call
        /// <see cref="IGridNode{TGridNode}.OnGridNodeChanged"/>.
        /// </summary>
        /// <param name="node"> The node that the structure was placed on.</param>
        /// <param name="grid"> The grid that the node belongs to.</param>
        void AfterPlacingStructure(TGridNode node, Grid<TGridNode> grid);
        /// <summary>
        /// Removes the structure from this node.
        /// This method should be called when the structure is removed from the grid.
        /// It will set the <see cref="Structure"/> property to null and trigger the <see cref="IGridNode{TGridNode}.OnGridNodeChanged"/> event if <paramref name="shouldTriggerGridNodeChanged"/> is true.
        /// </summary>
        /// <param name="shouldTriggerGridNodeChanged"> If true, the <see cref="IGridNode{TGridNode}.OnGridNodeChanged"/> event will be triggered after the structure is removed. Set to false when failing placement</param>
        /// <returns> True if the structure was successfully removed, false otherwise.</returns>
        bool RemoveStructure(bool shouldTriggerGridNodeChanged = true);
    }
}