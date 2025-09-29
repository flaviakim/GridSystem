using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Selection {
    public interface IGridSelectorDisplay<TGridNode> where TGridNode : IGridNode<TGridNode> {
        void StartDragPreviews(Vector2Int startPositionGrid, IGrid<TGridNode> grid);
        void UpdateDragPreviews(IEnumerable<Vector2Int> currentDragArea, IGrid<TGridNode> grid);
        void EndSelectionDrag(IEnumerable<Vector2Int> currentSelection, IGrid<TGridNode> grid);
        void CancelSelectionDrag();
        void EndCurrentSelection(IEnumerable<Vector2Int> endedSelection);
    }
}