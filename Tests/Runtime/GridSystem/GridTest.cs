using GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Runtime.GridSystem {
    [TestFixture]
    [TestOf(typeof(Grid<>))]
    public class GridTest {
        
        private Grid<BaseGridNodeTestImpl> _gridSimple5X5;

        [SetUp]
        public void SetUp() {
            _gridSimple5X5 = new Grid<BaseGridNodeTestImpl>(5, 5, (g, x, y) => new BaseGridNodeTestImpl(g, x, y));
        }


        [Test]
        public void TestGridCreationCreatesAllNodesWithCorrectValues() {
            const int width = 5;
            const int height = 5;
            var grid = new Grid<BaseGridNodeTestImpl>(width, height, (g, x, y) => new BaseGridNodeTestImpl(g, x, y));

            Assert.AreEqual(width, grid.Width);
            Assert.AreEqual(height, grid.Height);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var node = grid.GetGridNode(x, y);
                    Assert.IsNotNull(node);
                    Assert.AreEqual(x, node.X);
                    Assert.AreEqual(y, node.Y);
                    Assert.AreEqual(grid, node.Grid);
                }
            }
        }

        [Test]
        public void TestGetWorldPosition() {
            var grid = _gridSimple5X5;
            var gridOrigin = Vector3.zero;
            Assert.AreEqual(gridOrigin, grid.OriginPosition, "Grid origin position should be (0, 0, 0) by default.");
            var gridCellSize = 1f;
            Assert.AreEqual(gridCellSize, grid.CellSize, "Grid cell size should be 1 by default.");
            var pivot = Vector2.zero;
            Assert.AreEqual(pivot, grid.Pivot, "Grid pivot should be (0, 0) by default.");
            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    var expectedWorldPosition = gridOrigin + new Vector3(x * gridCellSize, y * gridCellSize, 0);
                    var worldPosition = grid.GetWorldPosition(x, y);
                    Assert.AreEqual(expectedWorldPosition, worldPosition, $"World position for cell ({x}, {y}) should be {expectedWorldPosition}, but got {worldPosition}.");
                }
            }
            var expectedOutOfBoundsPosition = gridOrigin + new Vector3(-1 * gridCellSize, -1 * gridCellSize, 0);
            var outOfBoundsWorldPosition = grid.GetWorldPosition(-1, -1);
            Assert.AreEqual(expectedOutOfBoundsPosition, outOfBoundsWorldPosition, "World position for out-of-bounds cell (-1, -1) should be (-1, -1, 0), as it should still work.");
        }
        
        [Test]
        public void TestGetWorldPositionWithPivot() {
            var grid = new Grid<BaseGridNodeTestImpl>(5, 5, 1f, Vector3.zero, new Vector2(0.5f, 0.5f), (g, x, y) => new BaseGridNodeTestImpl(g, x, y));
            var gridOrigin = Vector3.zero;
            Assert.AreEqual(gridOrigin, grid.OriginPosition);
            Assert.AreEqual(1f, grid.CellSize);
            Assert.AreEqual(new Vector2(0.5f, 0.5f), grid.Pivot);
            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    var expectedWorldPosition = gridOrigin + new Vector3(x + 0.5f, y + 0.5f, 0);
                    var worldPosition = grid.GetWorldPosition(x, y);
                    Assert.AreEqual(expectedWorldPosition, worldPosition, 
                        $"World position for cell ({x}, {y}) with pivot (0.5, 0.5) should be {expectedWorldPosition}, but got {worldPosition}.");
                }
            }
            grid.Pivot = Vector2.one;
            Assert.AreEqual(new Vector2(1f, 1f), grid.Pivot, "Grid pivot should be (1, 1) after setting it.");
            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    var expectedWorldPosition = gridOrigin + new Vector3(x + 1f, y + 1f, 0);
                    var worldPosition = grid.GetWorldPosition(x, y);
                    Assert.AreEqual(expectedWorldPosition, worldPosition, $"World position for cell ({x}, {y}) with pivot (1, 1) should be {expectedWorldPosition}, but got {worldPosition}.");
                }
            }
        }
        
    }
}