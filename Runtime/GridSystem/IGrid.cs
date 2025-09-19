using System;
using UnityEngine;

namespace GridSystem {
    public interface IGrid<TGridNode> : IDisposable where TGridNode : IGridNode<TGridNode> {
        int Width { get; }
        int Height { get; }
        float CellSize { get; }
        Vector3 OriginPosition { get; }
        Vector2 Pivot { get; set; }
        
        Vector3 GetWorldPosition(int x, int y);
        Vector3 GetWorldPosition(Vector2 positionInGrid);
        TGridNode GetGridNodeFromWorldPos(Vector3 worldPosition);
        void SetGridNode(int x, int y, TGridNode value);
        TGridNode GetGridNode(int x, int y);
    }
}
