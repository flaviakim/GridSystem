using GridSystem;
using NUnit.Framework;

namespace Tests.Runtime.GridSystem {
    [TestFixture]
    [TestOf(typeof(Grid<>))]
    public class GridTest {

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
    }
}