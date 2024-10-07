// src/Utilities/MovementValidator.cs
using System.Collections.Generic;
using GridMind.Environment;

namespace GridMind.Utilities
{
    public static class MovementValidator
    {
        public static bool IsNavigable(GridCell cell)
        {
            return cell.Type != CellType.Obstacle;
        }

        public static List<GridCell> GetValidNeighbors(Grid grid, GridCell cell)
        {
            var neighbors = new List<GridCell>();
            var directions = new MovementDirection[]
            {
                MovementDirection.Up,
                MovementDirection.Down,
                MovementDirection.Left,
                MovementDirection.Right
            };

            foreach (var dir in directions)
            {
                int newRow = cell.Row + dir.RowOffset;
                int newCol = cell.Column + dir.ColOffset;

                // Get the wrapped neighbor cell to account for toroidal grid
                var neighbor = grid.GetWrappedCell(newRow, newCol);
                if (IsNavigable(neighbor))
                    neighbors.Add(neighbor);
            }

            return neighbors;
        }
    }
}
