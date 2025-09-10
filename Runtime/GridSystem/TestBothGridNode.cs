// using System;
// using GridSystem.Building;
// using GridSystem.Pathfinding;
//
// namespace GridSystem {
//     public class TestUnit : IGridUnit {
//         
//     }
//     public class TestBothGridNode : IBuildableGridNode<TestBothGridNode>, IWalkableGridNode<TestBothGridNode, TestUnit> {
//         public int X { get; }
//         public int Y { get; }
//         public Grid<TestBothGridNode> Grid { get; }
//         public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
//         public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;
//
//         void IGridNode<TestBothGridNode>.TriggerGridNodeRemoved() {
//             throw new NotImplementedException();
//         }
//         public bool IsBuildable { get; }
//         public IStructure Structure { get; }
//         public bool TryPlaceStructure(IStructure structure) {
//             throw new NotImplementedException();
//         }
//         public void AfterPlacingStructure(TestBothGridNode node, Grid<TestBothGridNode> grid) {
//             throw new NotImplementedException();
//         }
//         public bool RemoveStructure(bool shouldTriggerGridNodeChanged = true) {
//             throw new NotImplementedException();
//         }
//         public bool IsWalkable { get; }
//         public float MovementCost { get; }
//     }
// }