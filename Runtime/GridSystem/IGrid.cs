using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public interface IGrid<TGridNode> : IDisposable where TGridNode : IGridNode<TGridNode> {
        /// <summary>
        /// The total number of cells in the grid.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The width of the grid, in number of cells.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the grid, in number of cells.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The size of each cell in the grid, in world units.
        /// </summary>
        float CellSize { get; }

        /// <summary>
        /// The offset of the grid in world space. This is the world position of the pivot of the cell (0, 0).
        /// </summary>
        Vector3 OriginPosition { get; }

        /// <summary>
        /// The pivot point of each cell in the grid, i.e., the point within each cell that is considered the "origin" of that cell.
        /// Represented as a Vector2 where (0,0) is the bottom-left corner,
        /// (0.5,0.5) is the center, and (1,1) is the top-right corner.
        /// The pivot is used to calculate the world position of each cell based on its coordinates.
        /// </summary>
        Vector2 Pivot { get; }

        /// <summary>
        /// Gets the world position of the pivot of a cell in the grid based on its x and y coordinates in the grid.
        /// </summary>
        /// <param name="x"> The x coordinate of the cell in the grid.</param>
        /// <param name="y"> The y coordinate of the cell in the grid.</param>
        /// <param name="isInBounds"> Outputs whether the given coordinates are within the grid bounds.</param>
        /// <returns> The world position of the pivot of the cell in the grid.</returns>
        Vector3 GetWorldPosition(int x, int y, out bool isInBounds);

        /// <summary>
        /// Gets the world position of the pivot of a cell in the grid based on its position in the grid.
        /// </summary>
        /// <param name="position"> The position of the cell in the grid as a Vector2Int.</param>
        /// <param name="isInBounds"> Outputs whether the given position is within the grid bounds.</param>
        /// <returns> The world position of the pivot of the cell in the grid.</returns>
        Vector3 GetWorldPosition(Vector2Int position, out bool isInBounds) => GetWorldPosition(position.x, position.y, out isInBounds);
        /// <summary>
        /// Gets the world position of the pivot of a cell in the grid based on the grid node.
        /// </summary>
        /// <param name="node"> The grid node to get the world position for.</param>
        /// <returns> The world position of the pivot of the cell in the grid.</returns>
        Vector3 GetWorldPosition(TGridNode node) => GetWorldPosition(node.X, node.Y, out bool _);
        
        /// <summary>
        /// Gets the world position of a cell in the grid based on its position in the grid. The whole number is the cell,
        /// the decimal part is the relative position within the cell, factoring in cell size and pivot.
        /// 
        /// The position in the grid is given as a Vector2, where (0.0, 0.0) is the pivot position of the cell at (0, 0),
        /// (1.0, 1.0) is the pivot position of the cell at (1, 1), and (2.5, 4.5) is half a cell size offset to the right and up from the pivot of the cell at (2, 4).
        /// </summary>
        /// <param name="positionInGrid"> The position in the grid to get the world position for.</param>
        /// <returns> The world position within the cell at the given position in the grid.</returns>
        Vector3 TransformGridPositionToWorld(Vector2 positionInGrid);

        /// <summary>
        /// Converts a world position to grid coordinates, returning the cell's position that contains the world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition);

        /// <summary>
        /// Converts a world position to grid coordinates, returning true if the position is within the grid bounds.
        /// Outputs the x and y coordinates whether the position is within bounds or not.
        /// </summary>
        /// <param name="worldPosition"> The world position to convert </param>
        /// <param name="x"> The x coordinate of the cell in the grid. </param>
        /// <param name="y"> The y coordinate of the cell in the grid. </param>
        /// <returns> True if the world position is within the grid bounds, false otherwise. </returns>
        bool GetXYFromWorldPosition(Vector3 worldPosition, out int x, out int y);

        /// <summary>
        /// Tries to get the grid node at the given world position. Returns true if successful, false if the position is out of bounds.
        /// If false, the out parameter 'node' will be set to default.
        /// If true, 'node' will contain the grid node at the specified world position.
        /// </summary>
        /// <param name="worldPosition"> The world position to get the grid node for. </param>
        /// <param name="node"> The output grid node at the specified world position, or default if out of bounds. </param>
        /// <returns> True if the grid node was successfully retrieved, false if the position is out of bounds. </returns>
        bool TryGetGridNodeFromWorldPos(Vector3 worldPosition, out TGridNode node) {
            if (GetXYFromWorldPosition(worldPosition, out int x, out int y)) {
                node = GetGridNode(x, y);
                return true;
            }

            node = default;
            return false;
        }

        /// <summary>
        /// Gets all grid nodes within a rectangular area defined by the bottom-left corner (x, y) and the specified width and height.
        /// The area includes all nodes from (x, y) to (x + width - 1, y + height - 1).
        /// If the specified area extends beyond the grid bounds, only the nodes within the grid will be returned.
        /// </summary>
        /// <param name="x"> The x coordinate of the bottom-left corner of the area in the grid. </param>
        /// <param name="y"> The y coordinate of the bottom-left corner of the area in the grid. </param>
        /// <param name="width"> The width of the area in number of cells. </param>
        /// <param name="height"> The height of the area in number of cells. </param>
        /// <returns> A list of grid nodes within the specified area. </returns>
        public List<TGridNode> GetGridNodesInArea(int x, int y, int width, int height);
        /// <summary>
        /// Sets the grid node at the specified (x, y) coordinates in the grid to the given value.
        /// If there is already a node at that position, it will be replaced.
        /// </summary>
        /// <param name="x"> The x coordinate of the cell in the grid. </param>
        /// <param name="y"> The y coordinate of the cell in the grid. </param>
        /// <param name="value"> The grid node to set at the specified position. </param>
        void SetGridNode(int x, int y, TGridNode value);
        /// <summary>
        /// Gets the grid node at the specified (x, y) coordinates in the grid.
        /// If the coordinates are out of bounds, this method may throw an exception or return default.
        /// It is recommended to check bounds before calling this method.
        /// </summary>
        /// <param name="x"> The x coordinate of the cell in the grid. </param>
        /// <param name="y"> The y coordinate of the cell in the grid. </param>
        /// <returns> The grid node at the specified position. </returns>
        TGridNode GetGridNode(int x, int y);
        /// <summary>
        /// Indexer to access grid nodes using grid coordinates.
        /// </summary>
        /// <param name="x"> The x coordinate of the cell in the grid. </param>
        /// <param name="y"> The y coordinate of the cell in the grid. </param>
        /// <returns> The grid node at the specified position. </returns>
        TGridNode this[int x, int y] => GetGridNode(x, y);
        
        int GetManhattanDistance(TGridNode from, TGridNode to);
        List<TGridNode> GetGridNodes(IEnumerable<Vector2Int> positions);
        
        // bool Contains(TGridNode node);
        //
        // IEnumerable<TGridNode> GetNeighbours(TGridNode node);
        
        /// <summary>
        /// Event triggered when a grid node is added to the grid.
        /// </summary>
        event EventHandler<GridNodeAddedEvent> OnGridNodeAdded;
        /// <summary>
        /// Event triggered when a grid node is removed from the grid.
        /// </summary>
        event EventHandler<GridNodeRemovedEvent> OnGridNodeRemoved;
        /// <summary>
        /// Event triggered when a grid node is changed in the grid.
        /// </summary>
        event EventHandler<GridNodeChangedEvent> OnGridNodeChanged;
    }
}
