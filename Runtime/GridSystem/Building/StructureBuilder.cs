using JetBrains.Annotations;

namespace GridSystem.Building {
    public class StructureBuilder<TGridNode> where TGridNode : IBuildableGridNode<TGridNode> {
        public StructureBuilder(Grid<TGridNode> grid) {
            Grid = grid;
        }

        public Grid<TGridNode> Grid { get; }
        
        public bool TryPlaceStructure([NotNull] IStructure structure, int x, int y) {
            var node = Grid.GetGridNode(x, y);
            if (node.IsBuildable && structure.AfterPlacing(x, y)) {
                return node.PlaceStructure(structure, x, y);
            }
            return false;
        }

        public bool RemoveStructure(int x, int y) {
            var node = Grid.GetGridNode(x, y);
            return node.RemoveStructure();
        }
    }
}