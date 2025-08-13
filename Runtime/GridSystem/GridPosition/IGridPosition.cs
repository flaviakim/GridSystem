using UnityEngine;

namespace GridSystem.GridPosition {
    /// <summary>
    /// The position of something inside a Grid.
    /// A simple version just stores the Node and the (X/Y)-Coordinates.
    /// A more extended version could, for example, also add an offset to position something within the Node.
    /// Or it could add a height of the object, or more.
    ///
    /// This interface is optional and an object could directly store the Node and/or the (X/Y)-Coordinates instead.
    /// </summary>
    /// <typeparam name="TGridNode"> The type of the GridNode used. </typeparam>
    public interface IGridPosition<out TGridNode> where TGridNode : IGridNode<TGridNode> {
        public Vector2 PositionInGrid { get; }
        public Vector3 PositionInWorld { get; }
        public TGridNode Node { get; }
        public int NodeX => Node.X;
        public int NodeY => Node.Y;
    }
}