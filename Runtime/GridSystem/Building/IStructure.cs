namespace GridSystem.Building {
    public interface IStructure {
        int Width { get; }
        int Height { get; }
        bool Place(int x, int y);
        void Remove();
    }
}