using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GridSystem.Selection {
    public class GridSelector<TGridNode> where TGridNode : IGridNode<TGridNode> {
        public event EventHandler<GridSelectionEventArgs<TGridNode>> SelectionChanged;
        
        private readonly IGrid<TGridNode> _grid;
        private readonly IGridSelectorDisplay<TGridNode> _gridSelectorDisplay;
        
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

        public GridSelector(IGrid<TGridNode> grid, IGridSelectorDisplay<TGridNode> gridSelectorDisplay = null, SelectionShape defaultSelectionShape = SelectionShape.Area) {
            _grid = grid;
            _gridSelectorDisplay = gridSelectorDisplay;
            DefaultSelectionShape = defaultSelectionShape;
        }

        public void StartSelectionDrag(Vector3 startPositionWorld, SelectionShape? selectionShape = null) {
            // TODO provide the real world start position to the grid selector display
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
            
            Assert.IsTrue(_currentDragArea.Count == 0);
            _currentDragArea.Add(startPositionGrid);
            _currentDragCurrentPosition = startPositionGrid;
            _gridSelectorDisplay.StartDragPreviews(startPositionGrid, _grid);
        }

        // TODO don't always return the list.
        public IReadOnlyList<Vector2Int> UpdateSelectionDrag(Vector3 updatedPositionWorld) {
            Vector2Int newPosition = _grid.GetGridPositionFromWorldPosition(updatedPositionWorld);
            // TODO provide the real world start position to the grid selector display
            return UpdateSelectionDrag(newPosition);
        }

        public IReadOnlyList<Vector2Int> UpdateSelectionDrag(Vector2Int newPositionGrid) {
            Vector2Int previousPositionGrid = _currentDragCurrentPosition; // TODO previousPositionGrid can be used to optimise this to only update some tiles instead of recalculate everything.
            _currentDragCurrentPosition = newPositionGrid;
            if (!IsDragging) {
                // TODO maybe a logic whether a tile should also count as selected, when the mouse is just hovering above, like for a building preview, even if it is not selected.
                Debug.Assert(_currentDragArea.Count == 0);
                return _currentDragArea.ToArray();
            }

            if (newPositionGrid == previousPositionGrid) {
                return _currentDragArea.ToArray(); // The update is only within the same grid node.
            }

            IReadOnlyList<Vector2Int> updatedDragArea = UpdateDragArea(newPositionGrid, previousPositionGrid);
            _gridSelectorDisplay.UpdateDragPreviews(updatedDragArea, _grid);
            return updatedDragArea;
        }
        
        public IReadOnlyList<Vector2Int> EndSelectionDrag() {
            _currentSelection.AddRange(_currentDragArea);
            _currentDragArea.Clear();

            _gridSelectorDisplay.EndSelectionDrag(_currentSelection, _grid);
            
            ResetDrag();
            _isSelection = true;
            Vector2Int[] currentSelectionCopy = _currentSelection.ToArray();
            Debug.Log($"Current selection: {string.Join(", ", currentSelectionCopy)}");
            SelectionChanged?.Invoke(this, new GridSelectionEventArgs<TGridNode>(SelectionChangeType.Started, currentSelectionCopy, _grid));
            return currentSelectionCopy;
        }

        public void CancelDrag() {
            _gridSelectorDisplay.CancelSelectionDrag();
            ResetDrag();
        }

        public bool TryGetCurrentDragArea(out IReadOnlyList<Vector2Int> dragArea) {
            if (!IsDragging) {
                dragArea = null;
                return false;
            }
            dragArea = _currentDragArea.ToArray();
            return true;
        }

        public bool TryGetCurrentSelection(out IReadOnlyList<Vector2Int> selection) {
            if (!_isSelection) {
                selection = null;
                return false;
            }
            selection = _currentSelection.ToArray();
            return true;
        }

        public bool EndCurrentSelection() {
            if (!_isSelection) {
                return false;
            }

            _gridSelectorDisplay.EndCurrentSelection(_currentSelection.ToArray());
            _currentSelection.Clear();
            _isSelection = false;
            SelectionChanged?.Invoke(this, new GridSelectionEventArgs<TGridNode>(SelectionChangeType.Cleared, Array.Empty<Vector2Int>(), _grid));
            return true;
        }

        private IReadOnlyList<Vector2Int> UpdateDragArea(Vector2Int newPositionGrid, Vector2Int previousPositionGrid) {
            // TODO what to do with grid positions outside the grid?
            if (_currentSelectionShape == SelectionShape.SingleStay || _currentSelectionShape == SelectionShape.SingleMove ||
                newPositionGrid == _currentDragStartPosition || !IsDragging) {
                // Only one Tile needed
                _currentDragArea.Clear();
                _currentDragArea.Add((IsDragging && _currentSelectionShape == SelectionShape.SingleStay)
                    ? _currentDragStartPosition
                    : newPositionGrid);
                return _currentDragArea.ToArray();
            }
            
            // Multiple Tiles needed
            _currentDragArea.Clear();
            
            int startX = _currentDragStartPosition.x, startY = _currentDragStartPosition.y;
            int endX = newPositionGrid.x, endY = newPositionGrid.y;
            
            if (startX > endX) (endX, startX) = (startX, endX);
            if (startY > endY) (startY, endY) = (endY, startY);
            // Now start <= end

            bool horizontal = endX - startX >= endY - startY; // only relevant for LShape and Line
            
            switch (_currentSelectionShape) {
                case SelectionShape.Area: {
                    FillArea(startY, endY, startX, endX);
                    break;
                }
                case SelectionShape.Line: {
                    FillLine(startX, endX, startY, endY, horizontal);
                    break;
                }
                case SelectionShape.LShape when horizontal: {
                    for (int x = startX; x <= endX; x++) {
                        _currentDragArea.Add(new Vector2Int(x, startY));
                    }
                    for (int y = startY + 1; y <= endY; y++) {
                        _currentDragArea.Add(new Vector2Int(endX, y));
                    }

                    break;
                }
                case SelectionShape.LShape: { // vertical
                    for (int y = startY; y <= endY; y++) {
                        _currentDragArea.Add(new Vector2Int(startX, y));
                    }
                    for (int x = startX + 1; x <= endX; x++) {
                        _currentDragArea.Add(new Vector2Int(x, endY));
                    }

                    break;
                }
                case SelectionShape.SingleStay:
                case SelectionShape.SingleMove:
                    Debug.LogError($"Selection shape {_currentSelectionShape} should have been handled before.");
                    break;
                default:
                    Debug.LogError($"Selection shape {_currentSelectionShape} not implemented.");
                    break;
            }

            return _currentDragArea.ToArray();
            
            void FillLine(int startXLine, int endXLine, int startYLine, int endYLine, bool horizontalLine) {
                FillArea(
                    startYLine,
                    horizontalLine ? startYLine : endYLine,
                    startXLine,
                    horizontalLine ? endXLine : startXLine);
            }

            void FillArea(int startYArea, int endYArea, int startXArea, int endXArea) {
                for (int y = startYArea; y <= endYArea; y++) {
                    for (int x = startXArea; x <= endXArea; x++) {
                        _currentDragArea.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        private void ResetDrag() {
            _isDragging = false;
            _currentDragStartPosition = Vector2Int.zero;
            _currentDragCurrentPosition = Vector2Int.zero;
            _currentSelectionShape = DefaultSelectionShape;
            _currentDragArea.Clear();
        }
    }
    
    public enum SelectionShape {
        SingleStay,
        SingleMove,
        Area,
        Line,
        LShape
    }
    
    public enum SelectionChangeType {
        Started,
        // Added,
        // Removed,
        Cleared
    }
    
    public class GridSelectionEventArgs<TGridNode> : EventArgs where TGridNode : IGridNode<TGridNode> {
        public SelectionChangeType ChangeType { get; }
        public IReadOnlyList<Vector2Int> CurrentSelection { get; }
        public IGrid<TGridNode> Grid { get; }
        public IReadOnlyList<TGridNode> CurrentSelectedGridNodes => Grid.GetGridNodes(CurrentSelection);
        public GridSelectionEventArgs(SelectionChangeType changeType, IReadOnlyList<Vector2Int> currentSelection, IGrid<TGridNode> grid) {
            ChangeType = changeType;
            CurrentSelection = currentSelection;
            Grid = grid;
        }
    }
}
