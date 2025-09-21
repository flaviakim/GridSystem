using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public interface IGrid<TGridNode> : IDisposable where TGridNode : IGridNode<TGridNode> {
        int Width { get; }
        int Height { get; }
        float CellSize { get; }
        Vector3 OriginPosition { get; }
        Vector2 Pivot { get; set; }
        
        Vector3 GetWorldPosition(int x, int y);
        Vector3 GetWorldPosition(Vector2Int position);
        Vector3 GetWorldPosition(TGridNode node);
        
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

        Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition);
        bool GetXYFromWorldPosition(Vector3 worldPosition, out int x, out int y);
        TGridNode GetGridNodeFromWorldPos(Vector3 worldPosition);
        public List<TGridNode> GetGridNodesInArea(int x, int y, int width, int height);
        void SetGridNode(int x, int y, TGridNode value);
        TGridNode GetGridNode(int x, int y);
    }
}
