using System;
using GridSystem.Building;

namespace GridSystem {
    public abstract class GridEvent : EventArgs {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        protected GridEvent(int x, int y) {
            X = x;
            Y = y;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class GridNodeAddedEvent : GridEvent {
        public GridNodeAddedEvent(int x, int y) : base(x, y) { }
    }

    public class GridNodeRemovedEvent : GridEvent {
        public GridNodeRemovedEvent(int x, int y) : base(x, y) { }
    }

    public class GridNodeChangedEvent : GridEvent {
        public GridNodeChangedEvent(int x, int y)
            : base(x, y) { }
    }

    public class StructurePlacedEvent : GridEvent {
        public IStructure Structure { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public StructurePlacedEvent(int x, int y, IStructure structure, int width, int height)
            : base(x, y) {
            Structure = structure;
            Width = width;
            Height = height;
        }
    }

    public class StructureRemovedEvent : GridEvent {
        public IStructure Structure { get; set; }

        public StructureRemovedEvent(int x, int y, IStructure structure) : base(x, y) {
            Structure = structure;
        }
    }
}