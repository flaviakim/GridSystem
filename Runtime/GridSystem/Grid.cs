using System;
using UnityEngine;

namespace GridSystem {
    public class Grid<TGridNode> where TGridNode : IGridNode<TGridNode> {
    
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
        /// When the pivot of a cell is (0, 0), the assumed default origin position is (0, 0, 0).
        /// When the pivot of a cell is (0.5, 0.5), the assumed default origin position is (-CellSize/2f, -CellSize/2f, 0).
        /// </summary>
        public Vector3 OriginPosition { get; protected set; }
    
        private readonly TGridNode[,] _gridArray;

        public Grid(int width, int height, Func<Grid<TGridNode>, int, int, TGridNode> createGridNode) : this(width, height, 1f, Vector3.zero, createGridNode) { }

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridNode>, int, int, TGridNode> createGridNode) {
            Width = width;
            Height = height;
            CellSize = cellSize;
            OriginPosition = originPosition;
            _gridArray = new TGridNode[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    _gridArray[x, y] = createGridNode(this, x, y);
                    _gridArray[x, y].OnGridNodeChanged += (_, args) => {
                        OnGridNodeChanged?.Invoke(this, new GridNodeChangedEventArgs(args.X, args.Y));
                    };
                    TriggerGridNodeAdded(x, y);
                }
            }
        }
    
        public Vector3 GetWorldPosition(int x, int y) {
            // Debug.Asset(x >= 0 && x < Width && y >= 0 && y < Height); // It is quite ok to get outside the grid, don't assert but maybe inform in a log
            var worldPosition = OriginPosition + new Vector3(x * CellSize, 0, y * CellSize);
            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                Debug.Log($"GetWorldPosition: ({x}, {y}) called out of bounds. Still returning world position {worldPosition}.");
            }
            return worldPosition;
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

    }
}