using GridSystem;

namespace Tests.Runtime.GridSystem {
    public class BaseGridNodeTestImpl : BaseGridNode<BaseGridNodeTestImpl> {
        public BaseGridNodeTestImpl(Grid<BaseGridNodeTestImpl> grid, int x, int y) : base(grid, x, y) { }
    }
}