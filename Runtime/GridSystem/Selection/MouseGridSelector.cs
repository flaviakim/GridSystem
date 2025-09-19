using UnityEngine.InputSystem;
using UnityEngine;

namespace GridSystem.Selection {
    public class MouseGridSelector<TGridObject> : MonoBehaviour where TGridObject : IGridNode<TGridObject> {
        
        [SerializeField] private GameObject previewGameObject;
        [SerializeField] private GridSelector<TGridObject>.SelectionShape selectionShape = GridSelector<TGridObject>.SelectionShape.Area;

        private void OnValidate() {
            if (_selector != null) {
                _selector.DefaultSelectionShape = selectionShape;
                _selector.DefaultTileSelectionIndicatorPrefab = previewGameObject;
            }
        }

        private GridSelector<TGridObject> _selector;

        public void Initialize(Grid<TGridObject> grid) {
            _selector = new GridSelector<TGridObject>(grid, tileSelectionIndicatorPrefab: previewGameObject, defaultSelectionShape: selectionShape);
        }

        private void Update() {
            if (_selector == null) {
                return; // not initialized
            }
            var mousePositionScreen = Mouse.current.position.ReadValue();
            var mousePositionWorld = Camera.main.ScreenToWorldPoint(mousePositionScreen);
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                _selector.EndCurrentSelection();
                _selector.StartSelectionDrag(mousePositionWorld);
            } else if (Mouse.current.leftButton.isPressed) {
                _selector.UpdateSelectionDrag(mousePositionWorld);
            } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
                _selector.EndSelectionDrag();
            }
        }
    }
}
