using JetBrains.Annotations;

namespace GridSystem.Building {
    public class StructureBuilder<TGridNode> where TGridNode : IBuildableGridNode<TGridNode> {
        public StructureBuilder(Grid<TGridNode> grid) {
            Grid = grid;
        }

        public Grid<TGridNode> Grid { get; }
        
        public bool TryPlaceStructure([NotNull] IStructure structure, int x, int y) {
            var success = true;
            var gridNodes = Grid.GetGridNodesInArea(x, y, structure.Width, structure.Height);
            var mainNode = Grid.GetGridNode(x, y);
            foreach (var node in gridNodes) {
                if (!node.IsBuildable || !structure.CanBePlacedOn(node)) return false;
                var nodePlacementSuccess = node.TryPlaceStructure(structure);
                if (!nodePlacementSuccess) {
                    success = false;
                }
            }
            
            if (!success) {
                // If any node failed to place the structure, we need to remove the structure from all nodes that were successfully placed.
                foreach (var node in gridNodes) {
                    if (node.Structure == structure) {
                        node.RemoveStructure(shouldTriggerGridNodeChanged: false);
                    }
                }
                return false;
            }

            structure.AfterPlacing(mainNode, Grid);
            foreach (var node in gridNodes) {
                node.AfterPlacingStructure(mainNode, Grid);
            }

            return true;
        }

        public bool RemoveStructure(int x, int y) {
            var node = Grid.GetGridNode(x, y);
            return node.RemoveStructure(shouldTriggerGridNodeChanged: true);
        }
    }
}