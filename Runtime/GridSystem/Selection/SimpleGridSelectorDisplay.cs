using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Selection {
    public class SimpleGridSelectorDisplay<TGridNode> : IGridSelectorDisplay<TGridNode> where TGridNode : IGridNode<TGridNode> {
        private readonly List<GameObject> _currentDragPreviewIndicators = new();
        private readonly List<GameObject> _currentSelectionIndicators = new();

        private GameObject _tileDragIndicatorPrefab;
        private GameObject _tileSelectionIndicatorPrefab;

        public GameObject TileDragIndicatorPrefab {
            get { return _tileDragIndicatorPrefab ??= CreateTileDragIndicatorPrefab(); }
            set => _tileDragIndicatorPrefab = value;
        }

        public GameObject TileSelectionIndicatorPrefab {
            get { return _tileSelectionIndicatorPrefab ??= CreateTileSelectionIndicatorPrefab(); }
            set => _tileSelectionIndicatorPrefab = value;
        }

        public Transform TileIndicatorParent { get; set; }

        public SimpleGridSelectorDisplay() { }

        public SimpleGridSelectorDisplay(GameObject tileDragIndicatorPrefab, GameObject tileSelectionIndicatorPrefab) {
            _tileDragIndicatorPrefab = tileDragIndicatorPrefab;
            _tileSelectionIndicatorPrefab = tileSelectionIndicatorPrefab;
        }

        public void UpdateDragPreviews(IReadOnlyList<Vector2Int> currentDragArea, IGrid<TGridNode> grid) {
            RemoveDragPreviews(); // If we optimise UpdateDraggingArea to not completely recalculate everything, but only change some tiles, we should also update this, to only remove/add the needed previews.
            CreateDragPreviews(currentDragArea, grid);
        }

        public void EndSelectionDrag(List<Vector2Int> currentSelection) {
            // TODO use TileSelection indicator instead of reusing the drag previews.
            _currentSelectionIndicators.AddRange(_currentDragPreviewIndicators);
            _currentDragPreviewIndicators.Clear(); // Don't RemoveDragPreviews, as we don't want to destroy the game objects, we reuse them as selection indicators.
        }

        public void EndCurrentSelection(IReadOnlyList<Vector2Int> endedSelection) {
            RemoveSelectionIndicators();
        }

        public void StartDragPreviews(Vector2Int startPositionGrid, IGrid<TGridNode> grid) {
            UpdateDragPreviews(new List<Vector2Int>(new []{startPositionGrid}), grid);
        }

        public void CancelSelectionDrag() {
            RemoveDragPreviews();
        }

        private void CreateDragPreviews(IReadOnlyList<Vector2Int> currentDragArea, IGrid<TGridNode> grid) {
            foreach (Vector2Int tileCoordinate in currentDragArea) {
                CreateSinglePreview(tileCoordinate, grid);
            }
        }

        private void CreateSinglePreview(Vector2Int tileCoordinate, IGrid<TGridNode> grid) {
            Vector3 worldPosition = grid.GetWorldPosition(tileCoordinate);
            // TODO use object pool
            GameObject go = Object.Instantiate(TileDragIndicatorPrefab, worldPosition, Quaternion.identity);
            go.transform.parent = TileIndicatorParent;
            _currentDragPreviewIndicators.Add(go);
        }

        private void RemoveDragPreviews() {
            foreach (GameObject previewObject in _currentDragPreviewIndicators) {
                // TODO use object pool
                Object.Destroy(previewObject.gameObject);
            }
            _currentDragPreviewIndicators.Clear();
        }

        private void RemoveSelectionIndicators() {
            foreach (GameObject selectionIndicator in _currentSelectionIndicators) {
                // TODO use object pool
                Object.Destroy(selectionIndicator.gameObject);
            }
            _currentSelectionIndicators.Clear();
        }

        private static GameObject CreateTileSelectionIndicatorPrefab() {
            Color previewSpriteColorOverlay = new(0, 0, 1, 0.2f);
            var go = new GameObject {
                name = "DefaultTileSelectionIndicatorPrefab",
                layer = 0
            };
            var previewSpriteRendererPrefab = go.AddComponent<SpriteRenderer>();
            previewSpriteRendererPrefab.color = previewSpriteColorOverlay;
            // previewSpriteRendererPrefab.sortingLayerName = "Overlay";
            return go;
        }
        
        private static GameObject CreateTileDragIndicatorPrefab() {
            Color previewSpriteColorOverlay = new(0, 0, 1, 0.2f);
            var go = new GameObject {
                name = "DefaultTileDragIndicatorPrefab",
                layer = 0
            };
            var previewSpriteRendererPrefab = go.AddComponent<SpriteRenderer>();
            previewSpriteRendererPrefab.color = previewSpriteColorOverlay;
            // previewSpriteRendererPrefab.sortingLayerName = "Overlay";
            return go;
        }
    }
}
