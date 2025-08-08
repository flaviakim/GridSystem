using System;

namespace GridSystem.Pathfinding {
    public class PathfindingSystem<TGridNode> where TGridNode : IWalkableGridNode<TGridNode> {
        private readonly Grid<TGridNode> _grid;

        public PathfindingSystem(Grid<TGridNode> grid) {
            _grid = grid;
        }

        public void FindPath(int startX, int startY, int targetX, int targetY) {
            // Implement pathfinding logic here
            // This is just a placeholder for the actual pathfinding algorithm
            Console.WriteLine($"Finding path from ({startX}, {startY}) to ({targetX}, {targetY})");
        }
    }
}