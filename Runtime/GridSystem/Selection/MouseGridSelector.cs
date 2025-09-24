using UnityEngine.InputSystem;
using UnityEngine;

namespace GridSystem.Selection {
    public class MouseGridSelector<TGridObject> : MonoBehaviour where TGridObject : IGridNode<TGridObject> {
        
        [SerializeField] private GameObject previewGameObject;
        [SerializeField] private SelectionShape selectionShape = SelectionShape.Area;

        private void Start() {
            _camera = Camera.main;
        }

        private void OnValidate() {
            if (_selector != null) {
                _selector.DefaultSelectionShape = selectionShape;
            }
        }

        private GridSelector<TGridObject> _selector;
        private Camera _camera;

        public void Initialize(Grid<TGridObject> grid) {
            _selector = new GridSelector<TGridObject>(grid, new SimpleGridSelectorDisplay<TGridObject>(previewGameObject, previewGameObject), defaultSelectionShape: selectionShape);
        }

        private void Update() {
            if (_selector == null) {
                return; // not initialized
            }
            Vector2 mousePositionScreen = Mouse.current.position.ReadValue();
            if (_camera == null) return;
            Vector3 mousePositionWorld = _camera.ScreenToWorldPoint(mousePositionScreen);
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
