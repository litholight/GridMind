// src/Utilities/MovementValidator.cs
using System.Collections.Generic;
using GridMind.Environment;

namespace GridMind.Utilities
{
    public static class MovementValidator
    {
        /// <summary>
        /// Checks if a given cell is navigable (i.e., not an obstacle).
        /// </summary>
        public static bool IsNavigable(GridCell cell)
        {
            return cell.Type != CellType.Obstacle;
        }

        /// <summary>
        /// Retrieves the valid navigable neighbors of a given cell in the grid.
        /// </summary>
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

                if (newRow >= 0 && newRow < grid.Rows && newCol >= 0 && newCol < grid.Columns)
                {
                    var neighbor = grid.GetCell(newRow, newCol);
                    if (IsNavigable(neighbor))
                        neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
    }
}
