using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using GridSystem;
using UnityEngine.TestTools;

namespace Tests.Runtime.GridSystem {
    // --- Mock Grid Node for Testing ---
    public class TestGridNode : IGridNode<TestGridNode> {
        public int X { get; }
        public int Y { get; }
        public Vector2Int GridPosition { get; }
        public Grid<TestGridNode> Grid { get; }

        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;

        public void TriggerGridNodeRemoved() {
            OnGridNodeRemoved?.Invoke(this, new GridNodeChangedEventArgs(X, Y));
            RemovedTriggered = true;
        }

        public bool RemovedTriggered { get; private set; }

        public TestGridNode(Grid<TestGridNode> grid, int x, int y) {
            Grid = grid;
            X = x;
            Y = y;
            GridPosition = new Vector2Int(x, y);
        }

        // Helper for raising node-changed
        public void RaiseChangedEvent() {
            OnGridNodeChanged?.Invoke(this, new GridNodeChangedEventArgs(X, Y));
        }
    }

    [TestFixture]
    public class DefaultGridTests {
        private Grid<TestGridNode> _grid;

        [SetUp]
        public void SetUp() {
            _grid = new Grid<TestGridNode>(3, 3, 1f, Vector3.zero, Vector2.zero,
                (grid, x, y) => new TestGridNode(grid, x, y));
        }

        [TearDown]
        public void TearDown() {
            _grid.Dispose();
        }

        // --- Construction Tests ---
        [Test]
        public void Constructor_ShouldInitializeGridWithCorrectDimensions() {
            Assert.AreEqual(3, _grid.Width);
            Assert.AreEqual(3, _grid.Height);
            Assert.AreEqual(1f, _grid.CellSize);
        }

        // --- Node Retrieval and Setting ---
        [Test]
        public void GetGridNode_ShouldReturnCorrectNode() {
            var node = _grid.GetGridNode(1, 1);
            Assert.AreEqual(1, node.X);
            Assert.AreEqual(1, node.Y);
        }

        // --- Events ---
        [Test]
        public void SetGridNode_ShouldReplaceNodeAndTriggerEvents() {
            var added = false;
            var removed = false;
            _grid.OnGridNodeAdded += (_, _) => added = true;
            _grid.OnGridNodeRemoved += (_, _) => removed = true;

            var newNode = new TestGridNode(_grid, 1, 1);
            _grid.SetGridNode(1, 1, newNode);

            Assert.IsTrue(added);
            Assert.IsTrue(removed);
            Assert.AreEqual(newNode, _grid.GetGridNode(1, 1));
        }

        [Test]
        public void NodeChangedEvent_ShouldPropagateToGrid() {
            var triggered = false;
            _grid.OnGridNodeChanged += (_, _) => triggered = true;

            var node = _grid.GetGridNode(0, 0);
            node.RaiseChangedEvent();

            Assert.IsTrue(triggered);
        }

        [Test]
        public void Dispose_ShouldRemoveNodesAndTriggerRemovedEvents() {
            var removedCount = 0;
            _grid.OnGridNodeRemoved += (_, _) => removedCount++;

            _grid.Dispose();

            Assert.AreEqual(9, removedCount);
        }

        // --- World Position Conversion ---
        [Test]
        public void GetWorldPosition_IntCoordinates_ShouldReturnExpectedPosition() {
            var pos = _grid.GetWorldPosition(2, 2);
            Assert.AreEqual(new Vector3(2, 2, 0), pos);
        }

        [Test]
        public void GetWorldPosition_VectorCoordinates_ShouldReturnExpectedPosition() {
            var pos = _grid.TransformGridPositionToWorld(new Vector2(1.5f, 1.5f));
            Assert.AreEqual(new Vector3(1.5f, 1.5f, 0), pos);
        }

        [Test]
        public void GetWorldPosition_VectorCoordinates1_8_ShouldReturnExpectedPosition() {
            var pos = _grid.TransformGridPositionToWorld(new Vector2(1.8f, 1.8f));
            Assert.AreEqual(new Vector3(1.8f, 1.8f, 0), pos);
        }

        [Test]
        public void GetWorldPosition_VectorCoordinates2_2_ShouldReturnExpectedPosition() {
            var pos = _grid.TransformGridPositionToWorld(new Vector2(2.2f, 2.2f));
            Assert.AreEqual(new Vector3(2.2f, 2.2f, 0), pos);
        }

        [Test]
        public void GetGridNodeFromWorldPos_ShouldReturnCorrectNode() {
            var worldPos = new Vector3(1.2f, 1.8f, 0f);
            var node = _grid.GetGridNodeFromWorldPos(worldPos);

            Assert.AreEqual(1, node.X);
            Assert.AreEqual(1, node.Y);
        }

        [Test]
        public void GetGridNodeFromWorldPos_OutOfBounds_ShouldReturnDefault() {
            // LogAssert.Expect(LogType.Error, "GetGridNodeFromWorldPos: (100, 100, 0) is out of bounds. Returning default.");
            var worldPos = new Vector3(100f, 100f, 0f);
            var node = _grid.GetGridNodeFromWorldPos(worldPos);

            Assert.IsNull(node);
        }

        // --- GetGridNodesInArea ---
        [Test]
        public void GetGridNodesInArea_ShouldReturnCorrectNodes() {
            var nodes = _grid.GetGridNodesInArea(1, 1, 2, 2);

            Assert.AreEqual(4, nodes.Count);
            Assert.That(nodes, Has.Exactly(1).Matches<TestGridNode>(n => n.X == 2 && n.Y == 2));
        }

        [Test]
        public void GetGridNodesInArea_ShouldReturnEmptyList_WhenOutOfBounds() {
            var nodes = _grid.GetGridNodesInArea(5, 5, 2, 2);
            Assert.That(nodes, Is.Empty);
        }

        // --- Roundtrip Tests ---
        [Test]
        public void GridToWorldAndBack_ShouldReturnSameCell() {
            var node = _grid.GetGridNode(1, 1);
            var worldPos = _grid.GetWorldPosition(node.X, node.Y);
            var backNode = _grid.GetGridNodeFromWorldPos(worldPos);

            Assert.AreEqual(node.X, backNode.X);
            Assert.AreEqual(node.Y, backNode.Y);
        }
    }

    [TestFixture]
    public class GridPivotTests {
        private Grid<TestGridNode> _cornerPivotGrid;
        private Grid<TestGridNode> _centerPivotGrid;
        private Grid<TestGridNode> _cornerGridOriginOffset;
        private Grid<TestGridNode> _centerGridOriginOffset;

        [SetUp]
        public void SetUp() {
            _cornerPivotGrid = new Grid<TestGridNode>(width: 2, height: 2, cellSize: 1f, originPosition: Vector3.zero, pivot: Vector2.zero,
                createGridNode: (grid, x, y) => new TestGridNode(grid, x, y));

            _centerPivotGrid = new Grid<TestGridNode>(2, 2, 1f, Vector3.zero, new Vector2(0.5f, 0.5f),
                (grid, x, y) => new TestGridNode(grid, x, y));
            
            _cornerGridOriginOffset = new Grid<TestGridNode>(width: 2, height: 2, cellSize: 1f, originPosition: new Vector3(-0.5f, -0.5f, -0.5f), pivot: Vector2.zero,
                createGridNode: (grid, x, y) => new TestGridNode(grid, x, y));

            _centerGridOriginOffset = new Grid<TestGridNode>(2, 2, 1f, new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.5f, 0.5f),
                (grid, x, y) => new TestGridNode(grid, x, y));
        }

        [TearDown]
        public void TearDown() {
            _cornerPivotGrid.Dispose();
            _centerPivotGrid.Dispose();
            _centerGridOriginOffset.Dispose();
            _cornerGridOriginOffset.Dispose();
        }

        [Test]
        public void CornerPivot_GetWorldPositionZero_ShouldMatchWorldOrigin() {
            var pos = _cornerPivotGrid.GetWorldPosition(0, 0);
            Assert.AreEqual(new Vector3(0, 0, 0), pos);
        }

        [Test]
        public void CenterPivot_GetWorldPositionZero_ShouldMatchWorldOrigin() {
            var pos = _centerPivotGrid.GetWorldPosition(0, 0);
            Assert.AreEqual(new Vector3(0, 0, 0), pos);
        }
        
        [Test]
        public void CornerOffsetPivot_GetWorldPositionZero_ShouldMatchOffset() {
            var pos = _cornerGridOriginOffset.GetWorldPosition(0, 0);
            Assert.AreEqual(new Vector3(-0.5f, -0.5f, -0.5f), pos);
        }

        [Test]
        public void CenterOffsetPivot_GetWorldPosition_ShouldBeOffsetByHalfCell() {
            var pos = _centerGridOriginOffset.GetWorldPosition(0, 0);
            Assert.AreEqual(new Vector3(-0.5f, -0.5f, -0.5f), pos);
        }

        private static void TestRoundTrip(Grid<TestGridNode> grid) {
            TestGridNode node;
            Vector3 world;
            TestGridNode back;
            node = grid.GetGridNode(1, 1);
            world = grid.GetWorldPosition(node.X, node.Y);
            back = grid.GetGridNodeFromWorldPos(world);

            Assert.AreEqual(node.X, back.X);
            Assert.AreEqual(node.Y, back.Y);
        }

        [Test]
        public void CenterPivot_RoundtripConversion_ShouldReturnSameNode() {
            var grid = _centerPivotGrid;
            TestRoundTrip(grid);
        }

        [Test]
        public void CornerPivot_RoundtripConversion_ShouldReturnSameNode() {
            TestRoundTrip(_cornerPivotGrid);
        }
        
        [Test]
        public void CenterPivotOffset_RoundtripConversion_ShouldReturnSameNode() {
            var grid = _centerGridOriginOffset;
            TestRoundTrip(grid);
        }
        
        [Test]
        public void CornerPivotOffset_RoundtripConversion_ShouldReturnSameNode() {
            TestRoundTrip(_cornerGridOriginOffset);
        }

        [Test]
        public void CenterPivot_TransformGridPositionToWorld_WithNonIntegerCoordinates_ShouldBeCorrect() {
            var pos = _centerPivotGrid.TransformGridPositionToWorld(new Vector2(0.5f, 0.5f));
            Assert.AreEqual(new Vector3(1f, 1f, 0), pos);
        }

        [Test]
        public void CenterPivot_GetGridPositionFromWorldPositionZero_ShouldReturnCorrectGridPosition() {
            var grid = _centerPivotGrid;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(0, 0, 0));
            Assert.AreEqual(new Vector2Int(0, 0), gridPos);
        }
        
        [Test]
        public void CornerPivot_GetGridPositionFromWorldPositionZero_ShouldReturnCorrectGridPosition() {
            var grid = _cornerPivotGrid;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(0, 0, 0));
            Assert.AreEqual(new Vector2Int(0, 0), gridPos);
        }

        [Test]
        public void CenterPivotOffset_GetGridPositionFromWorldPositionZero_ShouldReturnCorrectGridPosition() {
            var grid = _centerGridOriginOffset;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(-0.5f, -0.5f, -0.5f));
            Assert.AreEqual(new Vector2Int(0, 0), gridPos);
        }

        [Test]
        public void CornerPivotOffset_GetGridPositionFromWorldPositionGridZero_ShouldReturnCorrectGridPosition() {
            var grid = _cornerGridOriginOffset;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(0, 0, 0));
            Assert.AreEqual(new Vector2Int(0, 0), gridPos);
        }
        
        [Test]
        public void CenterPivot_GetGridPositionFromWorldPosition_ShouldReturnCorrectGridPosition() {
            var grid = _centerPivotGrid;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(1.2f, 0.8f, 0));
            Assert.AreEqual(new Vector2Int(1, 1), gridPos);
        }
        
        [Test]
        public void CornerPivot_GetGridPositionFromWorldPosition_ShouldReturnCorrectGridPosition() {
            var grid = _cornerPivotGrid;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(1.2f, 0.8f, 0));
            Assert.AreEqual(new Vector2Int(1, 0), gridPos);
        }

        [Test]
        public void CenterPivotOffset_GetGridPositionFromWorldPosition_ShouldReturnCorrectGridPosition() {
            var grid = _centerGridOriginOffset;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(0.7f, 0.3f, -0.5f));
            Assert.AreEqual(new Vector2Int(1, 1), gridPos);
        }

        [Test]
        public void CornerPivotOffset_GetGridPositionFromWorldPositionGrid_ShouldReturnCorrectGridPosition() {
            var grid = _cornerGridOriginOffset;
            Vector2Int gridPos = grid.GetGridPositionFromWorldPosition(new Vector3(1.2f, 0.8f, 0));
            Assert.AreEqual(new Vector2Int(1, 1), gridPos);
        }
    }
}
