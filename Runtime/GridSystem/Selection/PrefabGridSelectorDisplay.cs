using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridSystem.Selection {
    public class PrefabGridSelectorDisplay<TGridNode> : IGridSelectorDisplay<TGridNode> where TGridNode : IGridNode<TGridNode> {
        private readonly List<GameObject> _currentDragPreviewIndicators = new();
        private readonly List<GameObject> _currentSelectionIndicators = new();

        private GameObject _tileDragIndicatorPrefab;
        private GameObject _tileSelectionIndicatorPrefab;

        private readonly HashSet<Vector2Int> _lastDragArea = new();

        public GameObject TileDragIndicatorPrefab {
            get { return _tileDragIndicatorPrefab ??= CreateTileDragIndicatorPrefab(); }
            set => _tileDragIndicatorPrefab = value;
        }

        public GameObject TileSelectionIndicatorPrefab {
            get { return _tileSelectionIndicatorPrefab ??= CreateTileSelectionIndicatorPrefab(); }
            set => _tileSelectionIndicatorPrefab = value;
        }

        public Transform TileIndicatorParent { get; set; }

        public PrefabGridSelectorDisplay() { }

        public PrefabGridSelectorDisplay(GameObject tileDragIndicatorPrefab, GameObject tileSelectionIndicatorPrefab) {
            _tileDragIndicatorPrefab = tileDragIndicatorPrefab;
            _tileSelectionIndicatorPrefab = tileSelectionIndicatorPrefab;
        }

        public void StartDragPreviews(Vector2Int startPositionGrid, IGrid<TGridNode> grid) {
            UpdateDragPreviews(new List<Vector2Int>(new []{startPositionGrid}), grid);
        }

        public void UpdateDragPreviews(IEnumerable<Vector2Int> currentDragArea, IGrid<TGridNode> grid) {
            HashSet<Vector2Int> currentDragAreaSet = currentDragArea.ToHashSet();
            if (currentDragAreaSet.SetEquals(_lastDragArea)) {
                return;
            }
            RemoveDragIndicators(); // If we optimise UpdateDraggingArea to not completely recalculate everything, but only change some tiles, we should also update this, to only remove/add the needed previews.
            CreateDragPreviews(currentDragAreaSet, grid);
            _lastDragArea.Clear();
            foreach (Vector2Int position in currentDragAreaSet) {
                _lastDragArea.Add(position);
            }
        }

        public void EndSelectionDrag(IEnumerable<Vector2Int> currentSelection, IGrid<TGridNode> grid) {
            RemoveDragIndicators();
            _lastDragArea.Clear();
            CreateSelectionPreviews(currentSelection, grid);
        }

        public void CancelSelectionDrag() {
            RemoveDragIndicators();
            _lastDragArea.Clear();
        }

        public void EndCurrentSelection(IEnumerable<Vector2Int> endedSelection) {
            RemoveSelectionIndicators();
        }

        private void CreateDragPreviews(IEnumerable<Vector2Int> currentDragArea, IGrid<TGridNode> grid) {
            foreach (Vector2Int tileCoordinate in currentDragArea) {
                _currentDragPreviewIndicators.Add(CreateSinglePreview(tileCoordinate, grid, TileDragIndicatorPrefab));
            }
        }

        private void CreateSelectionPreviews(IEnumerable<Vector2Int> currentDragArea, IGrid<TGridNode> grid) {
            foreach (Vector2Int tileCoordinate in currentDragArea) {
                _currentSelectionIndicators.Add(CreateSinglePreview(tileCoordinate, grid,
                    TileSelectionIndicatorPrefab));
            }
        }

        private GameObject CreateSinglePreview(Vector2Int tileCoordinate, IGrid<TGridNode> grid, GameObject prefab) {
            Vector3 worldPosition = grid.GetWorldPosition(tileCoordinate, out _);
            // TODO use object pool
            GameObject go = Object.Instantiate(prefab, worldPosition, Quaternion.identity);
            go.transform.parent = TileIndicatorParent;
            return go;
        }

        private void RemoveDragIndicators() {
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
