namespace GridSystem.Building {
    public interface IStructure {
        int Width { get; }
        int Height { get; }
        bool AfterPlacing(int x, int y);
        void Remove();
    }
}