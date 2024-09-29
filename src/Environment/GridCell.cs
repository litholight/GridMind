// src/Environment/GridCell.cs
namespace GridMind.Environment
{
    public class GridCell
    {
        public int Row { get; }
        public int Column { get; }
        public CellType Type { get; set; }

        public GridCell(int row, int column, CellType type = CellType.Empty)
        {
            Row = row;
            Column = column;
            Type = type;
        }
    }

    public enum CellType
    {
        Empty,
        Start,
        Goal,
        Obstacle
    }
}