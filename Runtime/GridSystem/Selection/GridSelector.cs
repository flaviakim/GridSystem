using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace GridSystem.Selection {
    public class GridSelector<TGridNode> where TGridNode : IGridNode<TGridNode> {
        private readonly IGrid<TGridNode> _grid;
        private readonly IGridSelectorDisplay<TGridNode> _gridSelectorDisplay;
        
        public bool ShouldSingleBuildModeStayWhileDragging { get; set; }

        public SelectionShape DefaultSelectionShape { get; set; }

        public bool AllowSelection { get; set; } = true;


        public bool IsDragging => _isDragging;

        private bool _isDragging;
        private Vector2Int _currentDragStartPosition;
        private Vector2Int _currentDragCurrentPosition;
        private SelectionShape _currentSelectionShape;
        private readonly List<Vector2Int> _currentDragArea = new();

        private bool _isSelection;
        private readonly List<Vector2Int> _currentSelection = new();

        public GridSelector(IGrid<TGridNode> grid, IGridSelectorDisplay<TGridNode> gridSelectorDisplay = null,
            SelectionShape defaultSelectionShape = SelectionShape.Area) {
            _grid = grid;
            _gridSelectorDisplay = gridSelectorDisplay;
            DefaultSelectionShape = defaultSelectionShape;
        }

        public void StartSelectionDrag(Vector3 startPositionWorld, SelectionShape? selectionShape = null) {
            StartSelectionDrag(_grid.GetGridPositionFromWorldPosition(startPositionWorld), selectionShape);
        }

        public void StartSelectionDrag(Vector2Int startPositionGrid, SelectionShape? selectionShape = null) {
            if (!AllowSelection) {
                Debug.Log($"Start selecting not allowed ({nameof(AllowSelection)} was set to false)");
                return;
            }

            if (IsDragging) {
                Debug.LogWarning($"{nameof(GridSelector<TGridNode>)}: Starting a new selection drag, while there already was a drag. Canceling existing drag.");
                CancelDrag();
            }
            _currentSelectionShape = selectionShape ?? DefaultSelectionShape;
            _currentDragStartPosition = startPositionGrid;
            _isDragging = true;
        }

        public IReadOnlyList<Vector2Int> UpdateSelectionDrag(Vector3 updatedPositionWorld) {
            Vector2Int newPosition = _grid.GetGridPositionFromWorldPosition(updatedPositionWorld);
            return UpdateSelectionDrag(newPosition);
        }

        public IReadOnlyList<Vector2Int> UpdateSelectionDrag(Vector2Int newPositionGrid) {
            Vector2Int previousPositionGrid = _currentDragCurrentPosition;
            // TODO previousPositionGrid can be used to optimise this to only update some tiles instead of recalculate everything.
            if (!IsDragging) {
                // TODO maybe a logic whether a tile should also count as selected, when the mouse is just hovering above, like for a building preview, even if it is not selected.
                Debug.Assert(_currentDragArea.Count == 0);
                return _currentDragArea.AsReadOnly();
            }

            if (newPositionGrid == _currentDragCurrentPosition) {
                return _currentDragArea.AsReadOnly(); // The update is only within the same grid node.
            }

            IReadOnlyList<Vector2Int> updatedDragArea = UpdateDragArea(newPositionGrid, previousPositionGrid);
            _gridSelectorDisplay.UpdateDragPreviews(updatedDragArea, _grid);
            return updatedDragArea;
        }
        
        public IReadOnlyList<Vector2Int> EndSelectionDrag() {
            _currentSelection.AddRange(_currentDragArea);
            _currentDragArea.Clear();

            _gridSelectorDisplay.EndSelectionDrag(_currentSelection);
            
            _isSelection = true;

            return _currentSelection;
        }

        public void CancelDrag() {
            _gridSelectorDisplay.CancelSelectionDrag();
            ResetDrag();
        }


        public bool TryGetCurrentSelection(out IReadOnlyList<Vector2Int> selection) {
            if (!_isSelection) {
                selection = null;
                return false;
            }
            selection = _currentSelection;
            return true;
        }

        public bool EndCurrentSelection() {
            return EndCurrentSelection(out _);
        }

        public bool EndCurrentSelection(out IReadOnlyList<Vector2Int> endedSelection) {
            if (!TryGetCurrentSelection(out endedSelection)) {
                return false;
            }

            _currentSelection.Clear();
            _gridSelectorDisplay.EndCurrentSelection(endedSelection);
            _isSelection = false;
            return true;
        }

        private IReadOnlyList<Vector2Int> UpdateDragArea(Vector2Int newPositionGrid, Vector2Int previousPositionGrid) {
            // TODO what to do with grid positions outside the grid?
            if (_currentSelectionShape == SelectionShape.Single ||
                newPositionGrid == _currentDragStartPosition || !IsDragging) {
                // Only one Tile needed
                _currentDragArea.Clear();
                _currentDragArea.Add((IsDragging && ShouldSingleBuildModeStayWhileDragging)
                    ? _currentDragStartPosition
                    : newPositionGrid);
                return _currentDragArea.AsReadOnly();
            }
            
            // Multiple Tiles needed
            _currentDragArea.Clear();
            
            int startX = _currentDragStartPosition.x, startY = _currentDragStartPosition.y;
            int endX = newPositionGrid.x, endY = newPositionGrid.y;

            if (_currentSelectionShape == SelectionShape.Line)
            {
                if (Mathf.Abs(startX - endX) >= Mathf.Abs(startY - endY))
                { // We build a horizontal line
                    endY = startY;
                }
                else
                { // We build a vertical line
                    endX = startX;
                }
            }

            if (startX > endX) (endX, startX) = (startX, endX);
            if (startY > endY) (startY, endY) = (endY, startY);
            // Now start <= end

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    _currentDragArea.Add(new Vector2Int(x, y));
                }
            }

            return _currentDragArea;
        }

        private void ResetDrag() {
            _isDragging = false;
            _currentDragStartPosition = Vector2Int.zero;
            _currentDragCurrentPosition = Vector2Int.zero;
            _currentSelectionShape = DefaultSelectionShape;
            _currentDragArea.Clear();
        }

        public enum SelectionShape {
            Single,
            Area,
            Line,
            LShape
        }
    }
}
