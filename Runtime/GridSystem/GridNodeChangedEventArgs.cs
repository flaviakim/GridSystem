using System;

namespace GridSystem {
    public class GridNodeChangedEventArgs : EventArgs {
        public int X { get; }
        public int Y { get; }

        public GridNodeChangedEventArgs(int x, int y) {
            X = x;
            Y = y;
        }
    }
}