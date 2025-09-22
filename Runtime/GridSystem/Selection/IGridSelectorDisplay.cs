using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Selection {
    public interface IGridSelectorDisplay<TGridNode> where TGridNode : IGridNode<TGridNode> {
        void UpdateDragPreviews(IReadOnlyList<Vector2Int> currentDragArea, IGrid<TGridNode> grid);
        void EndSelectionDrag(List<Vector2Int> currentSelection);
        void CancelSelectionDrag();
        void EndCurrentSelection(IReadOnlyList<Vector2Int> endedSelection);
    }
}