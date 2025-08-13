using UnityEngine;

namespace GridSystem {
    public abstract class GridNodeVisualization<TGridNode> : MonoBehaviour where TGridNode : IGridNode<TGridNode> {
        /// <summary>
        /// Initializes the visualizations for all nodes in the given grid.
        /// This method will instantiate a new GameObject for each node in the grid.
        /// If an optional prefab is provided, it will be used to clone.
        /// Otherwise, a new empty GameObject will be created for each node and the <see cref="Initialize"/> method has to handle setting up the GameObject.
        /// The GameObjects will be parented to the specified parent transform.
        /// </summary>
        /// <param name="grid"> The grid containing the nodes to visualize.</param>
        /// <param name="prefab"> The prefab to use for visualizing the nodes. If null, a new empty GameObject will be created for each node.</param>
        /// <param name="parent"> The parent transform to which the visualizations will be added. Can be null.</param>
        /// <typeparam name="TGridNodeVisualisation"> The type of the visualization component to create for each node. Must inherit from <see cref="GridNodeVisualization{TGridNode}"/>.</typeparam>
        /// <exception cref="System.ArgumentNullException"> Thrown if the grid is null.</exception>
        public static void InitializeVisualisations<TGridNodeVisualisation>(Grid<TGridNode> grid, TGridNodeVisualisation prefab, Transform parent) where TGridNodeVisualisation : GridNodeVisualization<TGridNode> {
            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    TGridNode node = grid.GetGridNode(x, y);
                    Debug.Assert(node != null);
                    TGridNodeVisualisation visualisation = prefab == null
                        ? new GameObject().AddComponent<TGridNodeVisualisation>()
                        : Instantiate(prefab, parent);
                    visualisation.Initialize(node);
                }
            }
        }

        public TGridNode GridNode { get; private set; }

        public void Initialize(TGridNode gridNode) {
            Debug.Assert(gridNode != null, "GridNode should not be null when initializing the visualization.");
            GridNode = gridNode;
            GridNode.OnGridNodeChanged += OnGridNodeChanged;
            GridNode.OnGridNodeRemoved += OnGridNodeRemoved;
            name = $"{GetType().Name} ({GridNode.X}, {GridNode.Y})";
            SetupVisualization();
        }

        /// <summary>
        /// Sets up the visualization for the grid node.
        /// This method is called after the grid node has been initialized.
        /// It should be overridden in derived classes to set up the visual representation of the grid node.
        /// For example, it can set the position, scale, and any visual components like sprites or meshes.
        /// </summary>
        protected abstract void SetupVisualization();

        void OnGridNodeRemoved(object sender, GridNodeChangedEventArgs e) {
            if (GridNode != null) {
                GridNode.OnGridNodeChanged -= OnGridNodeChanged;
                GridNode.OnGridNodeRemoved -= OnGridNodeRemoved;
                GridNode = default;
            }
            AfterGridNodeRemoved(sender, e);
            Destroy(gameObject);
        }

        /// <summary>
        /// Called after the grid node has been removed.
        /// This method is called when the grid node is removed from the grid, or when the whole grid is destroyed.
        /// This method should be overridden in derived classes to perform any cleanup or finalization tasks related to the visualization.
        /// </summary>
        /// <param name="sender"> The sender of the event, typically the grid node itself.</param>
        /// <param name="e"> The event arguments containing information about the removal, such as the coordinates of the grid node.</param>
        protected abstract void AfterGridNodeRemoved(object sender, GridNodeChangedEventArgs e);

        /// <summary>
        /// Called when the grid node has changed.
        /// This method is called whenever the grid node's state changes, such as when a structure is placed or removed,
        /// or when any other property of the grid node changes that should be reflected in the visualization.
        /// It should be overridden in derived classes to update the visual representation of the grid node accordingly.
        /// For example, it can change the sprite, color, or any other visual properties based on the grid node's state.
        /// </summary>
        /// <param name="sender"> The sender of the event, typically the grid node itself.</param>
        /// <param name="e"> The event arguments containing information about the change, such as the coordinates of the grid node.</param>
        protected abstract void OnGridNodeChanged(object sender, GridNodeChangedEventArgs e);
    }
}