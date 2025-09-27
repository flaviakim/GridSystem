using System;
using UnityEngine.InputSystem;
using UnityEngine;

namespace GridSystem.Selection {
    public class MouseGridSelector<TGridNode> : MonoBehaviour where TGridNode : IGridNode<TGridNode> {
        [field: SerializeField] protected GameObject SelectionDisplayPrefab { get; set; }
        [field: SerializeField] protected GameObject DragDisplayPrefab { get; set; }
        [field: SerializeField] protected SelectionShape SelectionShape { get; set; } = SelectionShape.Area;
        
        public event EventHandler<GridSelectionEventArgs<TGridNode>> SelectionChanged {
            add => Selector.SelectionChanged += value;
            remove => Selector.SelectionChanged -= value;
        }
        
        protected Grid<TGridNode> Grid { get; private set; }


        protected GridSelector<TGridNode> Selector { get; set; }
        protected Camera Cam { get; set; }
        protected Mouse Mouse => Mouse.current;
        protected Vector3 MousePositionWorld => Cam.ScreenToWorldPoint(Mouse.position.ReadValue());
        protected Vector2Int MouseGridPosition => Grid.GetGridPositionFromWorldPosition(MousePositionWorld);
        protected TGridNode MouseGridNode {
            get {
                Vector2Int pos = MouseGridPosition;
                return Grid.GetGridNode(pos.x, pos.y);
            }
        }

        private void OnValidate() {
            if (Selector != null) {
                Selector.DefaultSelectionShape = SelectionShape;
            }
        }


        public void Initialize(Grid<TGridNode> grid) {
            Grid = grid;
            Cam = Camera.main;
            Selector = new GridSelector<TGridNode>(grid,
                new PrefabGridSelectorDisplay<TGridNode>(DragDisplayPrefab, SelectionDisplayPrefab),
                defaultSelectionShape: SelectionShape);
        }

        protected virtual void Update() {
            if (Selector == null || Grid == null || !Cam || Mouse == null) {
                return; // not initialized
            }

            Vector3 mousePositionWorld = MousePositionWorld;
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                Selector.EndCurrentSelection();
                Selector.StartSelectionDrag(mousePositionWorld);
            }
            else if (Mouse.current.leftButton.isPressed) {
                Selector.UpdateSelectionDrag(mousePositionWorld);
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame) {
                Selector.EndSelectionDrag();
            }
        }
    }
}