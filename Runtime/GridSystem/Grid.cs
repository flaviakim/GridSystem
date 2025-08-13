using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public class Grid<TGridNode> : IGrid<TGridNode> where TGridNode : IGridNode<TGridNode> {
    
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeAdded;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;
    
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float CellSize { get; protected set; }
    
        /// <summary>
        /// The origin position of the grid in world space.
        /// It is the position where the grid's (0, 0) cell is located and at (0, 0) within that cell.
        /// The origin position is used to calculate the world position of each cell based on its coordinates.
        /// <para />
        /// When the <see cref="Pivot"/> of a cell is (0, 0), the assumed default origin position is (0, 0, 0).
        /// When the <see cref="Pivot"/> of a cell is (0.5, 0.5), the assumed default origin position is (-CellSize/2f, -CellSize/2f, 0).
        /// </summary>
        public Vector3 OriginPosition { get; protected set; }
        
        /// <summary>
        /// The pivot of the grid cells, i.e., the point within each cell that is considered the "origin" of that cell.
        ///
        /// The pivot is used to calculate the world position of each cell based on its coordinates.
        /// (0, 0) means the bottom-left corner of the cell,
        /// (0.5, 0.5) means the center of the cell,
        /// (1, 1) means the top-right corner of the cell.
        /// </summary>
        public Vector2 Pivot { get; set; }
    
        private readonly TGridNode[,] _gridArray;

        public Grid(int width, int height, Func<Grid<TGridNode>, int, int, TGridNode> createGridNode) : this(width, height, 1f, Vector3.zero, Vector2.zero, createGridNode) { }

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Vector2 pivot,
            Func<Grid<TGridNode>, int, int, TGridNode> createGridNode) {
            Width = width;
            Height = height;
            CellSize = cellSize;
            OriginPosition = originPosition;
            Pivot = pivot;
            _gridArray = new TGridNode[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    _gridArray[x, y] = createGridNode(this, x, y);
                    _gridArray[x, y].OnGridNodeChanged += OnGridNodeChanged;
                    TriggerGridNodeAdded(x, y);
                }
            }
        }

        public void Dispose() {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (_gridArray[x, y] != null) {
                        _gridArray[x, y].OnGridNodeChanged -= OnGridNodeChanged;
                        _gridArray[x, y].TriggerGridNodeRemoved();
                        TriggerGridNodeRemoved(x, y);
                        _gridArray[x, y] = default;
                    }
                }
            }
        }

        public Vector3 GetWorldPosition(int x, int y) {
            // Debug.Asset(x >= 0 && x < Width && y >= 0 && y < Height); // It is quite ok to get outside the grid, don't assert but maybe inform in a log
            var worldPosition = OriginPosition + new Vector3((x + Pivot.x) * CellSize, (y + Pivot.y) * CellSize, 0);
            // Adjusted for pivot as if:
            // worldPosition.x += Pivot.x * CellSize;
            // worldPosition.y += Pivot.y * CellSize;
            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                Debug.Log($"GetWorldPosition: ({x}, {y}) called out of bounds. Still returning world position {worldPosition}.");
            }
            return worldPosition;
        }
        
        /// <summary>
        /// Gets the world position of a cell in the grid based on its position in the grid. The whole number is the cell,
        /// the decimal part is the relative position within the cell, factoring in cell size and pivot.
        /// 
        /// The position in the grid is given as a Vector2, where (0.0, 0.0) is the pivot position of the cell at (0, 0),
        /// (1.0, 1.0) is the pivot position of the cell at (1, 1), and (2.5, 4.5) is half a cell size offset to the right and up from the pivot of the cell at (2, 4).
        /// </summary>
        /// <param name="positionInGrid"> The position in the grid to get the world position for.</param>
        /// <returns> The world position within the cell at the given position in the grid.</returns>
        public Vector3 GetWorldPosition(Vector2 positionInGrid) {
            if (positionInGrid.x < 0 || positionInGrid.x >= Width || positionInGrid.y < 0 || positionInGrid.y >= Height) {
                Debug.LogWarning($"GetWorldPosition: ({positionInGrid.x}, {positionInGrid.y}) is out of bounds.");
            }

            return OriginPosition + new Vector3(
                (positionInGrid.x + Pivot.x) * CellSize,
                (positionInGrid.y + Pivot.y) * CellSize,
                0);
        }

        public TGridNode GetGridNodeFromWorldPos(Vector3 worldPosition) {
            if (!GetXYFromWorldPosition(worldPosition, out var x, out var y)) {
                Debug.LogError($"GetGridNodeFromWorldPos: ({worldPosition.x}, {worldPosition.y}, {worldPosition.z}) is out of bounds. Returning default.");
                return default;
            }
            Debug.Assert(x >= 0 && x < Width && y >= 0 && y < Height);
            return GetGridNode(x, y);
        }

        private bool GetXYFromWorldPosition(Vector3 worldPosition, out int x, out int y) {
            x = Mathf.FloorToInt((worldPosition.x - OriginPosition.x) / CellSize);
            y = Mathf.FloorToInt((worldPosition.z - OriginPosition.z) / CellSize);
        
            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                Debug.LogWarning($"GetXYFromWorldPosition: ({worldPosition.x}, {worldPosition.y}, {worldPosition.z}) is out of bounds. Returning ({x}, {y}).");
                return false;
            }
            return true;
        }


        public void SetGridNode(int x, int y, TGridNode value) {
            Debug.Assert(x >= 0 && x < Width && y >= 0 && y < Height);
            if (_gridArray[x, y] != null) {
                _gridArray[x, y].TriggerGridNodeRemoved();
                TriggerGridNodeRemoved(x, y);
            }
            if (x >= 0 && x < Width && y >= 0 && y < Height) {
                _gridArray[x, y] = value;
                TriggerGridNodeAdded(x, y);
            } else {
                Debug.LogError($"SetGridNode: ({x}, {y}) is out of bounds.");
            }
        }
    
        public TGridNode GetGridNode(int x, int y) {
            Debug.Assert(x >= 0 && x < Width && y >= 0 && y < Height);
            if (x >= 0 && x < Width && y >= 0 && y < Height) {
                return _gridArray[x, y];
            }

            Debug.LogError($"GetGridNode: ({x}, {y}) is out of bounds.");
            return default;
        }

        protected void TriggerGridNodeAdded(int x, int y) {
            Debug.Assert(x >= 0 && x < Width && y >= 0 && y < Height, $"TriggerGridNodeAdded: ({x}, {y}) is out of bounds.");
            OnGridNodeAdded?.Invoke(this, new GridNodeChangedEventArgs(x, y));
        }

        protected void TriggerGridNodeRemoved(int x, int y) {
            Debug.Assert(x >= 0 && x < Width && y >= 0 && y < Height, $"TriggerGridNodeRemoved: ({x}, {y}) is out of bounds.");
            OnGridNodeRemoved?.Invoke(this, new GridNodeChangedEventArgs(x, y));
        }

        public List<TGridNode> GetGridNodesInArea(int x, int y, int width, int height) {
            var nodes = new List<TGridNode>();
            for (int iY = 0; iY < height; iY++) {
                for (int iX = 0; iX < width; iX++) {
                    var node = GetGridNode(x + iX, y + iY);
                    if (node != null) {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }
    }
}