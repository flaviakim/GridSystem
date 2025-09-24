using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static GridSystem.Selection.SelectionShape;

// ReSharper disable once CheckNamespace
namespace GridSystem.Selection.Tests {
    [TestFixture]
    [TestOf(typeof(GridSelector<>))]
    public class GridSelectorTest {
        private GridSelector<TestGridNode> _gridSelector;
        private TestSelectorDisplay _testSelectorDisplay;

        [SetUp]
        public void SetUp() {
            var grid = new Grid<TestGridNode>(5, 5, (g, x, y) => new TestGridNode(x, y, g));
            _testSelectorDisplay = new TestSelectorDisplay();
            _gridSelector = new GridSelector<TestGridNode>(grid, _testSelectorDisplay);
        }
        
        [TearDown]
        public void TearDown() {
            _gridSelector = null;
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void CancelDrag_ShouldCancelDrag(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector3.zero);
            Assert.IsTrue(_gridSelector.IsDragging);
            _gridSelector.CancelDrag();
            Assert.IsFalse(_gridSelector.IsDragging);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndSelectionDrag_ShouldEndDrag(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector3.zero);
            Assert.IsTrue(_gridSelector.IsDragging);
            _gridSelector.EndSelectionDrag();
            Assert.IsFalse(_gridSelector.IsDragging);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void StartSelectionDrag_ShouldStartDrag(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            Assert.IsTrue(_gridSelector.IsDragging);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void StartSelectionDrag_ShouldNotHaveSelectionWhenDragging(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            Assert.IsTrue(_gridSelector.IsDragging);
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void GridSelector_ShouldStartWithoutSelection(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndSelectionDrag_ShouldCreateSelection(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Select2X2Area();
            Assert.IsFalse(_gridSelector.IsDragging);
            Assert.IsTrue(_gridSelector.TryGetCurrentSelection(out IReadOnlyList<Vector2Int> selection));
            Assert2X2SelectionIsRightShape(selection, shape);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void CancelDrag_ShouldNotCreateSelection(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Select2X2Area(cancelInsteadOfEnd: true);
            Assert.IsFalse(_gridSelector.IsDragging);
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndCurrentSelection_ShouldClearSelection(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Select2X2Area();
            Assert.IsTrue(_gridSelector.TryGetCurrentSelection(out _));
            _gridSelector.EndCurrentSelection();
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndCurrentSelection_ShouldReturnFalseWhenNoSelection(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
            Assert.IsFalse(_gridSelector.EndCurrentSelection());
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndSelectionDrag_ShouldDoNothingWhenNotDragging(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.IsFalse(_gridSelector.IsDragging);
            IReadOnlyList<Vector2Int> result = _gridSelector.EndSelectionDrag();
            Assert.IsEmpty(result);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndSelectionDrag_ShouldReturnSameAreaAsLastUpdate(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> firstUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            IReadOnlyList<Vector2Int> endSelection = _gridSelector.EndSelectionDrag();
            CollectionAssert.AreEquivalent(firstUpdate, endSelection);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndCurrentSelection_ShouldReturnFalseWhenDragging(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            Assert.IsTrue(_gridSelector.IsDragging);
            Assert.IsFalse(_gridSelector.EndCurrentSelection());
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldDoNothingWhenNotDragging(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.IsFalse(_gridSelector.IsDragging);
            IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            Assert.IsEmpty(result);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldReturnCurrentDragArea(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            Assert.True(_gridSelector.TryGetCurrentDragArea(out IReadOnlyList<Vector2Int> currentDragArea));
            CollectionAssert.AreEquivalent(result, currentDragArea);
            Assert.IsNotEmpty(result);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldReturnStartingAreaWhenSamePositionAsStart(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            Assert.IsTrue(_gridSelector.TryGetCurrentDragArea(out IReadOnlyList<Vector2Int> startingArea));
            IReadOnlyList<Vector2Int> firstUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.zero);
            CollectionAssert.AreEquivalent(startingArea, firstUpdate);
            IReadOnlyList<Vector2Int> secondUpdate = _gridSelector.UpdateSelectionDrag(new Vector2Int(1, 2));
            if (shape != SingleStay)
                CollectionAssert.AreNotEquivalent(firstUpdate, secondUpdate);
            IReadOnlyList<Vector2Int> thirdUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.zero);
            CollectionAssert.AreEquivalent(firstUpdate, thirdUpdate);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_DragAwayAndBack_ShouldReturnSameAreaAsBefore(SelectionShape shape) {
            if (shape == SingleStay) Assert.Pass("SingleStay always returns the same area");
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> firstUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            IReadOnlyList<Vector2Int> secondUpdate = _gridSelector.UpdateSelectionDrag(new Vector2Int(1, 2));
            CollectionAssert.AreNotEquivalent(firstUpdate, secondUpdate);
            IReadOnlyList<Vector2Int> thirdUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            CollectionAssert.AreEquivalent(firstUpdate, thirdUpdate);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldReturnCorrectAreaWhenDragging(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            Assert.IsNotEmpty(result);
            Assert2X2SelectionIsRightShape(result, shape);
        }


        // [Test]
        // public void UpdateSelectionDrag_Area_ShouldReturnCorrectAreaWhenDragging() {
        //     _gridSelector.DefaultSelectionShape = Area;
        //     _gridSelector.StartSelectionDrag(Vector2Int.zero);
        //     IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
        //     Assert.IsNotEmpty(result);
        //     Assert.AreEqual(4, result.Count);
        //     CollectionAssert.AreEquivalent(new List<Vector2Int> {
        //         new(0, 0),
        //         new(0, 1),
        //         new(1, 0),
        //         new(1, 1),
        //     }, result);
        // }
        //
        // [Test]
        // public void UpdateSelectionDrag_SingleMove_ShouldReturnCorrectAreaWhenDragging() {
        //     _gridSelector.DefaultSelectionShape = SingleMove;
        //     _gridSelector.StartSelectionDrag(Vector2Int.zero);
        //     IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
        //     Assert.IsNotEmpty(result);
        //     Assert.AreEqual(1, result.Count);
        //     CollectionAssert.AreEquivalent(new List<Vector2Int> {
        //         new(1, 1),
        //     }, result);
        // }
        //
        // [Test]
        // public void UpdateSelectionDrag_SingleStay_ShouldReturnCorrectAreaWhenDragging() {
        //     _gridSelector.DefaultSelectionShape = SingleStay;
        //     _gridSelector.StartSelectionDrag(Vector2Int.zero);
        //     IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
        //     Assert.IsNotEmpty(result);
        //     Assert.AreEqual(1, result.Count);
        //     CollectionAssert.AreEquivalent(new List<Vector2Int> {
        //         new(0, 0),
        //     }, result);
        // }
        //
        // [Test]
        // public void UpdateSelectionDrag_Line_ShouldReturnCorrectAreaWhenDragging() {
        //     _gridSelector.DefaultSelectionShape = Line;
        //     _gridSelector.StartSelectionDrag(Vector2Int.zero);
        //     IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
        //     Assert.IsNotEmpty(result);
        //     Assert.AreEqual(2, result.Count);
        //     Assert.True(result.Contains(new Vector2Int(0, 0)));
        //     Assert.False(result.Contains(new Vector2Int(1, 1)));
        //     Assert.True(result.Contains(new Vector2Int(1, 0)) || result.Contains(new Vector2Int(0, 1)));
        // }
        //
        // [Test]
        // public void UpdateSelectionDrag_LShape_ShouldReturnCorrectAreaWhenDragging() {
        //     _gridSelector.DefaultSelectionShape = LShape;
        //     _gridSelector.StartSelectionDrag(Vector2Int.zero);
        //     IReadOnlyList<Vector2Int> result = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
        //     Assert.IsNotEmpty(result);
        //     Assert.AreEqual(3, result.Count);
        //     Assert.True(result.Contains(new Vector2Int(0, 0)));
        //     Assert.True(result.Contains(new Vector2Int(1, 1)));
        //     Assert.True(result.Contains(new Vector2Int(1, 0)) || result.Contains(new Vector2Int(0, 1)));
        //     Assert.False(result.Contains(new Vector2Int(1, 0)) && result.Contains(new Vector2Int(0, 1)));
        // }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldReturnSameAreaWhenSameGridPosition(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> firstUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            IReadOnlyList<Vector2Int> secondUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            CollectionAssert.AreEquivalent(firstUpdate, secondUpdate);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldReturnSameAreaWhenDifferentWorldPositionSameGridPosition(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> firstUpdate = _gridSelector.UpdateSelectionDrag(new Vector2Int(1, 1));
            IReadOnlyList<Vector2Int> secondUpdate = _gridSelector.UpdateSelectionDrag(new Vector3(1.1f, 1.1f)); // Still in same grid position
            CollectionAssert.AreEquivalent(firstUpdate, secondUpdate);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateSelectionDrag_ShouldReturnDifferentAreaWhenDifferentGridPosition(SelectionShape shape) {
            if (shape == SingleStay) {
                Assert.Pass("SingleStay always returns the same area");
            }
            _gridSelector.DefaultSelectionShape = shape;
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            IReadOnlyList<Vector2Int> firstUpdate = _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            IReadOnlyList<Vector2Int> secondUpdate = _gridSelector.UpdateSelectionDrag(new Vector2Int(2, 2));
            CollectionAssert.AreNotEquivalent(firstUpdate, secondUpdate);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void StartDrag_ShouldCallStartDragOnDisplay(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.AreEqual(0, _testSelectorDisplay.StartDragCalled);
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            Assert.AreEqual(1, _testSelectorDisplay.StartDragCalled);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void UpdateDrag_ShouldCallUpdateOnDisplay(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.AreEqual(0, _testSelectorDisplay.UpdateCalled);
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            Assert.AreEqual(1, _testSelectorDisplay.UpdateCalled);
            _gridSelector.UpdateSelectionDrag(new Vector2Int(1, 2));
            Assert.AreEqual(2, _testSelectorDisplay.UpdateCalled);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndDrag_ShouldCallEndDragOnDisplay(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.AreEqual(0, _testSelectorDisplay.EndDragCalled);
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            _gridSelector.EndSelectionDrag();
            Assert.AreEqual(1, _testSelectorDisplay.EndDragCalled);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void CancelDrag_ShouldCallCancelOnDisplay(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.AreEqual(0, _testSelectorDisplay.CancelCalled);
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            _gridSelector.UpdateSelectionDrag(Vector2Int.one);
            _gridSelector.CancelDrag();
            Assert.AreEqual(1, _testSelectorDisplay.CancelCalled);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndCurrentSelection_ShouldCallEndSelectionOnDisplay(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.AreEqual(0, _testSelectorDisplay.EndSelectionCalled);
            Select2X2Area();
            Assert.AreEqual(1, _testSelectorDisplay.EndDragCalled);
            _gridSelector.EndCurrentSelection();
            Assert.AreEqual(1, _testSelectorDisplay.EndSelectionCalled);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void EndCurrentSelection_ShouldNotCallEndSelectionOnDisplayWhenNoSelection(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Assert.AreEqual(0, _testSelectorDisplay.EndSelectionCalled);
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
            _gridSelector.EndCurrentSelection();
            Assert.AreEqual(0, _testSelectorDisplay.EndSelectionCalled);
        }
        
        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void Selecting_ShouldWorkTwiceInARow(SelectionShape shape) {
            _gridSelector.DefaultSelectionShape = shape;
            Select2X2Area();
            Assert.IsTrue(_gridSelector.TryGetCurrentSelection(out IReadOnlyList<Vector2Int> firstSelection));
            Assert2X2SelectionIsRightShape(firstSelection, shape);
            _gridSelector.EndCurrentSelection();
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
            Select2X2Area();
            Assert.IsTrue(_gridSelector.TryGetCurrentSelection(out IReadOnlyList<Vector2Int> secondSelection));
            Assert2X2SelectionIsRightShape(secondSelection, shape);
        }

        [Test]
        [TestCaseSource(nameof(Shapes))]
        public void Selecting_OneShapeAndThenAnother_ShouldWorkBackToBack(SelectionShape shape) {
            int otherShapeNumber = (int)(shape + 1) % Enum.GetValues(typeof(SelectionShape)).Length;
            var otherShape = (SelectionShape)otherShapeNumber;
            _gridSelector.DefaultSelectionShape = shape;
            Select2X2Area();
            Assert.IsTrue(_gridSelector.TryGetCurrentSelection(out IReadOnlyList<Vector2Int> firstSelection));
            Assert2X2SelectionIsRightShape(firstSelection, shape);
            _gridSelector.EndCurrentSelection();
            _gridSelector.DefaultSelectionShape = otherShape;
            Assert.IsFalse(_gridSelector.TryGetCurrentSelection(out _));
            Select2X2Area();
            Assert.IsTrue(_gridSelector.TryGetCurrentSelection(out IReadOnlyList<Vector2Int> secondSelection));
            Assert2X2SelectionIsRightShape(secondSelection, otherShape);
        }

        private void Select2X2Area(bool cancelInsteadOfEnd = false) {
            _gridSelector.StartSelectionDrag(Vector2Int.zero);
            _gridSelector.UpdateSelectionDrag(new Vector2Int(1, 1));
            if (cancelInsteadOfEnd) {
                _gridSelector.CancelDrag();
            }
            else {
                _gridSelector.EndSelectionDrag();
            }
        }

        public static SelectionShape[][] Shapes =
        {
            new SelectionShape[] { SelectionShape.SingleMove },
            new SelectionShape[] { SelectionShape.SingleStay },
            new SelectionShape[] { SelectionShape.Area },
            new SelectionShape[] { SelectionShape.Line },
            new SelectionShape[] { SelectionShape.LShape }
        };

        private static void Assert2X2SelectionIsRightShape(IReadOnlyList<Vector2Int> selection, SelectionShape shape) {
            Assert.IsNotEmpty(selection);
            switch (shape) {
                case SingleMove:
                    Assert.AreEqual(1, selection.Count);
                    CollectionAssert.AreEquivalent(new List<Vector2Int> {
                        new(1, 1),
                    }, selection);
                    break;
                case SingleStay:
                    Assert.AreEqual(1, selection.Count);
                    CollectionAssert.AreEquivalent(new List<Vector2Int> {
                        new(0, 0),
                    }, selection);
                    break;
                case Area:
                    Assert.AreEqual(4, selection.Count);
                    CollectionAssert.AreEquivalent(new List<Vector2Int> {
                        new(0, 0),
                        new(0, 1),
                        new(1, 0),
                        new(1, 1),
                    }, selection);
                    break;
                case Line:
                    Assert.AreEqual(2, selection.Count);
                    Assert.True(selection.Contains(new Vector2Int(0, 0)));
                    Assert.False(selection.Contains(new Vector2Int(1, 1)));
                    Assert.True(selection.Contains(new Vector2Int(1, 0)) || selection.Contains(new Vector2Int(0, 1)));
                    break;
                case LShape:
                    Assert.AreEqual(3, selection.Count);
                    Assert.True(selection.Contains(new Vector2Int(0, 0)));
                    Assert.True(selection.Contains(new Vector2Int(1, 1)));
                    Assert.True(selection.Contains(new Vector2Int(1, 0)) || selection.Contains(new Vector2Int(0, 1)));
                    Assert.False(selection.Contains(new Vector2Int(1, 0)) && selection.Contains(new Vector2Int(0, 1)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }
    }

    public class TestSelectorDisplay : IGridSelectorDisplay<TestGridNode> {
        public int StartDragCalled { get; private set; }
        public int UpdateCalled { get; private set; }
        public int EndDragCalled { get; private set; }
        public int CancelCalled { get; private set; }
        public int EndSelectionCalled { get; private set; }

        public void StartDragPreviews(Vector2Int startPositionGrid, IGrid<TestGridNode> grid) {
            StartDragCalled++;
        }

        public void UpdateDragPreviews(IReadOnlyList<Vector2Int> currentDragArea, IGrid<TestGridNode> grid) {
            UpdateCalled++;
        }

        public void EndSelectionDrag(List<Vector2Int> currentSelection) {
            EndDragCalled++;
        }

        public void CancelSelectionDrag() {
            CancelCalled++;
        }

        public void EndCurrentSelection(IReadOnlyList<Vector2Int> endedSelection) {
            EndSelectionCalled++;
        }
    }

    public class TestGridNode : IGridNode<TestGridNode> {
        public int X => GridPosition.x;
        public int Y => GridPosition.y;
        public Vector2Int GridPosition { get; }
        public Grid<TestGridNode> Grid { get; }
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeChanged;
        public event EventHandler<GridNodeChangedEventArgs> OnGridNodeRemoved;

        public TestGridNode(int x, int y, Grid<TestGridNode> grid) {
            Grid = grid;
            GridPosition = new Vector2Int(x, y);
        }

        public void TriggerGridNodeRemoved() {
            throw new NotImplementedException();
        }
    }
}